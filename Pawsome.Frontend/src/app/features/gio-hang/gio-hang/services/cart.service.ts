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

  // Tín hiệu dùng chung: các nơi khác (vd Header - Phần 2) có thể đọc để hiển thị số lượng sản phẩm trong giỏ mà không cần gọi API lại.
  readonly gioHang = signal<Cart | null>(null);
  readonly soLuongSanPham = signal(0);

  layGioHang(): Observable<ApiResponse<Cart>> {
    return this.http.get<ApiResponse<Cart>>(this.baseUrl)
      .pipe(tap(res => this.capNhatTinHieu(res.data)));
  }

  themSanPham(dto: AddCartItem): Observable<ApiResponse<Cart>> {
    return this.http.post<ApiResponse<Cart>>(`${this.baseUrl}/items`, dto)
      .pipe(tap(res => this.capNhatTinHieu(res.data)));
  }

  capNhatSoLuong(cartItemId: number, dto: UpdateCartItem): Observable<ApiResponse<Cart>> {
    return this.http.put<ApiResponse<Cart>>(`${this.baseUrl}/items/${cartItemId}`, dto)
      .pipe(tap(res => this.capNhatTinHieu(res.data)));
  }

  xoaSanPham(cartItemId: number): Observable<ApiResponse<object>> {
    return this.http.delete<ApiResponse<object>>(`${this.baseUrl}/items/${cartItemId}`);
  }

  xoaSachGioHang(): Observable<ApiResponse<object>> {
    return this.http.delete<ApiResponse<object>>(this.baseUrl)
      .pipe(tap(() => this.capNhatTinHieu(null)));
  }

  apDungMaGiamGia(dto: ApplyCoupon): Observable<ApiResponse<ApplyCouponResult>> {
    return this.http.post<ApiResponse<ApplyCouponResult>>(`${this.baseUrl}/ap-dung-ma-giam-gia`, dto);
  }

  private capNhatTinHieu(cart: Cart | null): void {
    this.gioHang.set(cart);
    this.soLuongSanPham.set(cart?.items.reduce((tong, i) => tong + i.soLuong, 0) ?? 0);
  }
}