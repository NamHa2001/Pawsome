import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { Cart, CartItem, TanSuatDonTuDong } from './models/gio-hang.model';
import { CartService } from './services/cart.service';
import { AutoOrderService } from '../dat-hang-tu-dong/services/auto-order.services';
import { ChatAi } from '../../../shared/components/chat-ai/chat-ai';
import { Footer } from '../../../shared/components/footer/footer';
import { Header } from '../../../shared/components/header/header';
import { TokenService } from '../../../core/models/token.service';
import { ProductService } from '../../san-pham/services/product.service';
import { SanPham } from '../../san-pham/models/san-pham.model';
import { VND_PER_PAWPOINT } from '../../don-hang/pawpoints/models/pawpoints.model';

type TabGoiY = 'thuong-mua' | 'lien-quan';

interface SanPhamGoiY {
  variantId: number;
  ten: string;
  hinhAnh: string;
  diemDanhGia: number;
  soLuotDanhGia: number;
  gia: number;
}

interface ThongBaoCoupon {
  loai: 'thanh-cong' | 'loi';
  noiDung: string;
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
  private readonly autoOrderService = inject(AutoOrderService);
  private readonly tokenService = inject(TokenService);
  private readonly productService = inject(ProductService);
  private readonly activatedRoute = inject(ActivatedRoute);

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
  // Đánh dấu tạm các dòng đang chờ API tạo Auto Order trả về, để checkbox không bị
  // tick rồi tự bật lại về unchecked trong lúc chờ (xem daBatAutoOrder() bên dưới).
  readonly cartItemDangBat = signal<Set<number>>(new Set());

  daBatAutoOrder(cartItemId: number): boolean {
    return !!this.autoOrderIdTheoDong()[cartItemId] || this.cartItemDangBat().has(cartItemId);
  }

  readonly maCoupon = signal('');
  readonly dangApDungMa = signal(false);
  readonly thongBaoCoupon = signal<ThongBaoCoupon | null>(null);

  readonly tongTienHienThi = computed(() => {
    const gh = this.gioHang();
    if (!gh) return 0;
    return Math.max(0, gh.tienHang - gh.giamGia);
  });

  readonly diemThuongDuKien = computed(() => Math.floor(this.tongTienHienThi() / VND_PER_PAWPOINT));

  constructor() {
    this.taiGioHang();
    this.apDungMaTuQueryParam();
  }

  private apDungMaTuQueryParam(): void {
    const ma = this.activatedRoute.snapshot.queryParamMap.get('ma');
    if (ma) this.chonMa(ma);
  }

  taiGioHang(): void {
    this.dangTai.set(true);
    this.loi.set(null);
    this.cartService.layGioHang().subscribe({
      next: res => {
        this.gioHang.set(res.data ?? null);
        this.dangTai.set(false);
        this.taiSanPhamGoiY();
        this.dongBoAutoOrderTuDanhSach();
        const maDangApDung = res.data?.maCouponDangApDung;
        if (maDangApDung) {
          this.maCoupon.set(maDangApDung);
          this.thongBaoCoupon.set({ loai: 'thanh-cong', noiDung: `Coupon "${maDangApDung}" applied successfully.` });
        }
      },
      error: () => {
        this.loi.set('Could not load your cart. Please try again.');
        this.dangTai.set(false);
      }
    });
  }

  // Đối chiếu Auto Order đã có sẵn trên server (theo variantId) với các dòng trong giỏ,
  // để checkbox hiển thị đúng trạng thái đã tick sẵn thay vì luôn trống mỗi lần vào lại
  // trang - tránh việc bấm lại tạo trùng thêm 1 Auto Order cho cùng 1 sản phẩm.
  private dongBoAutoOrderTuDanhSach(): void {
    const gh = this.gioHang();
    if (!gh || gh.items.length === 0) return;

    this.autoOrderService.layDanhSach().subscribe({
      next: danhSach => {
        const map: Record<number, number> = {};
        for (const item of gh.items) {
          const donTrung = danhSach.find(d => d.variantId === item.variantId && d.trangThai !== 'cancelled');
          if (donTrung) map[item.cartItemId] = donTrung.autoOrderId;
        }
        this.autoOrderIdTheoDong.set(map);
      },
      error: () => {
        // Không chặn trang giỏ hàng nếu lấy danh sách Auto Order thất bại
      }
    });
  }

  private taiSanPhamGoiY(): void {
    const dongDauTien = this.gioHang()?.items[0];
    if (!dongDauTien) {
      this.layGoiYMacDinh();
      return;
    }

    this.productService.search({ tuKhoa: dongDauTien.tenSanPham, page: 1, pageSize: 1 }).subscribe({
      next: ket => {
        const sp = ket.items[0];
        if (sp) {
          this.taiGoiYTheoSanPham(sp.productId, sp.categoryId);
        } else {
          this.layGoiYMacDinh();
        }
      },
      error: () => this.layGoiYMacDinh()
    });
  }

  private layGoiYMacDinh(): void {
    this.productService.search({ page: 1, pageSize: 1 }).subscribe(ket => {
      const sp = ket.items[0];
      if (sp) this.taiGoiYTheoSanPham(sp.productId, sp.categoryId);
    });
  }

  private taiGoiYTheoSanPham(productId: number, categoryId: number): void {
    this.productService.getBanChay(productId, 4).subscribe(ds =>
      this.sanPhamThuongMua.set(ds.map(sp => this.mapSanPhamThanhGoiY(sp)))
    );

    this.productService.search({ categoryId, page: 1, pageSize: 5 }).subscribe(ket =>
      this.sanPhamLienQuan.set(
        ket.items.filter(sp => sp.productId !== productId).slice(0, 4).map(sp => this.mapSanPhamThanhGoiY(sp))
      )
    );
  }

  private mapSanPhamThanhGoiY(sp: SanPham): SanPhamGoiY {
    const bienThe = sp.variants.find(v => v.dangKinhDoanh && v.soLuongTon > 0) ?? sp.variants[0];
    return {
      variantId: bienThe?.variantId ?? 0,
      ten: sp.ten,
      hinhAnh: sp.images.find(i => i.laAnhChinh)?.url ?? sp.images[0]?.url ?? '/img/logo1.png',
      diemDanhGia: sp.diemDanhGiaTb,
      soLuotDanhGia: sp.soLuongDanhGia,
      gia: sp.giaTu ?? 0
    };
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

    // Đánh dấu ngay là "đang bật" để checkbox không bị bật rồi tự tắt lại trong lúc chờ API
    this.cartItemDangBat.update(bo => new Set(bo).add(item.cartItemId));

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
        this.cartItemDangBat.update(bo => { const con = new Set(bo); con.delete(item.cartItemId); return con; });
        this.dangXuLyAutoOrderDong.set(null);
      },
      error: () => {
        this.cartItemDangBat.update(bo => { const con = new Set(bo); con.delete(item.cartItemId); return con; });
        this.dangXuLyAutoOrderDong.set(null);
      }
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
            tongTien: Math.max(0, tienHang - gh.giamGia)
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
        this.gioHang.update(gh => gh ? { ...gh, items: [], tienHang: 0, giamGia: 0, tongTien: 0, couponId: null, maCouponDangApDung: null } : gh);
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
        this.capNhatGioHangTuServer();
        this.thongBaoCoupon.set({ loai: 'thanh-cong', noiDung: `Coupon "${ma}" applied successfully.` });
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

  // Sau khi áp mã, load lại giỏ hàng thật từ server (đã lưu bền cart.CouponId)
  private capNhatGioHangTuServer(): void {
    this.cartService.layGioHang().subscribe({
      next: res => this.gioHang.set(res.data ?? this.gioHang()),
      error: () => this.thongBaoCoupon.set({ loai: 'loi', noiDung: 'Applied, but failed to refresh cart totals. Please reload the page.' })
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
        tongTien: Math.max(0, tienHang - gh.giamGia)
      };
    });
  }
}