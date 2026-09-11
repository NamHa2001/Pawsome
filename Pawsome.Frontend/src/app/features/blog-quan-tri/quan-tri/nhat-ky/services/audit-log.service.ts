import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../../../environments/environment';
import { ApiResponse } from '../../../../../core/models/api-response.model';
import { PagedResult } from '../../../../../core/models/paged-result.model';
import { AuditLog, AuditLogFilter } from '../models/nhat-ky.model';

@Injectable({ providedIn: 'root' })
export class AuditLogService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/admin/adminauditlog`;

  layDanhSach(filter: AuditLogFilter): Observable<PagedResult<AuditLog>> {
    let params = new HttpParams()
      .set('page', filter.page ?? 1)
      .set('pageSize', filter.pageSize ?? 20);
    if (filter.userId) params = params.set('userId', filter.userId);
    if (filter.hanhDong) params = params.set('hanhDong', filter.hanhDong);
    if (filter.tuNgay) params = params.set('tuNgay', filter.tuNgay);
    if (filter.denNgay) params = params.set('denNgay', filter.denNgay);

    return this.http
      .get<ApiResponse<PagedResult<AuditLog>>>(this.baseUrl, { params })
      .pipe(map(res => res.data ?? {
        items: [], totalCount: 0, pageNumber: 1, pageSize: filter.pageSize ?? 20, totalPages: 0
      }));
  }
}
