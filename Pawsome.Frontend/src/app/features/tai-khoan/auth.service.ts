import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../core/models/api-response.model';
import { AuthUser } from '../../core/models/auth-user.model';
import { TokenService } from '../../core/models/token.service';
import { CartService } from '../gio-hang/gio-hang/services/cart.service';

export interface RegisterRequest {
  email: string;
  password: string;
  hoTen: string;
  soDienThoai?: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface ForgotPasswordResponse {
  resetToken: string;
}

export interface ResetPasswordRequest {
  token: string;
  otp: string;
  newPassword: string;
}

export interface GoogleLoginRequest {
  idToken: string;
}

export interface ChangePasswordRequest {
  matKhauCu: string;
  matKhauMoi: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly tokenService = inject(TokenService);
  private readonly cartService = inject(CartService);
  private readonly baseUrl = `${environment.apiUrl}/Auth`;

  register(dto: RegisterRequest): Observable<ApiResponse<AuthUser>> {
    return this.http.post<ApiResponse<AuthUser>>(`${this.baseUrl}/register`, dto)
      .pipe(tap(res => this.simpanNeuThanhCong(res)));
  }

  login(dto: LoginRequest): Observable<ApiResponse<AuthUser>> {
    return this.http.post<ApiResponse<AuthUser>>(`${this.baseUrl}/login`, dto)
      .pipe(tap(res => this.simpanNeuThanhCong(res)));
  }

  googleLogin(dto: GoogleLoginRequest): Observable<ApiResponse<AuthUser>> {
    return this.http.post<ApiResponse<AuthUser>>(`${this.baseUrl}/google-login`, dto)
      .pipe(tap(res => this.simpanNeuThanhCong(res)));
  }

  forgotPassword(dto: ForgotPasswordRequest): Observable<ApiResponse<ForgotPasswordResponse>> {
    return this.http.post<ApiResponse<ForgotPasswordResponse>>(`${this.baseUrl}/forgot-password`, dto);
  }

  resetPassword(dto: ResetPasswordRequest): Observable<ApiResponse<null>> {
    return this.http.post<ApiResponse<null>>(`${this.baseUrl}/reset-password`, dto);
  }

  changePassword(dto: ChangePasswordRequest): Observable<ApiResponse<null>> {
    return this.http.put<ApiResponse<null>>(`${this.baseUrl}/change-password`, dto);
  }

  logout(): void {
    this.cartService.resetSoLuong();
    this.tokenService.clear();
    this.http.post(`${this.baseUrl}/logout`, {}).subscribe({ error: () => { } });
  }

  private simpanNeuThanhCong(res: ApiResponse<AuthUser>): void {
    if (res.success && res.data) {
      this.tokenService.setUser(res.data);
    }
  }
}