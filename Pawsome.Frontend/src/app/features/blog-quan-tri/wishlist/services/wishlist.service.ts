import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../core/models/api-response.model';
import { PagedResult } from '../../../../core/models/paged-result.model';
import { WishlistItem } from '../models/wishlist.model';

@Injectable({ providedIn: 'root' })
export class WishlistService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/Wishlist`;

  layDanhSach(page: number, pageSize: number): Observable<PagedResult<WishlistItem>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http
      .get<ApiResponse<PagedResult<WishlistItem>>>(this.baseUrl, { params })
      .pipe(map(res => res.data ?? { items: [], totalCount: 0, pageNumber: 1, pageSize, totalPages: 0 }));
  }

  xoa(productId: number): Observable<void> {
    return this.http
      .delete<ApiResponse<object>>(`${this.baseUrl}/${productId}`)
      .pipe(map(() => undefined));
  }
}
