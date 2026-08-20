import { CurrencyPipe } from '@angular/common';
import { Component, HostListener, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Subject, distinctUntilChanged, of, switchMap } from 'rxjs';
import { TokenService } from '../../../core/models/token.service';
import { AuthService } from '../../../features/tai-khoan/auth.service';
import { ProductService } from '../../../features/san-pham/services/product.service';
import { GoiYSanPham, quyDoiUSD } from '../../../features/san-pham/models/san-pham.model';

@Component({
  selector: 'app-header',
  imports: [RouterLink, FormsModule, CurrencyPipe],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class Header {
  private readonly productService = inject(ProductService);
  private readonly tokenService = inject(TokenService);
  private readonly authService = inject(AuthService);

  readonly nguoiDungHienTai = toSignal(this.tokenService.currentUser$, {
    initialValue: this.tokenService.getUser()
  });
  readonly daDangNhap = computed(() => this.nguoiDungHienTai() !== null);

  readonly hienDropdownTaiKhoan = signal(false);
  readonly hienDropdownDangNhap = signal(false);

  readonly daCuonQua200 = signal(false);
  readonly danHeader = signal(false);
  readonly soLuongGioHang = signal(0);

  private viTriCuonTruoc = 0;
  tuKhoaTimKiem = '';

  protected readonly quyDoiUSD = quyDoiUSD;
  readonly danhSachGoiY = signal<GoiYSanPham[]>([]);
  readonly hienGoiY = signal(false);
  private readonly tuKhoaGoiY$ = new Subject<string>();

  constructor(private readonly router: Router) {
    const luu = localStorage.getItem('cartCount');
    this.soLuongGioHang.set(luu ? parseInt(luu, 10) : 0);

    // Gợi ý ngay trong lúc gõ, không chờ dừng gõ mới gọi API - switchMap tự hủy
    // request cũ mỗi khi có ký tự mới nên gõ nhanh không bị dồn/loạn kết quả.
    this.tuKhoaGoiY$
      .pipe(
        distinctUntilChanged(),
        switchMap(tuKhoa => (tuKhoa.length >= 2 ? this.productService.getSuggestions(tuKhoa) : of([])))
      )
      .subscribe(ds => this.danhSachGoiY.set(ds));
  }

  onTuKhoaThayDoi(giaTri: string): void {
    this.tuKhoaTimKiem = giaTri;
    const tuKhoa = giaTri.trim();
    this.hienGoiY.set(tuKhoa.length >= 2);
    this.tuKhoaGoiY$.next(tuKhoa);
  }

  chonGoiY(item: GoiYSanPham): void {
    this.hienGoiY.set(false);
    this.tuKhoaTimKiem = '';
    this.router.navigate(['/san-pham', item.productId]);
  }

  dongGoiY(): void {
    // Trễ 1 nhịp để kịp nhận sự kiện click vào item gợi ý trước khi ẩn danh sách.
    setTimeout(() => this.hienGoiY.set(false), 150);
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
      this.hienDropdownDangNhap.set(false);
    }
  }

  toggleDropdownTaiKhoan(event: MouseEvent): void {
    event.stopPropagation();
    this.hienDropdownTaiKhoan.update(v => !v);
  }

  dongDropdownTaiKhoan(): void {
    this.hienDropdownTaiKhoan.set(false);
  }

  toggleDropdownDangNhap(event: MouseEvent): void {
    event.stopPropagation();
    this.hienDropdownDangNhap.update(v => !v);
  }

  dongDropdownDangNhap(): void {
    this.hienDropdownDangNhap.set(false);
  }

  dangXuat(): void {
    this.authService.logout();
    this.hienDropdownTaiKhoan.set(false);
    this.router.navigate(['/']);
  }

  timKiem(): void {
    this.hienGoiY.set(false);
    const tuKhoa = this.tuKhoaTimKiem.trim();
    this.router.navigate(['/san-pham'], tuKhoa ? { queryParams: { tuKhoa } } : {});
  }
}
