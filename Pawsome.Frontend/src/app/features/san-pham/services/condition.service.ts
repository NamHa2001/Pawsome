import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/api-response.model';
import { TinhTrangSucKhoe } from '../models/san-pham.model';

@Injectable({ providedIn: 'root' })
export class ConditionService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/Conditions`;

  getAll(): Observable<TinhTrangSucKhoe[]> {
    return this.http
      .get<ApiResponse<TinhTrangSucKhoe[]>>(this.baseUrl)
      .pipe(map(res => res.data ?? []));
  }
}
