import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../core/models/api-response.model';
import { Coupon } from '../models/gio-hang.model';

@Injectable({ providedIn: 'root' })
export class CouponService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/coupons`;

  layDangHieuLuc(): Observable<Coupon[]> {
    return this.http.get<ApiResponse<Coupon[]>>(`${this.baseUrl}/dang-hieu-luc`)
      .pipe(map(res => res.data ?? []));
  }
}