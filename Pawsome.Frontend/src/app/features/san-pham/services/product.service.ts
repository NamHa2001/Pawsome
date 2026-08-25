import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/api-response.model';
import { PagedResult } from '../../../core/models/paged-result.model';
import { BoLocSanPham, GoiYSanPham, SanPham } from '../models/san-pham.model';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/Products`;

  search(filter: BoLocSanPham): Observable<PagedResult<SanPham>> {
    let params = new HttpParams();
    for (const [key, value] of Object.entries(filter)) {
      if (value !== undefined && value !== null && value !== '') {
        params = params.set(key, String(value));
      }
    }

    return this.http
      .get<ApiResponse<PagedResult<SanPham>>>(this.baseUrl, { params })
      .pipe(map(res => res.data ?? { items: [], totalCount: 0, pageNumber: 1, pageSize: 20, totalPages: 0 }));
  }

  getSuggestions(tuKhoa: string, soLuong = 8): Observable<GoiYSanPham[]> {
    const params = new HttpParams().set('tuKhoa', tuKhoa).set('soLuong', soLuong);

    return this.http
      .get<ApiResponse<GoiYSanPham[]>>(`${this.baseUrl}/suggestions`, { params })
      .pipe(map(res => res.data ?? []));
  }

  getById(id: number): Observable<SanPham | null> {
    return this.http
      .get<ApiResponse<SanPham>>(`${this.baseUrl}/${id}`)
      .pipe(map(res => res.data));
  }

  getBanChay(id: number, soLuong = 4): Observable<SanPham[]> {
    return this.http
      .get<ApiResponse<SanPham[]>>(`${this.baseUrl}/${id}/ban-chay`, { params: { soLuong } })
      .pipe(map(res => res.data ?? []));
  }
}
