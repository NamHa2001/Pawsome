import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../../../environments/environment';
import { ApiResponse } from '../../../../../core/models/api-response.model';
import { PagedResult } from '../../../../../core/models/paged-result.model';
import { AdminOrder, AdminOrderFilter } from '../models/don-hang.model';

@Injectable({ providedIn: 'root' })
export class AdminOrderService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/admin/adminorder`;

  layDanhSach(filter: AdminOrderFilter): Observable<PagedResult<AdminOrder>> {
    let params = new HttpParams()
      .set('page', filter.page ?? 1)
      .set('pageSize', filter.pageSize ?? 10);
    if (filter.trangThai) params = params.set('trangThai', filter.trangThai);

    return this.http
      .get<ApiResponse<PagedResult<AdminOrder>>>(this.baseUrl, { params })
      .pipe(map(res => res.data ?? {
        items: [], totalCount: 0, pageNumber: 1, pageSize: filter.pageSize ?? 10, totalPages: 0
      }));
  }

  capNhatTrangThai(orderId: number, trangThaiMoi: string): Observable<AdminOrder> {
    return this.http
      .put<ApiResponse<AdminOrder>>(`${this.baseUrl}/${orderId}/status`, { trangThaiMoi })
      .pipe(map(res => res.data!));
  }
}
