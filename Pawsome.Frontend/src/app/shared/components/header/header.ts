import { Component, HostListener, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { TokenService } from '../../../core/models/token.service';
import { AuthService } from '../../../features/tai-khoan/auth.service';

@Component({
  selector: 'app-header',
  imports: [RouterLink, FormsModule],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class Header {
  private readonly tokenService = inject(TokenService);
  private readonly authService = inject(AuthService);

  readonly nguoiDungHienTai = toSignal(this.tokenService.currentUser$, {
    initialValue: this.tokenService.getUser()
  });
  readonly daDangNhap = computed(() => this.nguoiDungHienTai() !== null);

  readonly hienDropdownTaiKhoan = signal(false);

  readonly daCuonQua200 = signal(false);
  readonly danHeader = signal(false);
  readonly soLuongGioHang = signal(0);

  private viTriCuonTruoc = 0;
  tuKhoaTimKiem = '';

  constructor(private readonly router: Router) {
    const luu = localStorage.getItem('cartCount');
    this.soLuongGioHang.set(luu ? parseInt(luu, 10) : 0);
  }

  @HostListener('window:scroll')
  onScroll(): void {
    const viTriHienTai = window.pageYOffset;

    if (viTriHienTai > 200) {
      this.danHeader.set(this.viTriCuonTruoc <= viTriHienTai);
    } else {
      this.danHeader.set(false);
    }

    this.viTriCuonTruoc = viTriHienTai;
  }

  @HostListener('document:click', ['$event'])
  onClickNgoaiDropdown(event: MouseEvent): void {
    const target = event.target as HTMLElement;
    if (!target.closest('.KhungTaiKhoan')) {
      this.hienDropdownTaiKhoan.set(false);
    }
  }

  toggleDropdownTaiKhoan(event: MouseEvent): void {
    event.stopPropagation();
    this.hienDropdownTaiKhoan.update(v => !v);
  }

  dongDropdownTaiKhoan(): void {
    this.hienDropdownTaiKhoan.set(false);
  }

  dangXuat(): void {
    this.authService.logout();
    this.hienDropdownTaiKhoan.set(false);
    this.router.navigate(['/']);
  }

  timKiem(): void {
    const tuKhoa = this.tuKhoaTimKiem.trim();
    this.router.navigate(['/san-pham'], tuKhoa ? { queryParams: { tuKhoa } } : {});
  }
}