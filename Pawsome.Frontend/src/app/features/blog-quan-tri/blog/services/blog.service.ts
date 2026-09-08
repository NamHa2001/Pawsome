import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../core/models/api-response.model';
import { PagedResult } from '../../../../core/models/paged-result.model';
import { BlogFilterRequest, BlogPost, BlogPostRequest } from '../models/blog.model';

@Injectable({ providedIn: 'root' })
export class BlogService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/Blog`;

  search(filter: BlogFilterRequest): Observable<PagedResult<BlogPost>> {
    let params = new HttpParams();
    for (const [key, value] of Object.entries(filter)) {
      if (value !== undefined && value !== null && value !== '') {
        params = params.set(key, String(value));
      }
    }

    return this.http
      .get<ApiResponse<PagedResult<BlogPost>>>(this.baseUrl, { params })
      .pipe(map(res => res.data ?? { items: [], totalCount: 0, pageNumber: 1, pageSize: 9, totalPages: 0 }));
  }

  layChuDeList(): Observable<string[]> {
    return this.http
      .get<ApiResponse<string[]>>(`${this.baseUrl}/chu-de`)
      .pipe(map(res => res.data ?? []));
  }

  layChiTiet(id: number): Observable<BlogPost | null> {
    return this.http
      .get<ApiResponse<BlogPost>>(`${this.baseUrl}/${id}`)
      .pipe(map(res => res.data ?? null));
  }

  // ── Quản trị (Admin) ───────────────────────────────────────────────────
  taoBaiViet(dto: BlogPostRequest): Observable<ApiResponse<BlogPost>> {
    return this.http.post<ApiResponse<BlogPost>>(this.baseUrl, dto);
  }

  suaBaiViet(postId: number, dto: BlogPostRequest): Observable<ApiResponse<BlogPost>> {
    return this.http.put<ApiResponse<BlogPost>>(`${this.baseUrl}/${postId}`, dto);
  }

  xoaBaiViet(postId: number): Observable<ApiResponse<object>> {
    return this.http.delete<ApiResponse<object>>(`${this.baseUrl}/${postId}`);
  }

  uploadAnh(file: File): Observable<string> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http
      .post<ApiResponse<string>>(`${this.baseUrl}/upload-anh`, formData)
      .pipe(map(res => res.data!));
  }
}
