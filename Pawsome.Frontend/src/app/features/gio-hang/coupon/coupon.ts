import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { ChatAi } from '../../../shared/components/chat-ai/chat-ai';
import { Footer } from '../../../shared/components/footer/footer';
import { Header } from '../../../shared/components/header/header';
import { Coupon } from '../gio-hang/models/gio-hang.model';
import { CouponService } from '../gio-hang/services/coupon.service';

@Component({
  selector: 'app-coupon',
  standalone: true,
  imports: [CommonModule, Header, Footer, ChatAi],
  templateUrl: './coupon.html',
  styleUrl: './coupon.css'
})
export class CouponComponent {
  private readonly couponService = inject(CouponService);
  private readonly router = inject(Router);

  readonly danhSach = signal<Coupon[]>([]);
  readonly dangTai = signal(true);
  readonly loi = signal<string | null>(null);
  readonly maVuaSaoChep = signal<string | null>(null);

  constructor() {
    this.taiDanhSach();
  }

  private taiDanhSach(): void {
    this.dangTai.set(true);
    this.loi.set(null);

    this.couponService.layDangHieuLuc().subscribe({
      next: ds => {
        this.danhSach.set(ds);
        this.dangTai.set(false);
      },
      error: () => {
        this.loi.set('Could not load coupons. Please try again.');
        this.dangTai.set(false);
      }
    });
  }

  hienThiUuDai(cp: Coupon): string {
    return cp.loaiGiam === 'percent'
      ? `${cp.giaTri}% OFF`
      : `${cp.giaTri.toLocaleString('en-US')}đ OFF`;
  }

  saoChepMa(maCode: string): void {
    navigator.clipboard.writeText(maCode).then(() => {
      this.maVuaSaoChep.set(maCode);
      setTimeout(() => {
        if (this.maVuaSaoChep() === maCode) this.maVuaSaoChep.set(null);
      }, 2000);
    });
  }

  // Sang giỏ hàng kèm mã qua query param - GioHangComponent tự đọc & áp mã ngay khi vào trang
  // (xem gio-hang.ts constructor), giống hành vi "Use Now" ở trang voucher Shopee.
  dungNgay(maCode: string): void {
    this.router.navigate(['/gio-hang'], { queryParams: { ma: maCode } });
  }
}
