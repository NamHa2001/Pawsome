import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AutoOrderService } from '../dat-hang-tu-dong/services/auto-order.services';
import { Cart, CartItem, Coupon } from './models/gio-hang.model';
import { CartService } from './services/cart.service';
import { CouponService } from './services/coupon.service';
import { ChatAi } from '../../../shared/components/chat-ai/chat-ai';
import { Footer } from '../../../shared/components/footer/footer';
import { Header } from '../../../shared/components/header/header';

interface SanPhamGoiY {
  variantId: number;
  ten: string;
  hinhAnh: string;
  diemDanhGia: number;
  soLuotDanhGia: number;
  giaGoc: number;
  giaKhuyenMai: number;
}

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

  // Auto-order theo từng dòng (checkbox)
  readonly autoOrderIdTheoDong = signal<Record<number, number>>({});
  readonly dangXuLyAutoOrderDong = signal<number | null>(null);

  // Loại vận chuyển (chỉ ảnh hưởng hiển thị tạm tính ở FE)
  readonly loaiVanChuyen = signal<'free' | 'tracked'>('free');
  readonly phiVanChuyenCoTheoDoi = 6990;

  // Trạng thái đăng nhập — được cập nhật lại dựa trên kết quả gọi API layGioHang() (xem taiGioHang())
  readonly daDangNhap = signal(true);

  // PawVip (UI tĩnh, tính năng chưa có trong SRS)
  readonly tietKiemPawVip = computed(() => Math.round((this.gioHang()?.tienHang ?? 0) * 0.65));
  readonly tongTienPawVip = computed(() => (this.gioHang()?.tienHang ?? 0) - this.tietKiemPawVip());

  // Tính toán tổng tiền theo loại vận chuyển đã chọn
  readonly phiVanChuyenHienThi = computed(() =>
    this.loaiVanChuyen() === 'free' ? 0 : this.phiVanChuyenCoTheoDoi
  );

  readonly tongTienHienThi = computed(() => {
    const gh = this.gioHang();
    if (!gh) return 0;
    return gh.tienHang - gh.giamGia + this.phiVanChuyenHienThi();
  });

  readonly diemThuongDuKien = computed(() => Math.floor(this.tongTienHienThi() / 10000));

  readonly tabGoiY = signal<'thuong-mua' | 'lien-quan'>('thuong-mua');
  readonly dangThemGoiY = signal<number | null>(null);

  private readonly sanPhamThuongMua: SanPhamGoiY[] = [
    { variantId: 101, ten: 'Frontline Plus', hinhAnh: '/img/products/frontline-plus.jpg', diemDanhGia: 4.8, soLuotDanhGia: 214, giaGoc: 100960, giaKhuyenMai: 60580 },
    { variantId: 102, ten: 'Revolution Plus', hinhAnh: '/img/products/revolution-plus.jpg', diemDanhGia: 4.5, soLuotDanhGia: 132, giaGoc: 40640, giaKhuyenMai: 24340 },
    { variantId: 103, ten: 'Capstar', hinhAnh: '/img/products/capstar.jpg', diemDanhGia: 4.9, soLuotDanhGia: 98, giaGoc: 38650, giaKhuyenMai: 23190 },
    { variantId: 104, ten: 'Bravecto Spot-On', hinhAnh: '/img/products/bravecto.jpg', diemDanhGia: 4.9, soLuotDanhGia: 176, giaGoc: 38650, giaKhuyenMai: 23190 }
  ];
  private readonly sanPhamLienQuan: SanPhamGoiY[] = [];

  readonly sanPhamGoiYHienThi = computed(() =>
    this.tabGoiY() === 'thuong-mua' ? this.sanPhamThuongMua : this.sanPhamLienQuan
  );

  constructor() {
    this.taiGioHang();

    this.couponService.layDangHieuLuc().subscribe({
      next: ds => this.danhSachMaGiamGia.set(ds),
      error: () => this.danhSachMaGiamGia.set([]) 
    });
  }

  taiGioHang(): void {
    this.dangTai.set(true);
    this.loi.set(null);

    this.cartService.layGioHang().subscribe({
      next: res => {
        this.gioHang.set(res.data ?? null);
        this.daDangNhap.set(true);
        this.dangTai.set(false);
      },
      error: err => {
        if (err?.status === 401) {
          // Chưa đăng nhập thì hiển thị như giỏ hàng trống (guest), KHÔNG phải lỗi thật
          this.gioHang.set(null);
          this.daDangNhap.set(false);
          this.loi.set(null);
        } else {
          // Lỗi thật sự (server lỗi, mất mạng...) thì mới hiển thị thông báo lỗi
          this.loi.set('Could not load your cart. Please try again.');
        }
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
      next: res => this.gioHang.set(res.data ?? null),
      error: err => this.loi.set(err?.error?.message ?? 'Failed to update quantity.')
    });
  }

  xoaSanPham(cartItemId: number): void {
    this.cartService.xoaSanPham(cartItemId).subscribe({
      next: () => this.taiGioHang(),
      error: err => this.loi.set(err?.error?.message ?? 'Failed to remove item.')
    });
  }

  xoaSachGioHang(): void {
    if (!confirm('Clear the entire cart?')) return;

    this.cartService.xoaSachGioHang().subscribe({
      next: () => this.taiGioHang(),
      error: err => this.loi.set(err?.error?.message ?? 'Failed to clear cart.')
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
          noiDung: res.message ?? (res.success ? 'Coupon applied successfully' : 'Invalid coupon code')
        });
        if (res.success) {
          this.taiGioHang();
        }
      },
      error: err => {
        this.dangApDungMa.set(false);
        this.thongBaoCoupon.set({ loai: 'loi', noiDung: err?.error?.message ?? 'Failed to apply coupon.' });
      }
    });
  }

  xoaMaGiamGia(): void {
    this.cartService.xoaMaGiamGia().subscribe({
      next: res => {
        this.gioHang.set(res.data ?? null);
        this.maCoupon.set('');
        this.thongBaoCoupon.set(null);
      },
      error: err => this.loi.set(err?.error?.message ?? 'Failed to remove coupon.')
    });
  }

  chonMa(ma: string): void {
    this.maCoupon.set(ma);
  }

  // Auto-order theo dòng: checkbox bật/tắt trực tiếp
  toggleAutoOrderDong(item: CartItem): void {
    const idHienTai = this.autoOrderIdTheoDong()[item.cartItemId];
    this.dangXuLyAutoOrderDong.set(item.cartItemId);

    if (idHienTai) {
      this.autoOrderService.huy(idHienTai).subscribe({
        next: () => {
          this.autoOrderIdTheoDong.update(map => {
            const { [item.cartItemId]: _bo, ...con } = map;
            return con;
          });
          this.dangXuLyAutoOrderDong.set(null);
        },
        error: err => {
          this.loi.set(err?.error?.message ?? 'Failed to cancel recurring order.');
          this.dangXuLyAutoOrderDong.set(null);
        }
      });
    } else {
      this.autoOrderService.tao({ variantId: item.variantId, soLuong: item.soLuong, tanSuat: 'monthly' }).subscribe({
        next: res => {
          if (res.data) {
            this.autoOrderIdTheoDong.update(map => ({ ...map, [item.cartItemId]: res.data!.autoOrderId }));
          }
          this.dangXuLyAutoOrderDong.set(null);
        },
        error: err => {
          this.loi.set(err?.error?.message ?? 'Failed to set up recurring order.');
          this.dangXuLyAutoOrderDong.set(null);
        }
      });
    }
  }

  // Gợi ý sản phẩm
  themVaoGioTuGoiY(sp: SanPhamGoiY): void {
    this.dangThemGoiY.set(sp.variantId);

    this.cartService.themSanPham({ variantId: sp.variantId, soLuong: 1 }).subscribe({
      next: () => {
        this.dangThemGoiY.set(null);
        this.taiGioHang();
      },
      error: err => {
        this.dangThemGoiY.set(null);
        this.loi.set(err?.error?.message ?? 'Failed to add product to cart.');
      }
    });
  }

  kichHoatPawVip(): void {
    // TODO: cần nhóm + GVHD thống nhất mô hình subscription trước khi làm backend thật
    alert('PawVip Membership is coming soon!');
  }
}