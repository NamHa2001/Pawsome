import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../core/models/api-response.model';
import { AddCartItem, ApplyCoupon, ApplyCouponResult, Cart, UpdateCartItem } from '../models/gio-hang.model';

@Injectable({ providedIn: 'root' })
export class CartService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/gio-hang`;

  layGioHang(): Observable<ApiResponse<Cart>> {
    return this.http.get<ApiResponse<Cart>>(this.baseUrl);
  }

  themSanPham(dto: AddCartItem): Observable<ApiResponse<Cart>> {
    return this.http.post<ApiResponse<Cart>>(`${this.baseUrl}/items`, dto);
  }

  capNhatSoLuong(cartItemId: number, dto: UpdateCartItem): Observable<ApiResponse<Cart>> {
    return this.http.put<ApiResponse<Cart>>(`${this.baseUrl}/items/${cartItemId}`, dto);
  }

  xoaSanPham(cartItemId: number): Observable<ApiResponse<object>> {
    return this.http.delete<ApiResponse<object>>(`${this.baseUrl}/items/${cartItemId}`);
  }

  xoaSachGioHang(): Observable<ApiResponse<object>> {
    return this.http.delete<ApiResponse<object>>(this.baseUrl);
  }

  apDungMaGiamGia(dto: ApplyCoupon): Observable<ApiResponse<ApplyCouponResult>> {
    return this.http.post<ApiResponse<ApplyCouponResult>>(`${this.baseUrl}/ap-dung-ma-giam-gia`, dto);
  }

  xoaMaGiamGia(): Observable<ApiResponse<Cart>> {
    return this.http.delete<ApiResponse<Cart>>(`${this.baseUrl}/ma-giam-gia`);
  }
}