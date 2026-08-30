import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../../../environments/environment';
import { ApiResponse } from '../../../../../core/models/api-response.model';
import { PagedResult } from '../../../../../core/models/paged-result.model';
import { AdminUser, AdminUserFilter } from '../models/nguoi-dung.model';

@Injectable({ providedIn: 'root' })
export class AdminUserService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/admin/adminuser`;

  layDanhSach(filter: AdminUserFilter): Observable<PagedResult<AdminUser>> {
    let params = new HttpParams()
      .set('pageNumber', filter.page ?? 1)
      .set('pageSize', filter.pageSize ?? 10);
    if (filter.keyword) params = params.set('keyword', filter.keyword);
    if (filter.trangThai) params = params.set('trangThai', filter.trangThai);

    return this.http
      .get<ApiResponse<PagedResult<AdminUser>>>(this.baseUrl, { params })
      .pipe(map(res => res.data ?? {
        items: [], totalCount: 0, pageNumber: 1, pageSize: filter.pageSize ?? 10, totalPages: 0
      }));
  }

  layChiTiet(userId: number): Observable<AdminUser> {
    return this.http
      .get<ApiResponse<AdminUser>>(`${this.baseUrl}/${userId}`)
      .pipe(map(res => res.data!));
  }

  khoa(userId: number): Observable<void> {
    return this.http.put<ApiResponse<object>>(`${this.baseUrl}/${userId}/lock`, {}).pipe(map(() => undefined));
  }

  moKhoa(userId: number): Observable<void> {
    return this.http.put<ApiResponse<object>>(`${this.baseUrl}/${userId}/unlock`, {}).pipe(map(() => undefined));
  }

  doiVaiTro(userId: number, roleId: number): Observable<void> {
    return this.http
      .put<ApiResponse<object>>(`${this.baseUrl}/${userId}/role`, { roleId })
      .pipe(map(() => undefined));
  }
}
