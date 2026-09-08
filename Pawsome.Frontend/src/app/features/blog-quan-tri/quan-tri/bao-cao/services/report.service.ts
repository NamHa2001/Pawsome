import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../../../environments/environment';
import { ApiResponse } from '../../../../../core/models/api-response.model';
import { PagedResult } from '../../../../../core/models/paged-result.model';
import {
  DoanhThuTheoDanhMuc,
  DoanhThuTheoKy,
  DoanhThuTheoThuongHieu,
  DonHangTheoTrangThai,
  LoaiXuatCsv,
  NhomTheoThoiGian,
  ReportDateFilter,
  SanPhamBanChay,
  SanPhamReportFilter
} from '../models/bao-cao.model';

@Injectable({ providedIn: 'root' })
export class ReportService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/admin/report`;

  private thamSoNgay(filter: ReportDateFilter): HttpParams {
    let params = new HttpParams();
    if (filter.tuNgay) params = params.set('tuNgay', filter.tuNgay);
    if (filter.denNgay) params = params.set('denNgay', filter.denNgay);
    return params;
  }

  private thamSoSanPham(filter: SanPhamReportFilter): HttpParams {
    let params = this.thamSoNgay(filter);
    if (filter.categoryId) params = params.set('categoryId', filter.categoryId);
    if (filter.brandId) params = params.set('brandId', filter.brandId);
    if (filter.sapXepTheo) params = params.set('sapXepTheo', filter.sapXepTheo);
    return params;
  }

  layDoanhThu(filter: ReportDateFilter, nhom: NhomTheoThoiGian): Observable<DoanhThuTheoKy[]> {
    const params = this.thamSoNgay(filter).set('nhom', nhom);
    return this.http
      .get<ApiResponse<DoanhThuTheoKy[]>>(`${this.baseUrl}/doanh-thu`, { params })
      .pipe(map(res => res.data ?? []));
  }

  layDonHangTheoTrangThai(filter: ReportDateFilter): Observable<DonHangTheoTrangThai[]> {
    return this.http
      .get<ApiResponse<DonHangTheoTrangThai[]>>(`${this.baseUrl}/don-hang-theo-trang-thai`, { params: this.thamSoNgay(filter) })
      .pipe(map(res => res.data ?? []));
  }

  laySanPham(filter: SanPhamReportFilter): Observable<PagedResult<SanPhamBanChay>> {
    const params = this.thamSoSanPham(filter)
      .set('page', filter.page ?? 1)
      .set('pageSize', filter.pageSize ?? 10);

    return this.http
      .get<ApiResponse<PagedResult<SanPhamBanChay>>>(`${this.baseUrl}/san-pham`, { params })
      .pipe(map(res => res.data ?? {
        items: [], totalCount: 0, pageNumber: 1, pageSize: filter.pageSize ?? 10, totalPages: 0
      }));
  }

  layDanhMuc(filter: ReportDateFilter): Observable<DoanhThuTheoDanhMuc[]> {
    return this.http
      .get<ApiResponse<DoanhThuTheoDanhMuc[]>>(`${this.baseUrl}/danh-muc`, { params: this.thamSoNgay(filter) })
      .pipe(map(res => res.data ?? []));
  }

  layThuongHieu(filter: ReportDateFilter): Observable<DoanhThuTheoThuongHieu[]> {
    return this.http
      .get<ApiResponse<DoanhThuTheoThuongHieu[]>>(`${this.baseUrl}/thuong-hieu`, { params: this.thamSoNgay(filter) })
      .pipe(map(res => res.data ?? []));
  }

  // Chưa có tiện ích tải file nào trong repo (xem plan) - tự tạo Blob từ response CSV rồi bấm
  // hộ 1 thẻ <a download> ẩn, cách chuẩn để tải file từ Angular không cần thư viện ngoài.
  xuatCsv(loai: LoaiXuatCsv, filter: SanPhamReportFilter, nhom: NhomTheoThoiGian): void {
    const params = this.thamSoSanPham(filter).set('loai', loai).set('nhom', nhom);

    this.http.get(`${this.baseUrl}/xuat-csv`, { params, responseType: 'blob' }).subscribe(blob => {
      const url = URL.createObjectURL(blob);
      const the = document.createElement('a');
      the.href = url;
      the.download = `bao-cao-${loai}.csv`;
      the.click();
      URL.revokeObjectURL(url);
    });
  }
}
