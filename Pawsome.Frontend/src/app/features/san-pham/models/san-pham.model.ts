export interface DanhMuc {
  categoryId: number;
  tenDanhMuc: string;
  danhMucChaId: number | null;
  moTa: string | null;
}

export interface ThuongHieu {
  brandId: number;
  tenThuongHieu: string;
  logoUrl: string | null;
}

export interface BienTheSanPham {
  variantId: number;
  tenBienThe: string;
  gia: number;
  soLuongTon: number;
  sku: string | null;
  dangKinhDoanh: boolean;
}

export interface HinhAnhSanPham {
  imageId: number;
  url: string;
  laAnhChinh: boolean;
}

export interface TinhTrangSucKhoe {
  conditionId: number;
  tenTinhTrang: string;
}

export interface SanPham {
  productId: number;
  ten: string;
  moTa: string | null;
  categoryId: number;
  brandId: number | null;
  lieuLuong: string | null;
  giaTu: number | null;
  diemDanhGiaTb: number;
  soLuongDanhGia: number;
  dangKinhDoanh: boolean;
  variants: BienTheSanPham[];
  images: HinhAnhSanPham[];
  conditions: TinhTrangSucKhoe[];
}

export interface GoiYSanPham {
  productId: number;
  ten: string;
  anhChinh: string | null;
  giaTu: number | null;
}

export interface BoLocSanPham {
  tuKhoa?: string;
  categoryId?: number;
  brandId?: number;
  giaMin?: number;
  giaMax?: number;
  danhGiaMin?: number;
  conditionId?: number;
  page?: number;
  pageSize?: number;
}

export interface DanhGia {
  reviewId: number;
  productId: number;
  userId: number;
  tenNguoiDanhGia: string;
  soSao: number;
  binhLuan: string | null;
  trangThai: 'cho_duyet' | 'da_duyet' | 'tu_choi';
  ngayTao: string;
  tenSanPham?: string | null;
  anhSanPham?: string | null;
  diemDanhGiaTbSanPham?: number | null;
}

export interface GuiDanhGia {
  productId: number;
  soSao: number;
  binhLuan?: string;
}

const TEN_DANH_MUC_TIENG_ANH: Record<string, string> = {
  'Chó': 'Dog',
  'Mèo': 'Cat',
  'Ngựa': 'Horse',
  'Chim': 'Bird'
};

export function dichTenDanhMuc(tenViet: string): string {
  return TEN_DANH_MUC_TIENG_ANH[tenViet] ?? tenViet;
}

const TY_GIA_USD = 24000;

export function quyDoiUSD(giaVnd: number | null | undefined): number {
  return (giaVnd ?? 0) / TY_GIA_USD;
}

// Giá PawVip chỉ là placeholder minh họa (DB chỉ có cột la_pawvip đánh dấu thành viên, không lưu
// % giảm giá) - % giảm thống nhất theo giỏ hàng (Phần 4, PHAN_TRAM_GIAM_PAWVIP) là 20%, dùng chung
// 1 công thức duy nhất ở đây để mọi nơi hiển thị (trang chủ, chi tiết sản phẩm...) luôn ra cùng 1
// con số cho cùng 1 sản phẩm, tránh mỗi nơi tự tính 1 kiểu.
export function giaVipPlaceholder(giaThat: number | null | undefined): number {
  return Math.round((giaThat ?? 0) * 0.8);
}

// % lấp vàng của sao thứ "vitri" (1-5) - thể hiện đúng điểm thật (vd 4.8 sao thứ 5 chỉ vàng 80%),
// không làm tròn về nguyên sao. Dùng chung cho mọi nơi vẽ rating (trang chủ, danh sách, chi tiết).
export function phanTramSao(vitri: number, diemTb: number | null | undefined): number {
  return Math.max(0, Math.min(1, (diemTb ?? 0) - (vitri - 1))) * 100;
}