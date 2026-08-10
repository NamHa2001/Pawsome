import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/api-response.model';
import { PagedResult } from '../../../core/models/paged-result.model';
import { DanhGia, GuiDanhGia } from '../models/san-pham.model';

@Injectable({ providedIn: 'root' })
export class ReviewService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/Reviews`;

  getByProduct(productId: number, page = 1, pageSize = 20): Observable<PagedResult<DanhGia>> {
    return this.http
      .get<ApiResponse<PagedResult<DanhGia>>>(`${this.baseUrl}/product/${productId}`, {
        params: { page, pageSize }
      })
      .pipe(map(res => res.data ?? { items: [], totalCount: 0, pageNumber: 1, pageSize: 20, totalPages: 0 }));
  }

  create(dto: GuiDanhGia): Observable<DanhGia> {
    return this.http
      .post<ApiResponse<DanhGia>>(this.baseUrl, dto)
      .pipe(map(res => res.data as DanhGia));
  }
}
