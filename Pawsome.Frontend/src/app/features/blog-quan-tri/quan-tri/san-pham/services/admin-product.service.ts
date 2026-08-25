import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../../../environments/environment';
import { ApiResponse } from '../../../../../core/models/api-response.model';
import { BienTheSanPham, HinhAnhSanPham, SanPham, DanhMuc, ThuongHieu } from '../../../../san-pham/models/san-pham.model';
import {
  BrandRequest,
  CategoryRequest,
  CreateProductRequest,
  ImageRequest,
  UpdateProductRequest,
  VariantRequest
} from '../models/quan-tri-san-pham.model';

@Injectable({ providedIn: 'root' })
export class AdminProductService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/admin/adminproduct`;

  // ── Sản phẩm ───────────────────────────────────────────────────────────
  taoSanPham(dto: CreateProductRequest): Observable<ApiResponse<SanPham>> {
    return this.http.post<ApiResponse<SanPham>>(this.baseUrl, dto);
  }

  suaSanPham(productId: number, dto: UpdateProductRequest): Observable<ApiResponse<SanPham>> {
    return this.http.put<ApiResponse<SanPham>>(`${this.baseUrl}/${productId}`, dto);
  }

  xoaSanPham(productId: number): Observable<ApiResponse<object>> {
    return this.http.delete<ApiResponse<object>>(`${this.baseUrl}/${productId}`);
  }

  // ── Biến thể ───────────────────────────────────────────────────────────
  themBienThe(productId: number, dto: VariantRequest): Observable<ApiResponse<BienTheSanPham>> {
    return this.http.post<ApiResponse<BienTheSanPham>>(`${this.baseUrl}/${productId}/Variants`, dto);
  }

  suaBienThe(productId: number, variantId: number, dto: VariantRequest): Observable<ApiResponse<BienTheSanPham>> {
    return this.http.put<ApiResponse<BienTheSanPham>>(`${this.baseUrl}/${productId}/Variants/${variantId}`, dto);
  }

  xoaBienThe(productId: number, variantId: number): Observable<ApiResponse<object>> {
    return this.http.delete<ApiResponse<object>>(`${this.baseUrl}/${productId}/Variants/${variantId}`);
  }

  // ── Hình ảnh ───────────────────────────────────────────────────────────
  themAnh(productId: number, dto: ImageRequest): Observable<ApiResponse<HinhAnhSanPham>> {
    return this.http.post<ApiResponse<HinhAnhSanPham>>(`${this.baseUrl}/${productId}/Images`, dto);
  }

  xoaAnh(productId: number, imageId: number): Observable<ApiResponse<object>> {
    return this.http.delete<ApiResponse<object>>(`${this.baseUrl}/${productId}/Images/${imageId}`);
  }

  // ── Danh mục ───────────────────────────────────────────────────────────
  taoDanhMuc(dto: CategoryRequest): Observable<ApiResponse<DanhMuc>> {
    return this.http.post<ApiResponse<DanhMuc>>(`${this.baseUrl}/categories`, dto);
  }

  suaDanhMuc(categoryId: number, dto: CategoryRequest): Observable<ApiResponse<DanhMuc>> {
    return this.http.put<ApiResponse<DanhMuc>>(`${this.baseUrl}/categories/${categoryId}`, dto);
  }

  xoaDanhMuc(categoryId: number): Observable<ApiResponse<object>> {
    return this.http.delete<ApiResponse<object>>(`${this.baseUrl}/categories/${categoryId}`);
  }

  // ── Thương hiệu ────────────────────────────────────────────────────────
  taoThuongHieu(dto: BrandRequest): Observable<ApiResponse<ThuongHieu>> {
    return this.http.post<ApiResponse<ThuongHieu>>(`${this.baseUrl}/brands`, dto);
  }

  suaThuongHieu(brandId: number, dto: BrandRequest): Observable<ApiResponse<ThuongHieu>> {
    return this.http.put<ApiResponse<ThuongHieu>>(`${this.baseUrl}/brands/${brandId}`, dto);
  }

  xoaThuongHieu(brandId: number): Observable<ApiResponse<object>> {
    return this.http.delete<ApiResponse<object>>(`${this.baseUrl}/brands/${brandId}`);
  }
}
