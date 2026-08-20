export interface SanPhamBanChay {
  productId: number;
  ten: string;
  soLuongBan: number;
  doanhThu: number;
}

export interface DashboardStats {
  doanhThuThangNay: number;
  soDonThangNay: number;
  soDanhGiaChoDuyet: number;
  sanPhamBanChay: SanPhamBanChay[];
}
