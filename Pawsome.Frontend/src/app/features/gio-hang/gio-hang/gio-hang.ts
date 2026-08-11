import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AutoOrderService } from '../dat-hang-tu-dong/services/auto-order.services';
import { Cart, Coupon, NHAN_TAN_SUAT, TanSuatDonTuDong } from './models/gio-hang.model';
import { CartService } from './services/cart.service';
import { CouponService } from './services/coupon.service';
import { ChatAi } from '../../../shared/components/chat-ai/chat-ai';
import { Footer } from '../../../shared/components/footer/footer';
import { Header } from '../../../shared/components/header/header';

@Component({
  selector: 'app-gio-hang',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, Header, Footer, ChatAi],
  templateUrl: './gio-hang.html',
  styleUrl: './gio-hang.css'
})
export class GioHangComponent {
  private readonly cartService = inject(CartService);
  private readonly couponService = inject(CouponService);
  private readonly autoOrderService = inject(AutoOrderService);

  readonly gioHang = signal<Cart | null>(null);
  readonly dangTai = signal(true);
  readonly loi = signal<string | null>(null);

  readonly maCoupon = signal('');
  readonly dangApDungMa = signal(false);
  readonly thongBaoCoupon = signal<{ loai: 'thanh-cong' | 'loi'; noiDung: string } | null>(null);
  readonly danhSachMaGiamGia = signal<Coupon[]>([]);

  // Dòng đang mở form "mua định kỳ"
  readonly cartItemIdDangMoAuto = signal<number | null>(null);
  readonly tanSuatDaChon = signal<TanSuatDonTuDong>('monthly');
  readonly nhanTanSuat = NHAN_TAN_SUAT;
  readonly tanSuatOptions: TanSuatDonTuDong[] = ['weekly', 'monthly', 'quarterly', 'yearly'];
  readonly dangGuiAutoOrder = signal(false);
  readonly thongBaoAutoOrder = signal<string | null>(null);

  constructor() {
    this.taiGioHang();
    this.couponService.layDangHieuLuc().subscribe(ds => this.danhSachMaGiamGia.set(ds));
  }

  taiGioHang(): void {
    this.dangTai.set(true);
    this.loi.set(null);

    this.cartService.layGioHang().subscribe({
      next: res => {
        this.gioHang.set(res.data);
        this.dangTai.set(false);
      },
      error: () => {
        this.loi.set('Không tải được giỏ hàng. Vui lòng thử lại.');
        this.dangTai.set(false);
      }
    });
  }

  tangSoLuong(cartItemId: number, soLuongHienTai: number): void {
    this.capNhatSoLuong(cartItemId, soLuongHienTai + 1);
  }

  giamSoLuong(cartItemId: number, soLuongHienTai: number): void {
    if (soLuongHienTai <= 1) return;
    this.capNhatSoLuong(cartItemId, soLuongHienTai - 1);
  }

  private capNhatSoLuong(cartItemId: number, soLuongMoi: number): void {
    this.cartService.capNhatSoLuong(cartItemId, { soLuong: soLuongMoi }).subscribe({
      next: res => this.gioHang.set(res.data),
      error: err => this.loi.set(err?.error?.message ?? 'Cập nhật số lượng thất bại.')
    });
  }

  xoaSanPham(cartItemId: number): void {
    this.cartService.xoaSanPham(cartItemId).subscribe({
      next: () => this.taiGioHang(),
      error: err => this.loi.set(err?.error?.message ?? 'Xóa sản phẩm thất bại.')
    });
  }

  xoaSachGioHang(): void {
    if (!confirm('Xóa toàn bộ giỏ hàng?')) return;

    this.cartService.xoaSachGioHang().subscribe({
      next: () => this.taiGioHang(),
      error: err => this.loi.set(err?.error?.message ?? 'Xóa giỏ hàng thất bại.')
    });
  }

  apDungMaGiamGia(): void {
    const ma = this.maCoupon().trim();
    if (!ma) return;

    this.dangApDungMa.set(true);
    this.thongBaoCoupon.set(null);

    this.cartService.apDungMaGiamGia({ maCode: ma }).subscribe({
      next: res => {
        this.dangApDungMa.set(false);
        this.thongBaoCoupon.set({
          loai: res.success ? 'thanh-cong' : 'loi',
          noiDung: res.message ?? (res.success ? 'Áp dụng mã thành công' : 'Mã không hợp lệ')
        });
        if (res.success) {
          this.taiGioHang();
        }
      },
      error: err => {
        this.dangApDungMa.set(false);
        this.thongBaoCoupon.set({ loai: 'loi', noiDung: err?.error?.message ?? 'Áp dụng mã thất bại.' });
      }
    });
  }

  chonMa(ma: string): void {
    this.maCoupon.set(ma);
  }

  moFormAutoOrder(cartItemId: number): void {
    this.cartItemIdDangMoAuto.set(this.cartItemIdDangMoAuto() === cartItemId ? null : cartItemId);
    this.thongBaoAutoOrder.set(null);
  }

  thietLapMuaDinhKy(variantId: number, soLuong: number): void {
    this.dangGuiAutoOrder.set(true);

    this.autoOrderService.tao({ variantId, soLuong, tanSuat: this.tanSuatDaChon() }).subscribe({
      next: res => {
        this.dangGuiAutoOrder.set(false);
        this.thongBaoAutoOrder.set(res.message ?? 'Đã thiết lập đơn đặt hàng tự động.');
        this.cartItemIdDangMoAuto.set(null);
      },
      error: err => {
        this.dangGuiAutoOrder.set(false);
        this.thongBaoAutoOrder.set(err?.error?.message ?? 'Thiết lập thất bại.');
      }
    });
  }
}