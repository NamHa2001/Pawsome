import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../core/models/api-response.model';
import { Coupon, CreateCoupon, UpdateCoupon } from '../models/gio-hang.model';

@Injectable({ providedIn: 'root' })
export class CouponService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/coupons`;

  // ── Dùng cho trang giỏ hàng (khách hàng) ──────────────────────────
  layDangHieuLuc(): Observable<Coupon[]> {
    return this.http.get<ApiResponse<Coupon[]>>(`${this.baseUrl}/dang-hieu-luc`)
      .pipe(map(res => res.data ?? []));
  }

  // ── Dùng cho trang quản trị mã giảm giá (Admin) ───────────────────
  layTatCa(): Observable<Coupon[]> {
    return this.http.get<ApiResponse<Coupon[]>>(this.baseUrl)
      .pipe(map(res => res.data ?? []));
  }

  tao(dto: CreateCoupon): Observable<ApiResponse<Coupon>> {
    return this.http.post<ApiResponse<Coupon>>(this.baseUrl, dto);
  }

  capNhat(id: number, dto: UpdateCoupon): Observable<ApiResponse<Coupon>> {
    return this.http.put<ApiResponse<Coupon>>(`${this.baseUrl}/${id}`, dto);
  }

  xoa(id: number): Observable<ApiResponse<object>> {
    return this.http.delete<ApiResponse<object>>(`${this.baseUrl}/${id}`);
  }
}