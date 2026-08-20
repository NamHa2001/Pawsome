import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../core/models/api-response.model';
import { PagedResult } from '../../../../core/models/paged-result.model';
import { PawPointsBalance, PawPointsHistoryFilter, PawPointsTransaction } from '../models/pawpoints.model';

@Injectable({ providedIn: 'root' })
export class PawPointsService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/PawPoints`;

  getBalance(): Observable<PawPointsBalance> {
    return this.http
      .get<ApiResponse<PawPointsBalance>>(`${this.baseUrl}/balance`)
      .pipe(map(res => res.data!));
  }

  getHistory(filter: PawPointsHistoryFilter): Observable<PagedResult<PawPointsTransaction>> {
    let params = new HttpParams()
      .set('page', filter.page ?? 1)
      .set('pageSize', filter.pageSize ?? 10);

    if (filter.loai) {
      params = params.set('loai', filter.loai);
    }

    return this.http
      .get<ApiResponse<PagedResult<PawPointsTransaction>>>(`${this.baseUrl}/history`, { params })
      .pipe(
        map(
          res =>
            res.data ?? {
              items: [],
              totalCount: 0,
              pageNumber: 1,
              pageSize: filter.pageSize ?? 10,
              totalPages: 0
            }
        )
      );
  }
}