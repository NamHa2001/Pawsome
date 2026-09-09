using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Pawsome.API.DTOs.AI;
using Pawsome.API.DTOs.SanPham;
using Pawsome.API.Services.SanPham;

namespace Pawsome.API.Services.AI;

public class ChatAiService : IChatAiService
{
    private const string TenHamTimSanPham = "tim_kiem_san_pham";
    private const int SoVongLapGoiHamToiDa = 4; // chặn trường hợp model cứ gọi hàm lặp không dừng
    private const int SoTinNhanLichSuToiDa = 16; // giới hạn ngữ cảnh gửi lên mỗi lần, tránh phình chi phí
    private const int DoDaiTinNhanToiDa = 1000;
    private const int SoSanPhamMoiLanTraCuu = 5;
    private const int SoSanPhamGoiYToiDa = 8;

    // GeminiPart là kiểu "oneof" (mỗi Part chỉ có đúng 1 trong text/functionCall/functionResponse) -
    // phải bỏ qua field null khi serialize request, không thì Gemini nhận về "functionCall": null,
    // "functionResponse": null tường minh trên cùng 1 Part chỉ có text, có thể bị hiểu sai cấu trúc.
    private static readonly JsonSerializerOptions _tuyChonJsonGuiDi = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IConditionService _conditionService;
    private readonly ILogger<ChatAiService> _logger;

    public ChatAiService(
        IHttpClientFactory httpClientFactory,
        IConfiguration config,
        IProductService productService,
        ICategoryService categoryService,
        IConditionService conditionService,
        ILogger<ChatAiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _config = config;
        _productService = productService;
        _categoryService = categoryService;
        _conditionService = conditionService;
        _logger = logger;
    }

    public async Task<ChatResponseDto> ChatAsync(ChatRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.TinNhanMoi))
            throw new InvalidOperationException("Vui lòng nhập nội dung câu hỏi.");

        if (request.TinNhanMoi.Length > DoDaiTinNhanToiDa)
            throw new InvalidOperationException($"Câu hỏi quá dài (tối đa {DoDaiTinNhanToiDa} ký tự).");

        var apiKey = _config["Gemini:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("Trợ lý AI chưa được cấu hình (thiếu Gemini:ApiKey).");

        var model = string.IsNullOrWhiteSpace(_config["Gemini:Model"]) ? "gemini-2.5-flash" : _config["Gemini:Model"]!;

        // Gọi song song vì 2 truy vấn độc lập nhau - không có lý do phải chờ tuần tự trên đường dẫn
        // nóng của endpoint công khai này.
        var danhMucTask = _categoryService.GetAllAsync();
        var tinhTrangTask = _conditionService.GetAllAsync();
        await Task.WhenAll(danhMucTask, tinhTrangTask);
        var danhMuc = danhMucTask.Result;
        var tinhTrang = tinhTrangTask.Result;

        var contents = new List<GeminiContent>();
        var lichSu = (request.LichSu ?? new List<ChatMessageDto>())
            .TakeLast(SoTinNhanLichSuToiDa);
        foreach (var tin in lichSu)
        {
            // Chỉ TinNhanMoi được validate độ dài ở trên - LichSu do client tự gửi lên (API công khai,
            // không bắt buộc phải đi qua đúng Frontend) nên phải chặn riêng, không thì 1 request có thể
            // nhét chuỗi rất dài vào lịch sử và bị gửi lặp lại tới Gemini ở mọi vòng lặp gọi hàm.
            if ((tin.NoiDung?.Length ?? 0) > DoDaiTinNhanToiDa)
                throw new InvalidOperationException("Lịch sử hội thoại gửi lên không hợp lệ.");

            contents.Add(new GeminiContent
            {
                Role = tin.Vai == "bot" ? "model" : "user",
                Parts = new List<GeminiPart> { new() { Text = tin.NoiDung } }
            });
        }
        contents.Add(new GeminiContent
        {
            Role = "user",
            Parts = new List<GeminiPart> { new() { Text = request.TinNhanMoi } }
        });

        var tools = new List<GeminiTool> { XayDungTool() };
        var sanPhamGoiY = new List<SanPhamGoiYDto>();
        var client = _httpClientFactory.CreateClient();
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

        // Dựng 1 lần trước vòng lặp - danhMuc/tinhTrang không đổi giữa các lượt gọi hàm trong CÙNG 1
        // request, dựng lại mỗi vòng lặp chỉ tốn CPU vô ích (string.Join + string interpolation lặp
        // lại y hệt tối đa 4 lần cho mỗi tin nhắn của khách).
        var systemInstruction = new GeminiContent
        {
            Parts = new List<GeminiPart> { new() { Text = XayDungSystemPrompt(danhMuc, tinhTrang) } }
        };

        for (var vongLap = 0; vongLap < SoVongLapGoiHamToiDa; vongLap++)
        {
            var geminiRequest = new GeminiRequest
            {
                SystemInstruction = systemInstruction,
                Contents = contents,
                Tools = tools,
                GenerationConfig = new GeminiGenerationConfig()
            };

            HttpResponseMessage httpResponse;
            try
            {
                httpResponse = await client.PostAsJsonAsync(url, geminiRequest, _tuyChonJsonGuiDi);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Không gọi được Gemini API");
                return TraLoiKhiLoi("Không kết nối được tới dịch vụ AI. Vui lòng thử lại sau.");
            }

            if (!httpResponse.IsSuccessStatusCode)
            {
                var noiDungLoi = await httpResponse.Content.ReadAsStringAsync();
                _logger.LogError(
                    "Gemini API trả lỗi {StatusCode}: {NoiDung}", httpResponse.StatusCode, noiDungLoi);

                var thongBao = httpResponse.StatusCode == System.Net.HttpStatusCode.TooManyRequests
                    ? "Trợ lý AI đang quá tải, vui lòng thử lại sau ít phút."
                    : "Trợ lý AI hiện không phản hồi được. Vui lòng thử lại sau.";
                return TraLoiKhiLoi(thongBao);
            }

            var geminiResponse = await httpResponse.Content.ReadFromJsonAsync<GeminiResponse>();

            if (!string.IsNullOrWhiteSpace(geminiResponse?.PromptFeedback?.BlockReason))
            {
                return TraLoiKhiLoi("Xin lỗi, mình không thể trả lời câu hỏi này. Bạn hỏi mình về sản phẩm hoặc tình trạng sức khỏe của thú cưng nhé!");
            }

            var candidate = geminiResponse?.Candidates?.FirstOrDefault();
            if (candidate?.Content == null)
                return TraLoiKhiLoi("Trợ lý AI hiện không phản hồi được. Vui lòng thử lại sau.");

            var functionCallPart = candidate.Content.Parts.FirstOrDefault(p => p.FunctionCall != null);
            if (functionCallPart?.FunctionCall != null)
            {
                // Giữ đúng lượt "model" chứa functionCall trong lịch sử gửi lên - Gemini yêu cầu
                // ngữ cảnh đầy đủ để hiểu functionResponse ở lượt kế tiếp là trả lời cho lệnh gọi nào.
                contents.Add(candidate.Content);

                var ketQuaHam = await ThucThiHamTimSanPhamAsync(functionCallPart.FunctionCall, sanPhamGoiY);

                contents.Add(new GeminiContent
                {
                    Role = "function",
                    Parts = new List<GeminiPart>
                    {
                        new()
                        {
                            FunctionResponse = new GeminiFunctionResponse
                            {
                                Name = functionCallPart.FunctionCall.Name,
                                Response = ketQuaHam
                            }
                        }
                    }
                });

                continue; // gọi lại Gemini để nó tổng hợp câu trả lời cuối dựa trên kết quả tra cứu thật
            }

            var vanBan = string.Concat(candidate.Content.Parts.Where(p => p.Text != null).Select(p => p.Text));
            return new ChatResponseDto
            {
                TraLoi = string.IsNullOrWhiteSpace(vanBan)
                    ? "Xin lỗi, mình chưa hiểu câu hỏi. Bạn có thể hỏi lại rõ hơn không?"
                    : vanBan,
                SanPham = sanPhamGoiY
            };
        }

        // Vẫn trả kèm sanPhamGoiY đã tra được (nếu có) - model gọi hàm tra cứu đủ 4 lượt liên tiếp mà
        // chưa kịp tổng hợp câu trả lời cuối không có nghĩa là những sản phẩm thật đã tìm được trước đó
        // trở thành vô giá trị, không có lý do gì bỏ phí kết quả đã tra cứu được.
        return TraLoiKhiLoi(
            "Câu hỏi này cần tra cứu quá nhiều bước, bạn hỏi cụ thể hơn giúp mình nhé (ví dụ: tên sản phẩm, loài thú cưng, hoặc triệu chứng cụ thể).",
            sanPhamGoiY);
    }

    private static ChatResponseDto TraLoiKhiLoi(string thongBao, List<SanPhamGoiYDto>? sanPham = null) =>
        new() { TraLoi = thongBao, SanPham = sanPham ?? new List<SanPhamGoiYDto>() };

    // Duy nhất 1 tool: tra cứu sản phẩm THẬT trong CSDL qua ProductService.SearchAsync (Phần 2) -
    // bắt buộc AI phải gọi hàm này trước khi nêu tên/giá sản phẩm cụ thể, không tự bịa (xem system
    // prompt). Không cấp cho AI bất kỳ hàm ghi dữ liệu nào.
    private static GeminiTool XayDungTool() => new()
    {
        FunctionDeclarations = new List<GeminiFunctionDeclaration>
        {
            new()
            {
                Name = TenHamTimSanPham,
                Description = "Tìm sản phẩm ĐANG BÁN THẬT trên Pawsome theo từ khóa, danh mục loài thú cưng và/hoặc tình trạng sức khỏe/triệu chứng. Luôn gọi hàm này trước khi nhắc tên hoặc giá một sản phẩm cụ thể - không tự nghĩ ra sản phẩm.",
                Parameters = new
                {
                    type = "OBJECT",
                    properties = new
                    {
                        tu_khoa = new { type = "STRING", description = "Từ khóa tìm theo tên sản phẩm, ví dụ tên thuốc/thương hiệu (tùy chọn)" },
                        category_id = new { type = "INTEGER", description = "category_id của danh mục loài thú cưng, lấy đúng theo danh sách category_id đã cho trong hướng dẫn hệ thống (tùy chọn)" },
                        condition_id = new { type = "INTEGER", description = "condition_id của tình trạng sức khỏe/triệu chứng, lấy đúng theo danh sách condition_id đã cho trong hướng dẫn hệ thống (tùy chọn)" },
                        gia_toi_da = new { type = "NUMBER", description = "Giá tối đa khách chấp nhận, đơn vị VND (tùy chọn)" }
                    }
                }
            }
        }
    };

    private async Task<object> ThucThiHamTimSanPhamAsync(GeminiFunctionCall functionCall, List<SanPhamGoiYDto> sanPhamGoiYTichLuy)
    {
        if (functionCall.Name != TenHamTimSanPham)
            return new { loi = "Không hỗ trợ hàm này." };

        var args = functionCall.Args;
        var filter = new ProductFilterRequestDto
        {
            TuKhoa = LayString(args, "tu_khoa"),
            CategoryId = LayInt(args, "category_id"),
            ConditionId = LayInt(args, "condition_id"),
            GiaMax = LayDecimal(args, "gia_toi_da"),
            Page = 1,
            PageSize = SoSanPhamMoiLanTraCuu
        };

        var ketQua = await _productService.SearchAsync(filter);

        foreach (var sp in ketQua.Items)
        {
            if (sanPhamGoiYTichLuy.Count >= SoSanPhamGoiYToiDa) break;
            if (sanPhamGoiYTichLuy.Any(x => x.ProductId == sp.ProductId)) continue;

            sanPhamGoiYTichLuy.Add(new SanPhamGoiYDto
            {
                ProductId = sp.ProductId,
                Ten = sp.Ten,
                GiaTu = sp.GiaTu,
                AnhChinh = sp.Images.FirstOrDefault(i => i.LaAnhChinh)?.Url ?? sp.Images.FirstOrDefault()?.Url
            });
        }

        // Chỉ mô tả cho Gemini đúng những sản phẩm ĐÃ lọt vào sanPhamGoiYTichLuy (danh sách thẻ sản
        // phẩm sẽ hiển thị) - nếu trả nguyên ketQua.Items không lọc, khi accumulator đã đầy
        // SoSanPhamGoiYToiDa ở lượt gọi hàm trước, Gemini vẫn có thể nhắc tên sản phẩm dư ra trong lời
        // văn mà không có thẻ sản phẩm tương ứng hiển thị (sản phẩm là thật, chỉ là không đồng bộ giữa
        // lời văn và thẻ hiển thị).
        var sanPhamSeMoTa = ketQua.Items
            .Where(sp => sanPhamGoiYTichLuy.Any(x => x.ProductId == sp.ProductId))
            .ToList();

        // Nếu tìm kiếm này thật sự có kết quả nhưng accumulator đã đầy từ lượt gọi hàm trước nên
        // không còn sản phẩm nào lọt vào sanPhamSeMoTa, phải nói rõ cho Gemini biết lý do - nếu không
        // nó có thể hiểu lầm "san_pham rỗng" nghĩa là Pawsome không có sản phẩm phù hợp (dữ liệu sai)
        // trong khi thực ra là do đã đạt giới hạn hiển thị của cuộc trò chuyện này.
        string? ghiChu = sanPhamSeMoTa.Count == 0 && ketQua.Items.Count > 0
            ? "Đã đạt giới hạn số sản phẩm gợi ý hiển thị cho cuộc trò chuyện này (có kết quả nhưng không hiển thị thêm được) - hãy trả lời dựa trên các sản phẩm đã nêu ở lượt trước, không nói là không có sản phẩm phù hợp."
            : null;

        return new
        {
            tong_so_ket_qua = ketQua.TotalCount,
            san_pham = sanPhamSeMoTa.Select(sp => new
            {
                ten = sp.Ten,
                mo_ta = sp.MoTa,
                gia_tu_vnd = sp.GiaTu,
                diem_danh_gia_trung_binh = sp.DiemDanhGiaTb,
                tinh_trang_suc_khoe = sp.Conditions.Select(c => c.TenTinhTrang),
                // Product.DangKinhDoanh (đã lọc ở SearchAsync) chỉ nghĩa là sản phẩm còn được bày bán,
                // KHÔNG đảm bảo còn hàng - phải kiểm thêm tồn kho từng biến thể để không tư vấn chắc
                // nịch một sản phẩm thực chất đã hết hàng.
                con_hang = sp.Variants.Any(v => v.DangKinhDoanh && v.SoLuongTon > 0)
            }),
            ghi_chu = ghiChu
        };
    }

    private static string XayDungSystemPrompt(List<CategoryDto> danhMuc, List<ConditionDto> tinhTrang)
    {
        var dsDanhMuc = string.Join("\n", danhMuc.Select(d => $"- category_id={d.CategoryId}: {d.TenDanhMuc}"));
        var dsTinhTrang = string.Join("\n", tinhTrang.Select(t => $"- condition_id={t.ConditionId}: {t.TenTinhTrang}"));

        return $"""
            Bạn là "PawSome AI Assistant" - trợ lý ảo trên website thương mại điện tử Pawsome, chuyên bán
            sản phẩm chăm sóc thú cưng (thuốc, thực phẩm bổ sung, sản phẩm vệ sinh...).

            PHẠM VI DUY NHẤT được phép trả lời:
            1. Tư vấn sản phẩm Pawsome đang bán dựa trên loài thú cưng và/hoặc triệu chứng/tình trạng sức khỏe khách mô tả.
            2. Kiến thức chăm sóc thú cưng liên quan trực tiếp tới việc chọn sản phẩm phù hợp.

            Nếu khách hỏi bất kỳ điều gì KHÔNG liên quan tới thú cưng hoặc sản phẩm Pawsome (thời tiết, code,
            chính trị, chuyện phiếm...), từ chối lịch sự và mời khách quay lại chủ đề thú cưng/sản phẩm - không
            trả lời nội dung đó dù chỉ một phần.

            QUY TẮC BẮT BUỘC khi tư vấn sản phẩm:
            - Luôn gọi hàm {TenHamTimSanPham} để tra cứu sản phẩm THẬT đang bán trước khi nêu tên hoặc giá bất kỳ
              sản phẩm nào. TUYỆT ĐỐI không tự bịa ra tên sản phẩm, giá, hoặc công dụng không có trong kết quả hàm trả về.
            - Nếu hàm trả về danh sách rỗng, thành thật nói với khách là hiện Pawsome chưa có sản phẩm phù hợp,
              không cố gợi ý sản phẩm không tồn tại.
            - Mỗi sản phẩm hàm trả về có trường con_hang. Nếu con_hang=false, vẫn có thể nhắc tên sản phẩm
              nhưng phải nói rõ hiện đang hết hàng/tạm hết, không mời khách mua ngay như hàng còn sẵn.
            - Khi khách mô tả triệu chứng (ví dụ: ngứa gãi nhiều, có bọ chét, đau khớp, giun sán, hành vi lo lắng...),
              hãy đối chiếu với danh sách tình trạng sức khỏe dưới đây để chọn đúng condition_id gần nghĩa nhất rồi
              mới gọi hàm tra cứu - không đoán tên sản phẩm khi chưa xác định được tình trạng.
            - Bạn không phải bác sĩ thú y: nếu triệu chứng nghiêm trọng hoặc không chắc chắn, khuyên khách đưa thú
              cưng đi khám thú y, bên cạnh việc gợi ý sản phẩm phù hợp nếu có.
            - Trả lời ngắn gọn, thân thiện, bằng tiếng Việt.

            DANH SÁCH DANH MỤC LOÀI THÚ CƯNG (category_id dùng cho hàm {TenHamTimSanPham}):
            {dsDanhMuc}

            DANH SÁCH TÌNH TRẠNG SỨC KHỎE / TRIỆU CHỨNG (condition_id dùng cho hàm {TenHamTimSanPham}):
            {dsTinhTrang}
            """;
    }

    private static JsonElement? LayThuocTinh(JsonElement args, string ten)
    {
        if (args.ValueKind != JsonValueKind.Object || !args.TryGetProperty(ten, out var el))
            return null;

        return el;
    }

    private static int? LayInt(JsonElement args, string ten)
    {
        if (LayThuocTinh(args, ten) is not { } el)
            return null;

        return el.ValueKind switch
        {
            JsonValueKind.Number when el.TryGetInt32(out var soNguyen) => soNguyen,
            // Gemini function-calling đôi khi trả số nguyên dạng có phần thập phân (VD 5.0) dù tham số
            // khai báo kiểu INTEGER - TryGetInt32 trả false cho "5.0" dù giá trị vẫn là số nguyên, nên
            // phải thử thêm qua TryGetDouble và chỉ nhận khi không có phần lẻ thật sự.
            JsonValueKind.Number when el.TryGetDouble(out var soThuc) && soThuc == Math.Floor(soThuc) => (int)soThuc,
            JsonValueKind.String when int.TryParse(el.GetString(), out var soNguyen) => soNguyen,
            _ => null
        };
    }

    private static decimal? LayDecimal(JsonElement args, string ten)
    {
        if (LayThuocTinh(args, ten) is not { } el)
            return null;

        return el.ValueKind switch
        {
            JsonValueKind.Number when el.TryGetDecimal(out var soThap) => soThap,
            JsonValueKind.String when decimal.TryParse(el.GetString(), out var soThap) => soThap,
            _ => null
        };
    }

    private static string? LayString(JsonElement args, string ten)
    {
        if (LayThuocTinh(args, ten) is not { ValueKind: JsonValueKind.String } el)
            return null;

        return el.GetString();
    }
}
