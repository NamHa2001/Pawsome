import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/api-response.model';
import { PagedResult } from '../../../core/models/paged-result.model';
import { DanhGia, GuiDanhGia } from '../models/san-pham.model';

@Injectable({ providedIn: 'root' })
export class ReviewService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/Reviews`;

  getByProduct(productId: number, page = 1, pageSize = 20): Observable<PagedResult<DanhGia>> {
    return this.http
      .get<ApiResponse<PagedResult<DanhGia>>>(`${this.baseUrl}/product/${productId}`, {
        params: { page, pageSize }
      })
      .pipe(map(res => res.data ?? { items: [], totalCount: 0, pageNumber: 1, pageSize: 20, totalPages: 0 }));
  }

  getNoiBat(soLuong = 6): Observable<DanhGia[]> {
    return this.http
      .get<ApiResponse<DanhGia[]>>(`${this.baseUrl}/noi-bat`, { params: { soLuong } })
      .pipe(map(res => res.data ?? []));
  }

  // "Verified purchase" - chỉ khách đã mua VÀ đã nhận sản phẩm này (đơn ở trạng thái da_giao)
  // mới được gửi đánh giá. Dùng để quyết định hiện form gửi đánh giá hay hiện thông báo cần mua
  // trước - backend (ReviewService.CreateAsync) vẫn tự kiểm tra lại, không chỉ tin phía client.
  coTheDanhGia(productId: number): Observable<boolean> {
    return this.http
      .get<ApiResponse<boolean>>(`${this.baseUrl}/product/${productId}/co-the-danh-gia`)
      .pipe(map(res => res.data ?? false));
  }

  create(dto: GuiDanhGia): Observable<DanhGia> {
    return this.http
      .post<ApiResponse<DanhGia>>(this.baseUrl, dto)
      .pipe(map(res => res.data as DanhGia));
  }

  // Bấm lại đúng lựa chọn cũ thì gỡ vote, bấm lựa chọn khác thì đổi vote - xử lý thật ở backend
  // (ReviewService.VoteAsync), không phải chỉ tăng số đếm ở client.
  vote(reviewId: number, huuIch: boolean): Observable<DanhGia> {
    return this.http
      .post<ApiResponse<DanhGia>>(`${this.baseUrl}/${reviewId}/vote`, { huuIch })
      .pipe(map(res => res.data as DanhGia));
  }
}
