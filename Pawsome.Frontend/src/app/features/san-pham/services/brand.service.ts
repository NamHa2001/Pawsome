import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/api-response.model';
import { ThuongHieu } from '../models/san-pham.model';

@Injectable({ providedIn: 'root' })
export class BrandService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/Brands`;

  getAll(): Observable<ThuongHieu[]> {
    return this.http
      .get<ApiResponse<ThuongHieu[]>>(this.baseUrl)
      .pipe(map(res => res.data ?? []));
  }

  getById(id: number): Observable<ThuongHieu | null> {
    return this.http
      .get<ApiResponse<ThuongHieu>>(`${this.baseUrl}/${id}`)
      .pipe(map(res => res.data));
  }
}
