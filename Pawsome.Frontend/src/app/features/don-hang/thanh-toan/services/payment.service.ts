import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../core/models/api-response.model';
import { CreatePaymentResult } from '../models/thanh-toan.model';

@Injectable({ providedIn: 'root' })
export class PaymentService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/Payments`;

  createMoMoPayment(orderId: number): Observable<CreatePaymentResult> {
    return this.http.post<ApiResponse<CreatePaymentResult>>(`${this.baseUrl}/momo/create-order/${orderId}`, {})
      .pipe(map(res => res.data!));
  }

  createVnPayPayment(orderId: number): Observable<CreatePaymentResult> {
    return this.http.post<ApiResponse<CreatePaymentResult>>(`${this.baseUrl}/vnpay/create-order/${orderId}`, {})
      .pipe(map(res => res.data!));
  }
}