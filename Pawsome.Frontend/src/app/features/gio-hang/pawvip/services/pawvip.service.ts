import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../core/models/api-response.model';
import { CreatePaymentResult } from '../../../don-hang/thanh-toan/models/thanh-toan.model';

export interface PawVipStatus {
  tier: string | null;
  hetHan: string | null;
}

export interface PawVipPaymentStatus {
  pawVipPaymentId: number;
  tier: string;
  trangThai: string;
}

@Injectable({ providedIn: 'root' })
export class PawVipService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/pawvip`;

  layTrangThai(): Observable<ApiResponse<PawVipStatus>> {
    return this.http.get<ApiResponse<PawVipStatus>>(`${this.baseUrl}/trang-thai`);
  }

  createMoMoPayment(tier: string): Observable<CreatePaymentResult> {
    return this.http.post<ApiResponse<CreatePaymentResult>>(`${this.baseUrl}/thanh-toan/momo`, { tier })
      .pipe(map(res => res.data!));
  }

  createVnPayPayment(tier: string): Observable<CreatePaymentResult> {
    return this.http.post<ApiResponse<CreatePaymentResult>>(`${this.baseUrl}/thanh-toan/vnpay`, { tier })
      .pipe(map(res => res.data!));
  }

  getPaymentStatus(id: number): Observable<ApiResponse<PawVipPaymentStatus>> {
    return this.http.get<ApiResponse<PawVipPaymentStatus>>(`${this.baseUrl}/thanh-toan/${id}`);
  }
}
