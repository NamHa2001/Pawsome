import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../core/models/api-response.model';
import { TokenService } from '../../core/models/token.service';

export interface UserProfile {
  userId: number;
  email: string;
  hoTen: string;
  soDienThoai: string | null;
  diemPawpoints: number;
  pawVipTier: string | null;
  role: string;
  ngayTao: string;
}

export interface UpdateProfileRequest {
  hoTen: string;
  soDienThoai?: string;
}

@Injectable({ providedIn: 'root' })
export class UserService {
  private readonly http = inject(HttpClient);
  private readonly tokenService = inject(TokenService);
  private readonly baseUrl = `${environment.apiUrl}/Users`;

  getProfile(): Observable<ApiResponse<UserProfile>> {
    return this.http.get<ApiResponse<UserProfile>>(`${this.baseUrl}/me`);
  }

  updateProfile(dto: UpdateProfileRequest): Observable<ApiResponse<UserProfile>> {
    return this.http.put<ApiResponse<UserProfile>>(`${this.baseUrl}/me`, dto)
      .pipe(tap(res => this.dongBoTenVaoTokenService(res)));
  }

  private dongBoTenVaoTokenService(res: ApiResponse<UserProfile>): void {
    if (!res.success || !res.data) return;
    const nguoiDungHienTai = this.tokenService.getUser();
    if (!nguoiDungHienTai) return;

    this.tokenService.setUser({ ...nguoiDungHienTai, hoTen: res.data.hoTen });
  }
}