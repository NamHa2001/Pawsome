export interface VariantRequest {
  tenBienThe: string;
  gia: number;
  soLuongTon: number;
  sku: string | null;
}

export interface ImageRequest {
  url: string;
  laAnhChinh: boolean;
}

export interface CreateProductRequest {
  ten: string;
  moTa: string | null;
  categoryId: number;
  brandId: number | null;
  lieuLuong: string | null;
  variants: VariantRequest[];
  images: ImageRequest[];
  conditionIds: number[];
}

export interface UpdateProductRequest {
  ten: string;
  moTa: string | null;
  categoryId: number;
  brandId: number | null;
  lieuLuong: string | null;
  conditionIds: number[];
}

export interface CategoryRequest {
  tenDanhMuc: string;
  danhMucChaId: number | null;
  moTa: string | null;
}

export interface BrandRequest {
  tenThuongHieu: string;
  logoUrl: string | null;
}
