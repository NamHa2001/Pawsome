import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../../../environments/environment';
import { ApiResponse } from '../../../../../core/models/api-response.model';
import { DashboardStats } from '../models/dashboard-stats.model';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/admin/dashboard`;

  layThongKe(): Observable<DashboardStats> {
    return this.http.get<ApiResponse<DashboardStats>>(this.baseUrl).pipe(
      map(res => res.data ?? { doanhThuThangNay: 0, soDonThangNay: 0, soDanhGiaChoDuyet: 0, sanPhamBanChay: [] })
    );
  }
}
