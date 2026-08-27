import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { ApplyCouponResult, Cart, CartItem, Coupon, TanSuatDonTuDong } from './models/gio-hang.model';
import { CartService } from './services/cart.service';
import { CouponService } from './services/coupon.service';
import { AutoOrderService } from '../dat-hang-tu-dong/services/auto-order.services';
import { ChatAi } from '../../../shared/components/chat-ai/chat-ai';
import { Footer } from '../../../shared/components/footer/footer';
import { Header } from '../../../shared/components/header/header';
import { TokenService } from '../../../core/models/token.service';

type TabGoiY = 'thuong-mua' | 'lien-quan';

interface SanPhamGoiY {
  variantId: number;
  ten: string;
  hinhAnh: string;
  diemDanhGia: number;
  soLuotDanhGia: number;
  giaGoc: number;
  giaKhuyenMai: number;
}

interface ThongBaoCoupon {
  loai: 'thanh-cong' | 'loi';
  noiDung: string;
}

const PHI_VAN_CHUYEN_CO_THEO_DOI = 30000;
const PHAN_TRAM_GIAM_AUTO_ORDER = 0.1; 

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
  private readonly tokenService = inject(TokenService);

  private readonly nguoiDungHienTai = toSignal(this.tokenService.currentUser$, {
    initialValue: this.tokenService.getUser()
  });
  readonly daDangNhap = computed(() => this.nguoiDungHienTai() !== null);

  readonly gioHang = signal<Cart | null>(null);
  readonly dangTai = signal(true);
  readonly loi = signal<string | null>(null);

  readonly tabGoiY = signal<TabGoiY>('thuong-mua');
  readonly dangThemGoiY = signal<number | null>(null);

  readonly sanPhamThuongMua = signal<SanPhamGoiY[]>([]);
  readonly sanPhamLienQuan = signal<SanPhamGoiY[]>([]);

  readonly sanPhamGoiYHienThi = computed(() =>
    this.tabGoiY() === 'thuong-mua' ? this.sanPhamThuongMua() : this.sanPhamLienQuan()
  );

  readonly saoArr = [1, 2, 3, 4, 5];

  phanTramSaoGoiY(sao: number, diem: number): number {
    if (diem >= sao) return 100;
    if (diem <= sao - 1) return 0;
    return (diem - (sao - 1)) * 100;
  }

  readonly autoOrderIdTheoDong = signal<Record<number, number>>({});
  readonly dangXuLyAutoOrderDong = signal<number | null>(null);

  readonly giamGiaAutoOrder = computed(() => {
    const gh = this.gioHang();
    if (!gh) return 0;
    const cacDongDangBat = this.autoOrderIdTheoDong();
    return gh.items
      .filter(item => !!cacDongDangBat[item.cartItemId])
      .reduce((tong, item) => tong + item.thanhTien * PHAN_TRAM_GIAM_AUTO_ORDER, 0);
  });

  readonly maCoupon = signal('');
  readonly dangApDungMa = signal(false);
  readonly thongBaoCoupon = signal<ThongBaoCoupon | null>(null);
  readonly danhSachMaGiamGia = signal<Coupon[]>([]);

  readonly loaiVanChuyen = signal<'free' | 'tracked'>('free');
  readonly phiVanChuyenCoTheoDoi = PHI_VAN_CHUYEN_CO_THEO_DOI;

  readonly phiVanChuyenHienThi = computed(() =>
    this.loaiVanChuyen() === 'free' ? 0 : this.phiVanChuyenCoTheoDoi
  );

  readonly tongTienHienThi = computed(() => {
    const gh = this.gioHang();
    if (!gh) return 0;
    return Math.max(0, gh.tienHang - gh.giamGia - this.giamGiaAutoOrder() + this.phiVanChuyenHienThi());
  });

  readonly diemThuongDuKien = computed(() => Math.floor(this.tongTienHienThi() / 10000));

  constructor() {
    this.taiGioHang();
    this.taiMaGiamGiaGoiY();
  }

  taiGioHang(): void {
    this.dangTai.set(true);
    this.loi.set(null);
    this.cartService.layGioHang().subscribe({
      next: res => {
        this.gioHang.set(res.data ?? null);
        this.dangTai.set(false);
      },
      error: () => {
        this.loi.set('Could not load your cart. Please try again.');
        this.dangTai.set(false);
      }
    });
  }

  private taiMaGiamGiaGoiY(): void {
    this.couponService.layDangHieuLuc().subscribe({
      next: ds => this.danhSachMaGiamGia.set(ds),
      error: () => this.danhSachMaGiamGia.set([])
    });
  }

  themVaoGioTuGoiY(sp: SanPhamGoiY): void {
    if (this.dangThemGoiY() !== null) return;

    this.dangThemGoiY.set(sp.variantId);
    this.cartService.themSanPham({ variantId: sp.variantId, soLuong: 1 }).subscribe({
      next: res => {
        this.gioHang.set(res.data ?? this.gioHang());
        this.dangThemGoiY.set(null);
      },
      error: () => this.dangThemGoiY.set(null)
    });
  }

tangSoLuong(cartItemId: number, soLuongHienTai: number): void {
  const soLuongMoi = soLuongHienTai + 1;
  this.cartService.capNhatSoLuong(cartItemId, { soLuong: soLuongMoi }).subscribe({
    next: res => this.apDungKetQuaSuaSoLuong(cartItemId, soLuongMoi, res.data),
    error: () => {
      // Có thể xử lý lỗi nếu cần, ví dụ hiển thị thông báo
    }
  });
}

giamSoLuong(cartItemId: number, soLuongHienTai: number): void {
  if (soLuongHienTai <= 1) return;
  const soLuongMoi = soLuongHienTai - 1;
  this.cartService.capNhatSoLuong(cartItemId, { soLuong: soLuongMoi }).subscribe({
    next: res => this.apDungKetQuaSuaSoLuong(cartItemId, soLuongMoi, res.data),
    error: () => {
      // Có thể xử lý lỗi nếu cần
    }
  });
}

toggleAutoOrderDong(item: CartItem): void {
  const autoOrderIdHienTai = this.autoOrderIdTheoDong()[item.cartItemId];
  this.dangXuLyAutoOrderDong.set(item.cartItemId);

  if (autoOrderIdHienTai) {
    this.autoOrderService.huy(autoOrderIdHienTai).subscribe({
      next: () => {
        this.autoOrderIdTheoDong.update(cac => {
          const { [item.cartItemId]: _, ...con } = cac;
          return con;
        });
        this.dangXuLyAutoOrderDong.set(null);
      },
      error: () => this.dangXuLyAutoOrderDong.set(null)
    });
    return;
  }

  this.autoOrderService.tao({
    variantId: item.variantId,
    soLuong: item.soLuong,
    tanSuat: 'monthly' as TanSuatDonTuDong
  }).subscribe({
    next: res => {
      const autoOrderId = res.data?.autoOrderId;
      if (autoOrderId) {
        this.autoOrderIdTheoDong.update(cac => ({ ...cac, [item.cartItemId]: autoOrderId }));
      }
      this.dangXuLyAutoOrderDong.set(null);
    },
    error: () => this.dangXuLyAutoOrderDong.set(null)
  });
}

xoaSanPham(cartItemId: number): void {
  this.cartService.xoaSanPham(cartItemId).subscribe({
    next: () => {
      this.gioHang.update(gh => {
        if (!gh) return gh;
        const items = gh.items.filter(item => item.cartItemId !== cartItemId);
        const tienHang = items.reduce((tong, i) => tong + i.thanhTien, 0);
        return {
          ...gh,
          items,
          tienHang,
          tongTien: Math.max(0, tienHang - gh.giamGia + gh.phiVanChuyenTamTinh)
        };
      });
    },
    error: () => {
      // Có thể xử lý lỗi nếu cần
    }
  });
}

xoaSachGioHang(): void {
  if (!confirm('Remove all items from your cart?')) return;

  this.cartService.xoaSachGioHang().subscribe({
    next: () => {
      this.gioHang.update(gh => gh ? { ...gh, items: [], tienHang: 0, giamGia: 0, tongTien: 0, maCouponDangApDung: null } : gh);
      this.maCoupon.set('');
      this.thongBaoCoupon.set(null);
    },
    error: () => {
      // Có thể xử lý lỗi nếu cần
    }
  });
}

apDungMaGiamGia(): void {
  const ma = this.maCoupon().trim();
  if (!ma) {
    this.thongBaoCoupon.set({ loai: 'loi', noiDung: 'Please enter a coupon code.' });
    return;
  }

  this.dangApDungMa.set(true);
  this.cartService.apDungMaGiamGia({ maCode: ma }).subscribe({
    next: res => {
      this.dangApDungMa.set(false);
      const ketQua = res.data;
      if (!ketQua || !ketQua.hopLe) {
        this.thongBaoCoupon.set({ loai: 'loi', noiDung: ketQua?.thongBao ?? 'Invalid coupon code.' });
        return;
      }
      this.apDungKetQuaCoupon(ma, ketQua);
      this.thongBaoCoupon.set({ loai: 'thanh-cong', noiDung: ketQua.thongBao ?? `Coupon "${ma}" applied.` });
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
      this.gioHang.set(res.data ?? this.gioHang());
      this.maCoupon.set('');
      this.thongBaoCoupon.set(null);
    },
    error: err => {
      this.thongBaoCoupon.set({ loai: 'loi', noiDung: err?.error?.message ?? 'Failed to remove coupon.' });
    }
  });
}

chonMa(maCode: string): void {
  this.maCoupon.set(maCode);
  this.apDungMaGiamGia();
}

private apDungKetQuaCoupon(maCode: string, ketQua: ApplyCouponResult): void {
  this.gioHang.update(gh => {
    if (!gh) return gh;
    return {
      ...gh,
      maCouponDangApDung: maCode,
      tienHang: ketQua.tienHang,
      giamGia: ketQua.giamGia,
      phiVanChuyenTamTinh: ketQua.phiVanChuyenTamTinh,
      tongTien: ketQua.tongTien
    };
  });
}

private apDungKetQuaSuaSoLuong(cartItemId: number, soLuongMoi: number, cartTuServer: Cart | null | undefined): void {
  const ghHienTai = this.gioHang();

  if (cartTuServer && ghHienTai && cartTuServer.items.length === ghHienTai.items.length) {
    this.gioHang.set(cartTuServer);
    return;
  }

  this.gioHang.update(gh => {
    if (!gh) return gh;

    const items = gh.items.map(item =>
      item.cartItemId === cartItemId
        ? { ...item, soLuong: soLuongMoi, thanhTien: item.donGia * soLuongMoi }
        : item
    );
    const tienHang = items.reduce((tong, i) => tong + i.thanhTien, 0);

    return {
      ...gh,
      items,
      tienHang,
      tongTien: Math.max(0, tienHang - gh.giamGia + gh.phiVanChuyenTamTinh)
    };
  });
}
}