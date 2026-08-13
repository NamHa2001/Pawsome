import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../core/models/api-response.model';
import { CreateOrderRequest, Order } from '../models/thanh-toan.model';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/Orders`;

  create(dto: CreateOrderRequest): Observable<Order> {
    return this.http.post<ApiResponse<Order>>(this.baseUrl, dto)
      .pipe(map(res => res.data!));
  }
}