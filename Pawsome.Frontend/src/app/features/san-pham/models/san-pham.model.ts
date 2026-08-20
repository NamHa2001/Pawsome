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