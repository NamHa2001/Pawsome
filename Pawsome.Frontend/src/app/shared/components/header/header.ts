import { Component, HostListener, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-header',
  imports: [RouterLink, FormsModule],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class Header {
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

  timKiem(): void {
    const tuKhoa = this.tuKhoaTimKiem.trim();
    this.router.navigate(['/san-pham'], tuKhoa ? { queryParams: { tuKhoa } } : {});
  }
}
