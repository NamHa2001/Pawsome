import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../core/models/api-response.model';

export interface Address {
  addressId: number;
  nguoiNhan: string;
  soDienThoai: string;
  diaChiChiTiet: string;
  phuongXa: string | null;
  quanHuyen: string | null;
  tinhThanh: string;
  laMacDinh: boolean;
}

export interface AddressFormValue {
  nguoiNhan: string;
  soDienThoai: string;
  diaChiChiTiet: string;
  phuongXa?: string;
  quanHuyen?: string;
  tinhThanh: string;
  laMacDinh: boolean;
}

@Injectable({ providedIn: 'root' })
export class AddressService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/Addresses`;

  getAll(): Observable<ApiResponse<Address[]>> {
    return this.http.get<ApiResponse<Address[]>>(this.baseUrl);
  }

  create(dto: AddressFormValue): Observable<ApiResponse<Address>> {
    return this.http.post<ApiResponse<Address>>(this.baseUrl, dto);
  }

  update(addressId: number, dto: AddressFormValue): Observable<ApiResponse<Address>> {
    return this.http.put<ApiResponse<Address>>(`${this.baseUrl}/${addressId}`, dto);
  }

  delete(addressId: number): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.baseUrl}/${addressId}`);
  }
}