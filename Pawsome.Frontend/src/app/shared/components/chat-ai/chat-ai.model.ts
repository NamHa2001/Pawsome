export interface SanPhamGoiY {
  productId: number;
  ten: string;
  giaTu: number | null;
  anhChinh: string | null;
}

export interface TinNhanChat {
  vai: 'nguoi_dung' | 'bot';
  noiDung: string;
  // Chỉ tin nhắn "bot" mới có thể kèm sản phẩm gợi ý. Có thể có sanPham dù loi=true (VD: đã tra
  // được vài sản phẩm ở lượt gọi hàm trước rồi mới gặp lỗi) - không suy luận sanPham khác rỗng là
  // tin nhắn chắc chắn thành công, luôn dựa vào cờ loi.
  sanPham?: SanPhamGoiY[];
  // true nếu đây là thông báo lỗi/từ chối, không phải câu trả lời AI tổng hợp thật.
  loi?: boolean;
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
  loi: boolean;
}
