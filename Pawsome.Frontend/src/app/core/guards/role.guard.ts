import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { TokenService } from '../models/token.service';

// Dùng cho các route quan-tri/*: mỗi route tự khai báo vai trò được phép, vd:
// { path: 'san-pham', canActivate: [roleGuard('Admin')], ... }
// { path: 'kiem-duyet-danh-gia', canActivate: [roleGuard('Admin', 'Moderator')], ... }
export function roleGuard(...allowedRoles: string[]): CanActivateFn {
  return () => {
    const tokenService = inject(TokenService);
    const router = inject(Router);

    if (!tokenService.isLoggedIn()) {
      router.navigate(['/tai-khoan/dang-nhap']);
      return false;
    }

    if (tokenService.hasRole(...allowedRoles)) {
      return true;
    }

    router.navigate(['/']);
    return false;
  };
}
