export interface ReportDateFilter {
  tuNgay?: string; // 'yyyy-MM-dd'
  denNgay?: string; // 'yyyy-MM-dd'
}

export type NhomTheoThoiGian = 'Ngay' | 'Thang';
export type SapXepSanPham = 'SoLuongBan' | 'DoanhThu';
export type LoaiXuatCsv = 'doanh-thu' | 'don-hang' | 'san-pham' | 'danh-muc' | 'thuong-hieu';

export interface SanPhamReportFilter extends ReportDateFilter {
  categoryId?: number;
  brandId?: number;
  page?: number;
  pageSize?: number;
  sapXepTheo?: SapXepSanPham;
}

export interface DoanhThuTheoKy {
  tuNgayKy: string;
  ky: string;
  soDon: number;
  doanhThu: number;
}

export interface DonHangTheoTrangThai {
  trangThai: string;
  soLuong: number;
}

export interface SanPhamBanChay {
  productId: number;
  ten: string;
  soLuongBan: number;
  doanhThu: number;
}

export interface DoanhThuTheoDanhMuc {
  categoryId: number;
  tenDanhMuc: string;
  soLuongBan: number;
  doanhThu: number;
}

export interface DoanhThuTheoThuongHieu {
  brandId: number | null;
  tenThuongHieu: string;
  soLuongBan: number;
  doanhThu: number;
}
