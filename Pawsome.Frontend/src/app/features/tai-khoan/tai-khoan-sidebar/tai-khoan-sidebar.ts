import { DecimalPipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../auth.service';
import { UserService } from '../user.service';
import { VND_PER_PAWPOINT } from '../../don-hang/pawpoints/models/pawpoints.model';

// Sidebar dùng chung cho các trang trong khu tài khoản (Manage Account, Dashboard...) - trước
// đây markup này chỉ tồn tại lặp lại bên trong ho-so.html, tách ra đây để trang Dashboard mới
// không phải copy lại, và mục "Dashboard" giờ trỏ route thật thay vì disabled "Coming soon".
@Component({
  selector: 'app-tai-khoan-sidebar',
  standalone: true,
  imports: [DecimalPipe, RouterLink, RouterLinkActive],
  templateUrl: './tai-khoan-sidebar.html',
  styleUrl: './tai-khoan-sidebar.scss'
})
export class TaiKhoanSidebarComponent {
  private readonly userService = inject(UserService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  private readonly nguongDoiThuong = 500;
  readonly diemPawpoints = signal(0);

  constructor() {
    this.userService.getProfile().subscribe({
      next: res => this.diemPawpoints.set(res.data?.diemPawpoints ?? 0),
      error: () => {}
    });
  }

  get diemConThieu(): number {
    return Math.max(this.nguongDoiThuong - this.diemPawpoints(), 0);
  }

  // Giá trị thật của mốc thưởng theo đúng tỉ lệ quy đổi PawPoints hiện hành.
  get soTienThuong(): number {
    return this.nguongDoiThuong * VND_PER_PAWPOINT;
  }

  dangXuat(): void {
    this.authService.logout();
    this.router.navigate(['/']);
  }
}
