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

  private readonly _soLuongGioHang = signal(0);
  readonly soLuongGioHang = this._soLuongGioHang.asReadonly();

  private capNhatSoLuongTuGioHang(cart: Cart | null | undefined): void {
    const soLoaiSanPham = cart?.items?.length ?? 0;
    this._soLuongGioHang.set(soLoaiSanPham);
  }

  taiLaiSoLuong(): void {
    this.layGioHangGoc().subscribe({
      next: res => this.capNhatSoLuongTuGioHang(res.data),
      error: () => this._soLuongGioHang.set(0)
    });
  }

  private layGioHangGoc(): Observable<ApiResponse<Cart>> {
    return this.http.get<ApiResponse<Cart>>(this.baseUrl);
  }

  layGioHang(): Observable<ApiResponse<Cart>> {
    return this.layGioHangGoc()
      .pipe(tap(res => this.capNhatSoLuongTuGioHang(res.data)));
  }

  themSanPham(dto: AddCartItem): Observable<ApiResponse<Cart>> {
    return this.http.post<ApiResponse<Cart>>(`${this.baseUrl}/items`, dto)
      .pipe(tap(res => this.capNhatSoLuongTuGioHang(res.data)));
  }

  capNhatSoLuong(cartItemId: number, dto: UpdateCartItem): Observable<ApiResponse<Cart>> {
    return this.http.put<ApiResponse<Cart>>(`${this.baseUrl}/items/${cartItemId}`, dto);
  }

  xoaSanPham(cartItemId: number): Observable<ApiResponse<object>> {
    return this.http.delete<ApiResponse<object>>(`${this.baseUrl}/items/${cartItemId}`)
      .pipe(tap(() => this.taiLaiSoLuong()));
  }

  xoaSachGioHang(): Observable<ApiResponse<object>> {
    return this.http.delete<ApiResponse<object>>(this.baseUrl)
      .pipe(tap(() => this._soLuongGioHang.set(0)));
  }

  apDungMaGiamGia(dto: ApplyCoupon): Observable<ApiResponse<ApplyCouponResult>> {
    return this.http.post<ApiResponse<ApplyCouponResult>>(`${this.baseUrl}/ap-dung-ma-giam-gia`, dto);
  }

  xoaMaGiamGia(): Observable<ApiResponse<Cart>> {
    return this.http.delete<ApiResponse<Cart>>(`${this.baseUrl}/ma-giam-gia`)
      .pipe(tap(res => this.capNhatSoLuongTuGioHang(res.data)));
  }
}
