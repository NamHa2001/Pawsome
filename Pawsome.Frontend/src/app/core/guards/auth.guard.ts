import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { TokenService } from '../models/token.service';

export const authGuard: CanActivateFn = () => {
  const tokenService = inject(TokenService);
  const router = inject(Router);

  if (tokenService.isLoggedIn()) {
    return true;
  }

  router.navigate(['/tai-khoan/dang-nhap']);
  return false;
};
