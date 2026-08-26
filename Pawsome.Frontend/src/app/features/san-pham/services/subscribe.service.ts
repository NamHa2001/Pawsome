import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/api-response.model';

export interface SubscribeRequest {
  email: string;
  recaptchaToken: string;
}

@Injectable({ providedIn: 'root' })
export class SubscribeService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/Subscribe`;

  subscribe(req: SubscribeRequest): Observable<ApiResponse<{ maCode: string }>> {
    return this.http.post<ApiResponse<{ maCode: string }>>(this.baseUrl, req);
  }
}
