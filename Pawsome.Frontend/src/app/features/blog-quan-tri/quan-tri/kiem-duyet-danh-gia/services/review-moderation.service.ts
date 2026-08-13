import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../../../environments/environment';
import { ApiResponse } from '../../../../../core/models/api-response.model';
import { DanhGia } from '../../../../san-pham/models/san-pham.model';

@Injectable({ providedIn: 'root' })
export class ReviewModerationService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/admin/adminreview`;

  layDanhSachChoDuyet(): Observable<DanhGia[]> {
    return this.http.get<ApiResponse<DanhGia[]>>(`${this.baseUrl}/pending`)
      .pipe(map(res => res.data ?? []));
  }

  duyet(reviewId: number): Observable<DanhGia> {
    return this.http.put<ApiResponse<DanhGia>>(`${this.baseUrl}/${reviewId}/approve`, {})
      .pipe(map(res => res.data!));
  }

  tuChoi(reviewId: number): Observable<DanhGia> {
    return this.http.put<ApiResponse<DanhGia>>(`${this.baseUrl}/${reviewId}/reject`, {})
      .pipe(map(res => res.data!));
  }
}
