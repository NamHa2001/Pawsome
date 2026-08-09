import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { AuthUser } from './auth-user.model';

// Nơi lưu duy nhất token + thông tin đăng nhập, để interceptor/guard và trang đăng nhập
// (Phần 1) đều đọc/ghi cùng 1 chỗ, tránh mỗi nơi tự đặt tên key localStorage khác nhau.
// Dùng BehaviorSubject (đúng SRS mục 11.2 - "Quản lý trạng thái: RxJS Service/BehaviorSubject
// ... phiên đăng nhập") để các component khác (vd header) tự cập nhật ngay khi đăng nhập/đăng
// xuất, không cần tải lại trang.
const STORAGE_KEY = 'pawsome_auth';

@Injectable({ providedIn: 'root' })
export class TokenService {
  private readonly currentUserSubject = new BehaviorSubject<AuthUser | null>(this.readFromStorage());

  readonly currentUser$: Observable<AuthUser | null> = this.currentUserSubject.asObservable();

  getUser(): AuthUser | null {
    return this.currentUserSubject.value;
  }

  getToken(): string | null {
    return this.currentUserSubject.value?.token ?? null;
  }

  setUser(user: AuthUser): void {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(user));
    this.currentUserSubject.next(user);
  }

  clear(): void {
    localStorage.removeItem(STORAGE_KEY);
    this.currentUserSubject.next(null);
  }

  isLoggedIn(): boolean {
    return this.getToken() !== null;
  }

  hasRole(...roles: string[]): boolean {
    const user = this.getUser();
    return user !== null && roles.includes(user.role);
  }

  private readFromStorage(): AuthUser | null {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return null;

    try {
      return JSON.parse(raw) as AuthUser;
    } catch {
      return null;
    }
  }
}
