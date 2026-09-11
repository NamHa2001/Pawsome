using System.Globalization;
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
    // Tổng số lần THẬT SỰ gọi ProductService.SearchAsync tối đa cho CẢ 1 request, cộng dồn qua mọi
    // lượt gọi Gemini (không phải giới hạn riêng từng lượt) - nếu chỉ giới hạn theo lượt, 1 request có
    // thể đạt tới SoVongLapGoiHamToiDa (4) lượt x giới hạn mỗi lượt = nhiều lần truy vấn DB hơn hẳn mức
    // cần thiết, trong khi rate limit theo IP ở Program.cs chỉ đếm SỐ REQUEST chứ không đếm việc này.
    private const int SoLuongTraCuuThatToiDa = 6;
    private const string ThongBaoLoiChung = "Trợ lý AI hiện không phản hồi được. Vui lòng thử lại sau.";

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

        // PHẢI await tuần tự, KHÔNG Task.WhenAll: ICategoryService/IConditionService cùng dùng chung
        // 1 PawsomeDbContext (scoped, cùng vòng đời với request) - chạy 2 truy vấn EF Core đồng thời
        // trên cùng 1 DbContext instance không an toàn, DbContext sẽ ném
        // "A second operation was started on this context before a previous operation completed"
        // gần như chắc chắn mỗi lần gọi, vì cả 2 câu SQL cùng khởi động gần như đồng thời.
        var danhMuc = await _categoryService.GetAllAsync();
        var tinhTrang = await _conditionService.GetAllAsync();

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
        var soLuongDaTraCuuThat = 0; // cộng dồn qua mọi lượt - xem SoLuongTraCuuThatToiDa
        // Cache dedup PHẢI khai báo NGOÀI vòng lặp gọi Gemini (cộng dồn qua mọi lượt, không phải tạo
        // mới mỗi lượt) - nếu để trong vòng lặp, lệnh gọi hàm với tham số y hệt lặp lại ở LƯỢT SAU
        // (khác lượt) sẽ không được nhận diện là trùng, vẫn tốn thêm 1 suất trong
        // SoLuongTraCuuThatToiDa và truy vấn DB lại dù đã có kết quả từ lượt trước.
        var ketQuaTheoThamSo = new Dictionary<KhoaThamSo, object>();
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
            // HttpRequestException: lỗi mạng/DNS thông thường. TaskCanceledException: HttpClient hết
            // thời gian chờ (mặc định 100s) - .NET ném TaskCanceledException cho timeout, KHÔNG phải
            // HttpRequestException (điểm dễ nhầm) - nếu chỉ bắt HttpRequestException, Gemini phản hồi
            // chậm sẽ lọt thành lỗi 500 chung chung thay vì thông báo thân thiện như các lỗi khác ở đây.
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                _logger.LogError(ex, "Không gọi được Gemini API");
                // Vẫn kèm sanPhamGoiY - nếu lỗi xảy ra ở vòng lặp gọi hàm thứ 2 trở đi, vòng lặp trước
                // đó có thể đã tra được sản phẩm thật rồi, không có lý do gì bỏ phí.
                return TraLoiKhiLoi("Không kết nối được tới dịch vụ AI. Vui lòng thử lại sau.", sanPhamGoiY);
            }

            if (!httpResponse.IsSuccessStatusCode)
            {
                var thongBao = httpResponse.StatusCode == System.Net.HttpStatusCode.TooManyRequests
                    ? "Trợ lý AI đang quá tải, vui lòng thử lại sau ít phút."
                    : ThongBaoLoiChung;

                // Đọc body lỗi để log cũng là I/O mạng như đọc body thành công bên dưới - cùng rủi ro
                // rớt kết nối giữa chừng, nên cũng phải bọc try/catch. thongBao đã tính xong ở trên và
                // được return ngay sau khối try/catch này bất kể đọc log có thành công hay không - vì
                // vậy bắt Exception CHUNG (không giới hạn vài loại exception mạng cụ thể) là an toàn
                // tuyệt đối ở đây: khối try/catch chỉ phục vụ mục đích ghi log, không hề ảnh hưởng gì
                // tới giá trị trả về. Nếu chỉ bắt vài loại exception cụ thể như trước, 1 exception loại
                // khác (VD ObjectDisposedException) sẽ văng thẳng ra ngoài ChatAsync và làm mất luôn
                // sanPhamGoiY đã tra được ở các lượt gọi hàm trước đó trong cùng request - đúng lỗi mà
                // các catch(Exception) khác trong file này đang cố tránh.
                try
                {
                    var noiDungLoi = await httpResponse.Content.ReadAsStringAsync();
                    _logger.LogError(
                        "Gemini API trả lỗi {StatusCode}: {NoiDung}", httpResponse.StatusCode, noiDungLoi);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Gemini API trả lỗi {StatusCode} (không đọc được nội dung lỗi)", httpResponse.StatusCode);
                }

                return TraLoiKhiLoi(thongBao, sanPhamGoiY);
            }

            GeminiResponse? geminiResponse;
            try
            {
                geminiResponse = await httpResponse.Content.ReadFromJsonAsync<GeminiResponse>();
            }
            // JsonException: body 2xx nhưng không đúng dạng GeminiResponse mong đợi. HttpRequestException/
            // IOException/TaskCanceledException: đọc response body cũng là I/O mạng (dù status đã 2xx) -
            // kết nối có thể rớt giữa chừng lúc đang stream body, không chỉ lỗi parse JSON. NotSupportedException:
            // ReadFromJsonAsync có thể ném lỗi này nếu charset khai trong header Content-Type không hợp lệ/không
            // hỗ trợ. Nếu không bắt đủ, lỗi lọt ra ngoài ChatAsync, qua ExceptionHandlingMiddleware thành lỗi 500
            // khác hẳn khung ChatResponseDto và làm mất luôn sanPhamGoiY đã tra được.
            catch (Exception ex) when (ex is JsonException or HttpRequestException or IOException or TaskCanceledException or NotSupportedException)
            {
                _logger.LogError(ex, "Không đọc được phản hồi từ Gemini API");
                return TraLoiKhiLoi(ThongBaoLoiChung, sanPhamGoiY);
            }

            if (!string.IsNullOrWhiteSpace(geminiResponse?.PromptFeedback?.BlockReason))
            {
                return TraLoiKhiLoi(
                    "Xin lỗi, mình không thể trả lời câu hỏi này. Bạn hỏi mình về sản phẩm hoặc tình trạng sức khỏe của thú cưng nhé!",
                    sanPhamGoiY);
            }

            var candidate = geminiResponse?.Candidates?.FirstOrDefault();
            if (candidate?.Content == null)
                return TraLoiKhiLoi(ThongBaoLoiChung, sanPhamGoiY);

            // Gemini có thể gọi hàm NHIỀU lần trong cùng 1 lượt (VD tra 2 triệu chứng khác nhau cùng
            // lúc) dù chỉ khai báo 1 tool.
            var functionCallParts = candidate.Content.Parts.Where(p => p.FunctionCall != null).ToList();
            if (functionCallParts.Count > 0)
            {
                // Phải trả lời ĐỦ từng lệnh gọi (Gemini yêu cầu mỗi functionCall phải có đúng 1
                // functionResponse tương ứng trong lượt kế tiếp, thiếu 1 cái là sai giao thức) - không
                // phải chỉ xử lý lệnh đầu tiên như code cũ. Nhưng chỉ THẬT SỰ truy vấn DB khi chưa đạt
                // SoLuongTraCuuThatToiDa (cộng dồn qua mọi lượt, không phải riêng lượt này) - lệnh dư
                // nhận phản hồi báo đã đạt giới hạn thay vì gọi ProductService.SearchAsync.
                contents.Add(candidate.Content);

                var responseParts = new List<GeminiPart>();
                // ketQuaTheoThamSo khai báo ngoài vòng lặp gọi Gemini (không phải ở đây) - xem chú
                // thích ở chỗ khai báo: cache theo TÊN HÀM + tham số gốc, cộng dồn qua mọi lượt để bắt
                // được cả trường hợp Gemini lặp lại cùng 1 lệnh gọi ở lượt SAU, không chỉ trong cùng lượt.
                foreach (var part in functionCallParts)
                {
                    var fc = part.FunctionCall!;

                    // Khóa dựng từ GIÁ TRỊ ĐÃ PARSE (qua LayString/LayInt/LayDecimal), không phải raw
                    // JSON text - nếu dùng nguyên văn GetRawText(), 2 lệnh gọi cùng ý nghĩa nhưng khác
                    // thứ tự property trong JSON (Gemini có thể phát sinh khác nhau giữa các lượt) sẽ bị
                    // coi là khác nhau, dedup bỏ sót, tốn oan suất trong SoLuongTraCuuThatToiDa.
                    var khoaThamSo = XayDungKhoaThamSo(fc);
                    object ketQuaHam;

                    if (ketQuaTheoThamSo.TryGetValue(khoaThamSo, out var ketQuaDaCo))
                    {
                        ketQuaHam = ketQuaDaCo;
                    }
                    else if (fc.Name != TenHamTimSanPham)
                    {
                        // Không phải hàm tim_kiem_san_pham (chỉ có thể xảy ra nếu Gemini gọi sai tên -
                        // hiện chỉ khai báo đúng 1 tool) - không chạm DB nên KHÔNG tính vào
                        // soLuongDaTraCuuThat, tránh tốn oan 1 suất tra cứu thật cho lệnh gọi không hợp lệ.
                        ketQuaHam = new { loi = "Không hỗ trợ hàm này." };
                        ketQuaTheoThamSo[khoaThamSo] = ketQuaHam;
                    }
                    else if (soLuongDaTraCuuThat < SoLuongTraCuuThatToiDa)
                    {
                        // Tính suất NGAY (trước try) vì sắp thật sự chạm DB bất kể thành hay bại - vẫn
                        // đúng ý nghĩa "đã tốn 1 lượt truy vấn DB thật" kể cả khi lượt đó thất bại.
                        soLuongDaTraCuuThat++;
                        try
                        {
                            // Dùng lại khoaThamSo.ThamSo (đã parse ở XayDungKhoaThamSo phía trên) thay vì
                            // parse lại từ fc.Args - tránh parse trùng 2 lần cho cùng 1 lệnh gọi thật.
                            ketQuaHam = await ThucThiHamTimSanPhamAsync(khoaThamSo.ThamSo, sanPhamGoiY);
                            // CHỈ cache khi THÀNH CÔNG - nếu cache cả lỗi tạm thời, khi Gemini nghe lời
                            // khuyên "vui lòng thử lại" trong thongBaoLoi và gọi lại đúng tham số này ở
                            // lượt sau, cache sẽ phát lại y nguyên lỗi cũ thay vì thực sự thử lại DB,
                            // biến 1 lỗi thoáng qua thành lỗi vĩnh viễn cho suốt phần còn lại của request.
                            ketQuaTheoThamSo[khoaThamSo] = ketQuaHam;
                        }
                        // Bắt Exception CHUNG (không chỉ riêng DbException) xảy ra GIỮA vòng lặp - nếu để
                        // văng ra ngoài ChatAsync sẽ mất luôn sanPhamGoiY đã tra được ở các lệnh gọi TRƯỚC
                        // ĐÓ trong cùng request, trái với đúng mục tiêu "giữ sản phẩm đã tra được" mà mọi
                        // nhánh lỗi khác trong file này đang cố làm. Trước đây chỉ bắt DbException với lý
                        // do "không nuốt bug lập trình thật" - nhưng _logger.LogError(ex, ...) bên dưới
                        // vẫn ghi đầy đủ exception + stack trace, KHÔNG hề giấu bug (vẫn thấy rõ trong
                        // log), nó chỉ tránh trả 500 kèm mất trắng kết quả cho một chuyện không liên quan
                        // (VD lỗi hết connection pool là InvalidOperationException, không phải DbException,
                        // từng lọt qua nhánh catch cũ rồi văng thẳng ra ChatAiController's
                        // catch (InvalidOperationException) và lộ message nội bộ ra ngoài dưới dạng lỗi 400).
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Lỗi khi tra cứu sản phẩm cho trợ lý AI");
                            ketQuaHam = new { loi = "Có lỗi khi tra cứu sản phẩm, vui lòng thử lại." };
                        }
                    }
                    else
                    {
                        ketQuaHam = new { loi = "Đã đạt giới hạn số lượt tra cứu cho câu hỏi này, hãy hỏi cụ thể/ngắn gọn hơn." };
                    }

                    responseParts.Add(new GeminiPart
                    {
                        FunctionResponse = new GeminiFunctionResponse
                        {
                            Id = fc.Id,
                            Name = fc.Name,
                            Response = ketQuaHam
                        }
                    });
                }

                // Role "user" (KHÔNG phải "function") - Gemini 3.x trả lỗi 400 INVALID_ARGUMENT
                // "Role 'function' is not supported" nếu dùng role cũ, xác nhận qua tài liệu chính
                // thức: https://ai.google.dev/gemini-api/docs/gemini-3 - lượt chứa functionResponse
                // giờ phải mang role "user" giống lượt hỏi thường.
                contents.Add(new GeminiContent { Role = "user", Parts = responseParts });

                continue; // gọi lại Gemini để nó tổng hợp câu trả lời cuối dựa trên kết quả tra cứu thật
            }

            var vanBan = string.Concat(candidate.Content.Parts.Where(p => p.Text != null).Select(p => p.Text));
            if (string.IsNullOrWhiteSpace(vanBan))
            {
                // Gemini kết thúc lượt này mà không có cả functionCall lẫn text (VD bị lọc an toàn ở
                // cấp candidate mà không set PromptFeedback.BlockReason) - đây không phải câu trả lời
                // AI thật, phải đánh dấu Loi=true như các nhánh dự phòng khác, không lẫn với thành công.
                return TraLoiKhiLoi("Xin lỗi, mình chưa hiểu câu hỏi. Bạn có thể hỏi lại rõ hơn không?", sanPhamGoiY);
            }

            return new ChatResponseDto { TraLoi = vanBan, SanPham = sanPhamGoiY };
        }

        // Vẫn trả kèm sanPhamGoiY đã tra được (nếu có) - model gọi hàm tra cứu đủ 4 lượt liên tiếp mà
        // chưa kịp tổng hợp câu trả lời cuối không có nghĩa là những sản phẩm thật đã tìm được trước đó
        // trở thành vô giá trị, không có lý do gì bỏ phí kết quả đã tra cứu được.
        return TraLoiKhiLoi(
            "Câu hỏi này cần tra cứu quá nhiều bước, bạn hỏi cụ thể hơn giúp mình nhé (ví dụ: tên sản phẩm, loài thú cưng, hoặc triệu chứng cụ thể).",
            sanPhamGoiY);
    }

    private static ChatResponseDto TraLoiKhiLoi(string thongBao, List<SanPhamGoiYDto>? sanPham = null) =>
        new() { TraLoi = thongBao, SanPham = sanPham ?? new List<SanPhamGoiYDto>(), Loi = true };

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

    // Parse 4 tham số của hàm tim_kiem_san_pham DUY NHẤT 1 lần ở đây - dùng chung cho cả khóa dedup
    // (XayDungKhoaThamSo) lẫn filter truy vấn thật (ThucThiHamTimSanPhamAsync). Trước đây 2 nơi tự
    // gọi lại LayString/LayInt/LayDecimal riêng, cùng đọc đúng 4 tên tham số - nếu sau này đổi/thêm
    // tham số cho hàm mà chỉ sửa 1 trong 2 chỗ, khóa dedup và filter thật sẽ lệch nhau (dedup bỏ sót
    // 1 tham số mới sẽ coi 2 lệnh gọi khác nhau là trùng, trả nhầm kết quả cache).
    private static ThamSoTimKiem TrichThamSoTimKiem(JsonElement args) => new(
        LayString(args, "tu_khoa"),
        LayInt(args, "category_id"),
        LayInt(args, "condition_id"),
        LayDecimal(args, "gia_toi_da"));

    private readonly record struct ThamSoTimKiem(
        string? TuKhoa, int? CategoryId, int? ConditionId, decimal? GiaToiDa);

    // Khóa dedup dạng tuple có so sánh cấu trúc (không phải chuỗi ghép bằng string.Join) - nếu ghép
    // thành 1 chuỗi bằng dấu phân cách (VD "|"), tu_khoa là chuỗi tự do do Gemini/khách nhập nên có
    // thể tự chứa đúng dấu phân cách đó, khiến 2 lệnh gọi có tham số khác nhau (VD tu_khoa="x|5" so
    // với tu_khoa="x" + category_id=5) ghép ra CÙNG 1 chuỗi khóa và bị coi là trùng - tuple tránh
    // hoàn toàn kiểu đụng độ này vì so sánh từng field riêng, không qua bước ghép chuỗi. Bọc luôn
    // ThamSoTimKiem làm 1 field duy nhất (không lặp lại từng field của nó) - record struct so sánh
    // cấu trúc lồng nhau tự động, tránh phải sửa 2 chỗ (KhoaThamSo và ThamSoTimKiem) mỗi khi hàm
    // tim_kiem_san_pham đổi/thêm tham số.
    private static KhoaThamSo XayDungKhoaThamSo(GeminiFunctionCall functionCall) =>
        new(functionCall.Name, TrichThamSoTimKiem(functionCall.Args));

    private readonly record struct KhoaThamSo(string TenHam, ThamSoTimKiem ThamSo);

    // Nhận thẳng ThamSoTimKiem đã parse sẵn (từ khoaThamSo.ThamSo ở nơi gọi) thay vì nhận
    // GeminiFunctionCall rồi tự parse lại - tránh gọi TrichThamSoTimKiem 2 lần cho cùng 1 lệnh gọi
    // (1 lần dựng khóa dedup, 1 lần dựng filter thật). Đồng thời loại bỏ hẳn khái niệm "tên hàm" khỏi
    // chữ ký hàm này - không còn cách nào gọi nhầm hàm khác qua đây được nữa, không cần precondition
    // comment như trước.
    private async Task<object> ThucThiHamTimSanPhamAsync(ThamSoTimKiem thamSo, List<SanPhamGoiYDto> sanPhamGoiYTichLuy)
    {
        var filter = new ProductFilterRequestDto
        {
            TuKhoa = thamSo.TuKhoa,
            CategoryId = thamSo.CategoryId,
            ConditionId = thamSo.ConditionId,
            GiaMax = thamSo.GiaToiDa,
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
            - Nếu khách chỉ nêu MỘT triệu chứng chung chung, có thể do nhiều nguyên nhân khác nhau (ví dụ: bỏ ăn,
              mệt mỏi, nôn, đi ngoài, bỏ ăn kèm lừ đừ...), ĐỪNG vội gọi hàm tra cứu ngay - hãy hỏi lại 1-2 câu ngắn
              gọn để làm rõ trước (ví dụ: có kèm tiêu chảy/nôn không, tình trạng kéo dài mấy ngày rồi, còn dấu hiệu
              bất thường nào khác không) rồi mới chọn đúng condition_id và gọi hàm dựa trên câu trả lời đó. Chỉ gọi
              hàm tra cứu ngay khi triệu chứng khách mô tả đã đủ cụ thể để khớp rõ ràng với 1 tình trạng trong danh
              sách bên dưới (ví dụ: có bọ chét, đau khớp, ngứa gãi nhiều), hoặc khi khách đã trả lời đủ các câu hỏi
              làm rõ ở lượt trước.
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
            // phải thử thêm qua TryGetDouble và chỉ nhận khi không có phần lẻ thật sự. Phải kiểm tra
            // nằm trong khoảng int trước khi ép kiểu - double ngoài phạm vi Int32 (VD 5_000_000_000)
            // vẫn có thể qua được điều kiện "không có phần lẻ", nhưng (int)soThuc sẽ tràn số âm thầm
            // (không ném OverflowException vì đây không phải khối checked) ra 1 category_id/condition_id
            // vô nghĩa thay vì bị coi là không có giá trị.
            JsonValueKind.Number when el.TryGetDouble(out var soThuc) && soThuc == Math.Floor(soThuc)
                && soThuc is >= int.MinValue and <= int.MaxValue => (int)soThuc,
            JsonValueKind.String when int.TryParse(el.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var soNguyen) => soNguyen,
            // Cùng lý do với nhánh Number ở trên (VD chuỗi "5.0", và cùng cần chặn tràn số Int32) - phải
            // xử lý nhất quán giữa 2 kiểu ValueKind, nếu không chuỗi "5.0" sẽ bị âm thầm coi là không có
            // giá trị trong khi số 5.0 (không phải chuỗi) vẫn được nhận đúng thành 5. BẮT BUỘC
            // CultureInfo.InvariantCulture - double.TryParse mặc định dùng CurrentCulture, ở culture coi
            // "." là dấu phân cách hàng nghìn (VD vi-VN) thì "5.0" sẽ bị đọc thành 50 thay vì 5.0, sai
            // lệch gấp 10 lần mà không hề có exception nào báo.
            JsonValueKind.String when double.TryParse(el.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var soThuc) && soThuc == Math.Floor(soThuc)
                && soThuc is >= int.MinValue and <= int.MaxValue => (int)soThuc,
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
            // CultureInfo.InvariantCulture - cùng lý do với LayInt: decimal.TryParse mặc định dùng
            // CurrentCulture, có thể đọc sai dấu phân cách thập phân/hàng nghìn tùy server đang chạy
            // culture nào (gia_toi_da là giá tiền, đọc sai gấp/chia 1000 lần là lỗi nghiêm trọng).
            // PHẢI dùng NumberStyles.Number (không phải Float) - Number có thêm AllowThousands so với
            // Float, giữ đúng hành vi decimal.TryParse(string) mặc định trước đây (đã cho qua chuỗi có
            // dấu phân cách hàng nghìn kiểu "500,000") - nếu chỉ dùng Float, giá dạng "500,000" sẽ bị
            // parse thất bại, gia_toi_da lặng lẽ thành null (mất hẳn điều kiện lọc giá) thay vì đúng
            // 500000, dù mục tiêu sửa ban đầu chỉ là cố định culture, không phải bớt định dạng được chấp nhận.
            JsonValueKind.String when decimal.TryParse(el.GetString(), NumberStyles.Number, CultureInfo.InvariantCulture, out var soThap) => soThap,
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
