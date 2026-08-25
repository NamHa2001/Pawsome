import { DecimalPipe } from '@angular/common';
import { Component, HostListener, computed, effect, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Subject, distinctUntilChanged, of, switchMap } from 'rxjs';
import { TokenService } from '../../../core/models/token.service';
import { AuthService } from '../../../features/tai-khoan/auth.service';
import { ProductService } from '../../../features/san-pham/services/product.service';
import { BrandService } from '../../../features/san-pham/services/brand.service';
import { GoiYSanPham, ThuongHieu } from '../../../features/san-pham/models/san-pham.model';
import { CartService } from '../../../features/gio-hang/gio-hang/services/cart.service';

@Component({
  selector: 'app-header',
  imports: [RouterLink, FormsModule, DecimalPipe],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class Header {
  private readonly productService = inject(ProductService);
  private readonly brandService = inject(BrandService);
  private readonly tokenService = inject(TokenService);
  private readonly authService = inject(AuthService);
  private readonly cartService = inject(CartService);

  // Menu "Brands" trước đây hardcode cứng tên + brandId, dễ bị lỗi thời khi danh sách brand thay
  // đổi (đã xảy ra thật) - giờ lấy trực tiếp từ danh mục thương hiệu thật.
  readonly danhSachThuongHieu = signal<ThuongHieu[]>([]);

  readonly nguoiDungHienTai = toSignal(this.tokenService.currentUser$, {
    initialValue: this.tokenService.getUser()
  });
  readonly daDangNhap = computed(() => this.nguoiDungHienTai() !== null);

  readonly hienDropdownTaiKhoan = signal(false);
  readonly hienDropdownDangNhap = signal(false);

  readonly daCuonQua200 = signal(false);
  readonly danHeader = signal(false);
  // Số lượng trên icon giỏ hàng lấy trực tiếp từ CartService (nguồn dữ liệu thật,
  // tự động đồng bộ khi thêm/sửa/xóa sản phẩm) thay vì đọc localStorage 1 lần lúc khởi tạo.
  readonly soLuongGioHang = this.cartService.soLuongGioHang;

  private viTriCuonTruoc = 0;
  tuKhoaTimKiem = '';

  readonly danhSachGoiY = signal<GoiYSanPham[]>([]);
  readonly hienGoiY = signal(false);
  private readonly tuKhoaGoiY$ = new Subject<string>();

  constructor(private readonly router: Router) {
    this.brandService.getAll().subscribe(ds => this.danhSachThuongHieu.set(ds));

    // Tải số lượng giỏ hàng thật từ server khi header khởi tạo (và mỗi khi
    // trạng thái đăng nhập thay đổi) để icon luôn khớp với giỏ hàng hiện có.
    effect(() => {
      if (this.daDangNhap()) {
        this.cartService.taiLaiSoLuong();
      }
    });

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