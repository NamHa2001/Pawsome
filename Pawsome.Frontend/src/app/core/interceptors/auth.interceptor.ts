import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { TokenService } from '../models/token.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const tokenService = inject(TokenService);
  const router = inject(Router);
  const token = tokenService.getToken();

  const cloned = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(cloned).pipe(
    catchError((err: unknown) => {
      // TokenService.isLoggedIn() chỉ kiểm tra "có token" chứ không tự kiểm tra token còn hạn hay
      // không - token hết hạn/không hợp lệ chỉ lộ ra khi server trả 401 (do JWT ValidateLifetime ở
      // Program.cs). Tự đăng xuất + đưa về trang đăng nhập ngay tại đây (dùng chung cho mọi request
      // của cả 5 Phần) thay vì để lỗi rơi xuống từng nơi gọi API rồi hiện thông báo chung chung khó
      // hiểu như "Failed to add to cart". Chỉ xử lý khi trước đó THẬT SỰ đang có token (tránh nhầm
      // với 401 của route công khai không cần đăng nhập, dù hiện tại chưa route nào như vậy).
      if (err instanceof HttpErrorResponse && err.status === 401 && tokenService.isLoggedIn()) {
        tokenService.clear();
        router.navigate(['/tai-khoan/dang-nhap']);
      }
      return throwError(() => err);
    })
  );
};
