import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../core/models/api-response.model';
import { AutoOrder, CreateAutoOrder, UpdateAutoOrder } from '../../gio-hang/models/gio-hang.model';

@Injectable({ providedIn: 'root' })
export class AutoOrderService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/dat-hang-tu-dong`;

  layDanhSach(): Observable<AutoOrder[]> {
    return this.http.get<ApiResponse<AutoOrder[]>>(this.baseUrl)
      .pipe(map(res => res.data ?? []));
  }

  tao(dto: CreateAutoOrder): Observable<ApiResponse<AutoOrder>> {
    return this.http.post<ApiResponse<AutoOrder>>(this.baseUrl, dto);
  }

  capNhat(id: number, dto: UpdateAutoOrder): Observable<ApiResponse<AutoOrder>> {
    return this.http.put<ApiResponse<AutoOrder>>(`${this.baseUrl}/${id}`, dto);
  }

  tamDung(id: number): Observable<ApiResponse<object>> {
    return this.http.patch<ApiResponse<object>>(`${this.baseUrl}/${id}/tam-dung`, {});
  }

  kichHoat(id: number): Observable<ApiResponse<object>> {
    return this.http.patch<ApiResponse<object>>(`${this.baseUrl}/${id}/kich-hoat`, {});
  }

  huy(id: number): Observable<ApiResponse<object>> {
    return this.http.delete<ApiResponse<object>>(`${this.baseUrl}/${id}`);
  }
}