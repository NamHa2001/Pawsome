import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/api-response.model';
import { DanhMuc } from '../models/san-pham.model';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/Categories`;

  getAll(): Observable<DanhMuc[]> {
    return this.http
      .get<ApiResponse<DanhMuc[]>>(this.baseUrl)
      .pipe(map(res => res.data ?? []));
  }

  getById(id: number): Observable<DanhMuc | null> {
    return this.http
      .get<ApiResponse<DanhMuc>>(`${this.baseUrl}/${id}`)
      .pipe(map(res => res.data));
  }
}
