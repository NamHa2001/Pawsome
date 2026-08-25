import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { Cart, CartItem, Coupon, TanSuatDonTuDong } from './models/gio-hang.model';
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

const PHAN_TRAM_GIAM_PAWVIP = 0.2;
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

  //Gợi ý sản phẩm khi giỏ hàng trống
  readonly tabGoiY = signal<TabGoiY>('thuong-mua');
  readonly dangThemGoiY = signal<number | null>(null);
  // TODO: nạp dữ liệu thật từ API gợi ý sản phẩm (thuộc Part sản phẩm) khi có.
  private readonly sanPhamThuongMua = signal<SanPhamGoiY[]>([]);
  private readonly sanPhamLienQuan = signal<SanPhamGoiY[]>([]);

  readonly sanPhamGoiYHienThi = computed(() =>
    this.tabGoiY() === 'thuong-mua' ? this.sanPhamThuongMua() : this.sanPhamLienQuan()
  );

  // PawVip banner
  readonly tietKiemPawVip = computed(() => (this.gioHang()?.tienHang ?? 0) * PHAN_TRAM_GIAM_PAWVIP);
  readonly tongTienPawVip = computed(() => (this.gioHang()?.tienHang ?? 0) - this.tietKiemPawVip());

  // Auto Order theo từng dòng sản phẩm 
  readonly autoOrderIdTheoDong = signal<Record<number, number>>({});
  readonly dangXuLyAutoOrderDong = signal<number | null>(null);

  // Tổng tiền được giảm 10% từ các dòng sản phẩm đang bật Auto Order.
  // Tính trên thanhTien (đơn giá x số lượng) của từng dòng đang bật.
  readonly giamGiaAutoOrder = computed(() => {
    const gh = this.gioHang();
    if (!gh) return 0;

    const cacDongDangBat = this.autoOrderIdTheoDong();
    return gh.items
      .filter(item => !!cacDongDangBat[item.cartItemId])
      .reduce((tong, item) => tong + item.thanhTien * PHAN_TRAM_GIAM_AUTO_ORDER, 0);
  });

  //  Mã giảm giá 
  readonly maCoupon = signal('');
  readonly dangApDungMa = signal(false);
  readonly thongBaoCoupon = signal<ThongBaoCoupon | null>(null);
  readonly danhSachMaGiamGia = signal<Coupon[]>([]);

  // Vận chuyển 
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

  //  Tải dữ liệu 
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

  //  Gợi ý sản phẩm 
  themVaoGioTuGoiY(sp: SanPhamGoiY): void {
    this.dangThemGoiY.set(sp.variantId);

    this.cartService.themSanPham({ variantId: sp.variantId, soLuong: 1 }).subscribe({
      next: res => {
        this.gioHang.set(res.data ?? this.gioHang());
        this.dangThemGoiY.set(null);
      },
      error: () => this.dangThemGoiY.set(null)
    });
  }

  //  PawVip 
  kichHoatPawVip(): void {
   
  }

  //  Số lượng / xoá sản phẩm 
  tangSoLuong(cartItemId: number, soLuongHienTai: number): void {
    this.cartService.capNhatSoLuong(cartItemId, { soLuong: soLuongHienTai + 1 }).subscribe({
      next: res => this.gioHang.set(res.data ?? this.gioHang())
    });
  }

  giamSoLuong(cartItemId: number, soLuongHienTai: number): void {
    if (soLuongHienTai <= 1) return;

    this.cartService.capNhatSoLuong(cartItemId, { soLuong: soLuongHienTai - 1 }).subscribe({
      next: res => this.gioHang.set(res.data ?? this.gioHang())
    });
  }

  xoaSanPham(cartItemId: number): void {
    this.cartService.xoaSanPham(cartItemId).subscribe({
      next: () => {
        this.gioHang.update(gh => gh
          ? { ...gh, items: gh.items.filter(i => i.cartItemId !== cartItemId) }
          : gh);
        this.taiGioHang();
      }
    });
  }

  xoaSachGioHang(): void {
    if (!confirm('Remove all items from your cart?')) return;

    this.cartService.xoaSachGioHang().subscribe({
      next: () => this.gioHang.update(gh => gh ? { ...gh, items: [] } : gh)
    });
  }

  //  Auto Order theo dòng
  toggleAutoOrderDong(item: CartItem): void {
    this.dangXuLyAutoOrderDong.set(item.cartItemId);
    const autoOrderIdHienTai = this.autoOrderIdTheoDong()[item.cartItemId];

    if (autoOrderIdHienTai) {
      this.autoOrderService.huy(autoOrderIdHienTai).subscribe({
        next: () => {
          this.autoOrderIdTheoDong.update(map => {
            const { [item.cartItemId]: _, ...rest } = map;
            return rest;
          });
          this.dangXuLyAutoOrderDong.set(null);
        },
        error: () => this.dangXuLyAutoOrderDong.set(null)
      });
      return;
    }

    const tanSuatMacDinh: TanSuatDonTuDong = 'monthly';

    this.autoOrderService.tao({
      variantId: item.variantId,
      soLuong: item.soLuong,
      tanSuat: tanSuatMacDinh
    }).subscribe({
      next: res => {
        if (res.data) {
          this.autoOrderIdTheoDong.update(map => ({ ...map, [item.cartItemId]: res.data!.autoOrderId }));
        }
        this.dangXuLyAutoOrderDong.set(null);
      },
      error: () => this.dangXuLyAutoOrderDong.set(null)
    });
  }

  //  Mã giảm giá 
  apDungMaGiamGia(): void {
    const ma = this.maCoupon().trim();
    if (!ma) return;

    this.dangApDungMa.set(true);

    this.cartService.apDungMaGiamGia({ maCode: ma }).subscribe({
      next: res => {
        this.dangApDungMa.set(false);
        const ketQua = res.data;

        if (!ketQua || !ketQua.hopLe) {
          this.thongBaoCoupon.set({ loai: 'loi', noiDung: ketQua?.thongBao ?? 'Invalid coupon code.' });
          return;
        }

        this.gioHang.update(gh => gh ? {
          ...gh,
          maCouponDangApDung: ma,
          tienHang: ketQua.tienHang,
          giamGia: ketQua.giamGia,
          phiVanChuyenTamTinh: ketQua.phiVanChuyenTamTinh,
          tongTien: ketQua.tongTien
        } : gh);

        this.thongBaoCoupon.set({ loai: 'thanh-cong', noiDung: ketQua.thongBao ?? 'Coupon applied successfully.' });
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
      }
    });
  }

  chonMa(maCode: string): void {
    this.maCoupon.set(maCode);
    this.apDungMaGiamGia();
  }
}