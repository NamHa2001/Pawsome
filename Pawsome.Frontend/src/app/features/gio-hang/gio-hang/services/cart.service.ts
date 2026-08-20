import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../core/models/api-response.model';
import { AddCartItem, ApplyCoupon, ApplyCouponResult, Cart, UpdateCartItem } from '../models/gio-hang.model';

@Injectable({ providedIn: 'root' })
export class CartService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/gio-hang`;
  readonly soLuongGioHang = signal(this.docSoLuongTuLuuTru());

  private docSoLuongTuLuuTru(): number {
    const luu = localStorage.getItem('cartCount');
    return luu ? parseInt(luu, 10) : 0;
  }

  private capNhatSoLuongTuGioHang(gioHang: Cart | null | undefined): void {
    const tong = gioHang?.items?.reduce((t, i) => t + i.soLuong, 0) ?? 0;
    this.soLuongGioHang.set(tong);
    localStorage.setItem('cartCount', String(tong));
  }

  layGioHang(): Observable<ApiResponse<Cart>> {
    return this.http.get<ApiResponse<Cart>>(this.baseUrl)
      .pipe(tap(res => this.capNhatSoLuongTuGioHang(res.data)));
  }

  themSanPham(dto: AddCartItem): Observable<ApiResponse<Cart>> {
    return this.http.post<ApiResponse<Cart>>(`${this.baseUrl}/items`, dto)
      .pipe(tap(res => this.capNhatSoLuongTuGioHang(res.data)));
  }

  capNhatSoLuong(cartItemId: number, dto: UpdateCartItem): Observable<ApiResponse<Cart>> {
    return this.http.put<ApiResponse<Cart>>(`${this.baseUrl}/items/${cartItemId}`, dto)
      .pipe(tap(res => this.capNhatSoLuongTuGioHang(res.data)));
  }

xoaSanPham(cartItemId: number): Observable<ApiResponse<object>> {
  return this.http.delete<ApiResponse<object>>(`${this.baseUrl}/items/${cartItemId}`)
    .pipe(tap(() => this.layGioHang().subscribe()));
}

  xoaSachGioHang(): Observable<ApiResponse<object>> {
    return this.http.delete<ApiResponse<object>>(this.baseUrl)
      .pipe(tap(() => this.capNhatSoLuongTuGioHang(null)));
  }

  apDungMaGiamGia(dto: ApplyCoupon): Observable<ApiResponse<ApplyCouponResult>> {
    return this.http.post<ApiResponse<ApplyCouponResult>>(`${this.baseUrl}/ap-dung-ma-giam-gia`, dto);
  }

  xoaMaGiamGia(): Observable<ApiResponse<Cart>> {
    return this.http.delete<ApiResponse<Cart>>(`${this.baseUrl}/ma-giam-gia`)
      .pipe(tap(res => this.capNhatSoLuongTuGioHang(res.data)));
  }
}