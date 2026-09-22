import { DecimalPipe } from '@angular/common';
import { Component, DestroyRef, HostListener, computed, effect, inject, signal } from '@angular/core';
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

  readonly danhSachThuongHieu = signal<ThuongHieu[]>([]);

  readonly nguoiDungHienTai = toSignal(this.tokenService.currentUser$, {
    initialValue: this.tokenService.getUser()
  });
  readonly daDangNhap = computed(() => this.nguoiDungHienTai() !== null);

  readonly hienDropdownTaiKhoan = signal(false);
  readonly hienDropdownDangNhap = signal(false);

  // Mobile: "Shop by Pet"/"Shop by Condition"/"Brands" trước đây bấm là điều hướng thẳng, không xổ
  // menu con ra được (menu con .KhungMenuCon chỉ hiện qua :hover - không có trên cảm ứng). Giờ bấm
  // để xổ/đóng menu con (giống các dropdown khác trong header), chỉ 1 menu mở tại 1 thời điểm -
  // lưu index (0/1/2) của .MucMenuChinh đang mở, null = không mở cái nào.
  readonly mucMenuMoTrenMobile = signal<number | null>(null);

  // routerLink là directive riêng, tự lắng nghe click và tự điều hướng qua Router API - gọi
  // event.preventDefault()/stopPropagation() trong (click) handler của mình KHÔNG chặn được nó (2
  // listener độc lập trên cùng 1 phần tử). Phải tắt hẳn routerLink trên mobile ([routerLink]="null"
  // = Angular tự hiểu là không điều hướng) thay vì cố chặn sau khi nó đã tự gọi navigate().
  private readonly mqMobileNav = window.matchMedia('(max-width: 900px)');
  readonly laMobileNav = signal(this.mqMobileNav.matches);

  readonly daCuonQua200 = signal(false);
  readonly danHeader = signal(false);

  // Tự động nhận giá trị items.length từ CartService
  readonly soLuongGioHang = this.cartService.soLuongGioHang;

  private viTriCuonTruoc = 0;
  tuKhoaTimKiem = '';

  readonly danhSachGoiY = signal<GoiYSanPham[]>([]);
  readonly hienGoiY = signal(false);
  private readonly tuKhoaGoiY$ = new Subject<string>();

  constructor(private readonly router: Router) {
    // Header bị tạo/hủy lại theo mỗi lần đổi trang (mỗi trang tự khai <app-header/> riêng, không
    // nằm ngoài router-outlet) - phải tự gỡ listener khi hủy, không thì mỗi lần đổi trang lại cộng
    // dồn thêm 1 listener resize không bao giờ được gỡ.
    const capNhatMobileNav = (e: MediaQueryListEvent) => this.laMobileNav.set(e.matches);
    this.mqMobileNav.addEventListener('change', capNhatMobileNav);
    inject(DestroyRef).onDestroy(() => this.mqMobileNav.removeEventListener('change', capNhatMobileNav));

    this.brandService.getAll().subscribe(ds => this.danhSachThuongHieu.set(ds));

    effect(() => {
      if (this.daDangNhap()) {
        this.cartService.taiLaiSoLuong();
      } else {
        // Lớp bảo hiểm cho các nơi tự clear token mà không qua AuthService.logout() (vd
        // interceptor 401 tự đăng xuất) - badge giỏ hàng vẫn phải về 0 khi hết đăng nhập.
        this.cartService.resetSoLuong();
      }
    });

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
    if (!target.closest('.MucMenuChinh')) {
      this.mucMenuMoTrenMobile.set(null);
    }
  }

  // Chỉ chặn điều hướng + xổ menu con trên mobile (<=900px, đúng mốc menu chuyển sang dạng cuộn
  // ngang trong header.scss) - ở desktop vẫn bấm là đi thẳng trang như cũ, menu con vẫn hiện qua
  // :hover như cũ, không đổi hành vi.
  toggleMenuConMobile(index: number, event: MouseEvent): void {
    if (!window.matchMedia('(max-width: 900px)').matches) return;

    event.preventDefault();
    event.stopPropagation();
    this.mucMenuMoTrenMobile.update(v => (v === index ? null : index));
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