import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../core/models/api-response.model';
import { PagedResult } from '../../../../core/models/paged-result.model';
import {
  CancelOrderRequest, CreateOrderRequest, Order, OrderFilter, ReturnOrderRequest
} from '../models/thanh-toan.model';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/Orders`;

  create(dto: CreateOrderRequest): Observable<Order> {
    return this.http.post<ApiResponse<Order>>(this.baseUrl, dto)
      .pipe(map(res => res.data!));
  }

  getMyOrders(filter: OrderFilter): Observable<PagedResult<Order>> {
    let params = new HttpParams()
      .set('page', filter.page ?? 1)
      .set('pageSize', filter.pageSize ?? 5);
    if (filter.trangThai) params = params.set('trangThai', filter.trangThai);

    return this.http.get<ApiResponse<PagedResult<Order>>>(this.baseUrl, { params })
      .pipe(map(res => res.data ?? { items: [], totalCount: 0, pageNumber: 1, pageSize: 5, totalPages: 0 }));
  }

  tongTietKiem(): Observable<number> {
    return this.http.get<ApiResponse<number>>(`${this.baseUrl}/tong-tiet-kiem`)
      .pipe(map(res => res.data ?? 0));
  }

  getById(orderId: number): Observable<Order> {
    return this.http.get<ApiResponse<Order>>(`${this.baseUrl}/${orderId}`)
      .pipe(map(res => res.data!));
  }

  cancel(orderId: number, dto: CancelOrderRequest): Observable<Order> {
    return this.http.post<ApiResponse<Order>>(`${this.baseUrl}/${orderId}/cancel`, dto)
      .pipe(map(res => res.data!));
  }

  requestReturn(orderId: number, dto: ReturnOrderRequest): Observable<Order> {
    return this.http.post<ApiResponse<Order>>(`${this.baseUrl}/${orderId}/return-request`, dto)
      .pipe(map(res => res.data!));
  }
}