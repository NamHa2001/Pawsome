export interface SanPhamGoiY {
  productId: number;
  ten: string;
  giaTu: number | null;
  anhChinh: string | null;
}

export interface TinNhanChat {
  vai: 'nguoi_dung' | 'bot';
  noiDung: string;
  // Chỉ tin nhắn "bot" mới có thể kèm sản phẩm gợi ý (AI đã tra cứu được khi trả lời câu hỏi này).
  sanPham?: SanPhamGoiY[];
}

// Payload gửi lên backend chỉ cần vai/nội dung - không gửi kèm sanPham vì đó là dữ liệu
// hiển thị phía client, backend không cần nhận lại chính kết quả nó vừa trả về.
export interface ChatRequest {
  tinNhanMoi: string;
  lichSu: { vai: string; noiDung: string }[];
}

export interface ChatResponse {
  traLoi: string;
  sanPham: SanPhamGoiY[];
}
