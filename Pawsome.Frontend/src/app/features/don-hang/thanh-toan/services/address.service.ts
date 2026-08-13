import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../core/models/api-response.model';
import { Address, CreateAddressRequest } from '../models/address.model';

@Injectable({ providedIn: 'root' })
export class AddressService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/Addresses`;

  getAll(): Observable<Address[]> {
    return this.http.get<ApiResponse<Address[]>>(this.baseUrl)
      .pipe(map(res => res.data ?? []));
  }

  create(dto: CreateAddressRequest): Observable<Address> {
    return this.http.post<ApiResponse<Address>>(this.baseUrl, dto)
      .pipe(map(res => res.data!));
  }
}