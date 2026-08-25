import { DecimalPipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, OnDestroy, OnInit, computed, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { TokenService } from '../../../core/models/token.service';
import { ChatAi } from '../../../shared/components/chat-ai/chat-ai';
import { Footer } from '../../../shared/components/footer/footer';
import { Header } from '../../../shared/components/header/header';
import { DanhGia, DanhMuc, SanPham, dichTenDanhMuc, giaVipPlaceholder, phanTramSao } from '../models/san-pham.model';
import { BrandService } from '../services/brand.service';
import { CategoryService } from '../services/category.service';
import { ProductService } from '../services/product.service';
import { ReviewService } from '../services/review.service';
import { BannerNoiBat } from '../banner-noi-bat/banner-noi-bat';
import { WelcomeBonus } from '../welcome-bonus/welcome-bonus';
import { CartService } from '../../gio-hang/gio-hang/services/cart.service';
import { CouponService } from '../../gio-hang/gio-hang/services/coupon.service';
import { Coupon } from '../../gio-hang/gio-hang/models/gio-hang.model';

interface BaiVietBlog {
  postId: number;
  tieuDe: string;
  anhDaiDien: string | null;
  ngayDang: string;
}

interface NhomDanhMuc {
  danhMuc: DanhMuc;
  sanPham: SanPham[];
}

@Component({
  selector: 'app-trang-chu',
  imports: [Header, Footer, ChatAi, RouterLink, DecimalPipe, BannerNoiBat, WelcomeBonus],
  templateUrl: './trang-chu.html',
  styleUrl: './trang-chu.scss'
})
export class TrangChu implements OnInit, OnDestroy {
  private readonly categoryService = inject(CategoryService);
  private readonly productService = inject(ProductService);
  private readonly brandService = inject(BrandService);
  private readonly couponService = inject(CouponService);
  private readonly reviewService = inject(ReviewService);
  private readonly cartService = inject(CartService);
  private readonly tokenService = inject(TokenService);
  private readonly router = inject(Router);
  private readonly http = inject(HttpClient);

  readonly danhMucList = signal<DanhMuc[]>([]);
  readonly nhomTheoDanhMuc = signal<NhomDanhMuc[]>([]);
  readonly thuongHieuList = signal<{ brandId: number; tenThuongHieu: string; logoUrl: string | null }[]>([]);
  readonly blogList = signal<BaiVietBlog[]>([]);
  readonly couponNoiBat = signal<Coupon | null>(null);
  readonly danhGiaNoiBat = signal<DanhGia[]>([]);
  readonly saoArr = [1, 2, 3, 4, 5];

  // "Deal Of The Month" trước đây hardcode cứng tên/giá 3 sản phẩm (Dorwest/Aristopet/Milpro),
  // không có link nào cả - giờ lấy đúng 3 sản phẩm thật này để tên/giá khớp thật và bấm vào được.
  readonly sanPhamDealThang = signal<SanPham[]>([]);
  private readonly DEAL_THANG_PRODUCT_IDS = [10, 11, 12];

  // Tên đầy đủ dài (vd "Aristopet Horse Wormer") không vừa khung tròn nhỏ - từ đầu tiên trong tên
  // sản phẩm ở catalog này luôn chính là tên thương hiệu (Dorwest/Aristopet/Milpro...), lấy gọn lại.
  tenNganDeal(ten: string): string {
    return ten.split(' ')[0];
  }

  // Hiển thị 2 đánh giá/lần, bấm mũi tên trái/phải để xem cặp tiếp theo (Reviews đã tải sẵn cả
  // danhGiaNoiBat() từ backend - soLuong=6 - nên chuyển trang chỉ cần cắt mảng, không gọi lại API).
  private readonly SO_DANH_GIA_MOI_TRANG = 2;
  readonly trangDanhGiaNoiBat = signal(0);

  readonly tongTrangDanhGiaNoiBat = computed(() =>
    Math.ceil(this.danhGiaNoiBat().length / this.SO_DANH_GIA_MOI_TRANG)
  );

  readonly danhGiaHienThi = computed(() => {
    const batDau = this.trangDanhGiaNoiBat() * this.SO_DANH_GIA_MOI_TRANG;
    return this.danhGiaNoiBat().slice(batDau, batDau + this.SO_DANH_GIA_MOI_TRANG);
  });

  doiTrangNoiBat(buoc: number): void {
    const trangCuoi = this.tongTrangDanhGiaNoiBat() - 1;
    this.trangDanhGiaNoiBat.update(t => Math.min(Math.max(t + buoc, 0), trangCuoi));
  }

  readonly slideIndex = signal(0);
  readonly slideAnh = ['/img/banner1.png', '/img/banner3.png', '/img/banner2.png'];
  private timerSlide: ReturnType<typeof setInterval> | undefined;

  ngOnInit(): void {
    this.categoryService.getAll().subscribe(ds => {
      const goc = ds.filter(d => !d.danhMucChaId).slice(0, 2);
      this.danhMucList.set(goc);

      if (goc.length === 0) return;

      forkJoin(
        goc.map(dm => this.productService.search({ categoryId: dm.categoryId, page: 1, pageSize: 8 }))
      ).subscribe(ketQuaList => {
        this.nhomTheoDanhMuc.set(
          goc.map((dm, i) => ({ danhMuc: dm, sanPham: ketQuaList[i].items }))
        );
      });
    });

    this.brandService.getAll().subscribe(ds => this.thuongHieuList.set(ds.slice(0, 6)));

    // Flash Sale trang chủ = coupon đang hiệu lực do Phần 3 quản lý, Phần 2 chỉ đọc để hiển thị
    // (Pawsome_KhungDuAn.md mục 5.9) - chọn coupon % giảm cao nhất để làm nổi bật.
    this.couponService.layDangHieuLuc().subscribe(ds => this.couponNoiBat.set(this.chonCouponNoiBat(ds)));

    this.http.get<{ data: { items: BaiVietBlog[] } }>(`${environment.apiUrl}/Blog`, { params: { page: 1, pageSize: 3 } })
      .subscribe({
        next: res => this.blogList.set(res.data?.items ?? []),
        error: () => this.blogList.set([])
      });

    this.reviewService.getNoiBat(6).subscribe(ds => this.danhGiaNoiBat.set(ds));

    forkJoin(this.DEAL_THANG_PRODUCT_IDS.map(id => this.productService.getById(id)))
      .subscribe(ds => this.sanPhamDealThang.set(ds.filter((sp): sp is SanPham => sp !== null)));

    this.timerSlide = setInterval(() => this.doiSlide(1), 10000);
  }

  ngOnDestroy(): void {
    if (this.timerSlide) clearInterval(this.timerSlide);
  }

  chonSlide(i: number): void {
    this.slideIndex.set(i);
  }

  protected readonly dichTenDanhMuc = dichTenDanhMuc;
  protected readonly giaVipPlaceholder = giaVipPlaceholder;
  protected readonly phanTramSao = phanTramSao;

  private readonly anhBlogMauArr = ['/img/blog1.png', '/img/blog2.png', '/img/blog3.png'];

  anhBlogMacDinh(index: number): string {
    return this.anhBlogMauArr[index % this.anhBlogMauArr.length];
  }

  private doiSlide(buoc: number): void {
    const tong = this.slideAnh.length;
    this.slideIndex.update(i => (i + buoc + tong) % tong);
  }

  private chonCouponNoiBat(list: Coupon[]): Coupon | null {
    if (list.length === 0) return null;

    const theoPhanTram = list.filter(c => c.loaiGiam === 'percent');
    const nguon = theoPhanTram.length > 0 ? theoPhanTram : list;
    return nguon.reduce((noiBat, c) => (c.giaTri > noiBat.giaTri ? c : noiBat), nguon[0]);
  }

  readonly dangThemGioNhanh = signal(false);

  // Nút "SHOP NOW" trên thẻ sản phẩm (trang chủ, gợi ý...) - thêm thẳng vào giỏ hàng thay vì chỉ
  // điều hướng như trước (cả thẻ đã là 1 thẻ <a> bọc ngoài để bấm ảnh/tên vào trang chi tiết, nên
  // nút bên trong phải preventDefault + stopPropagation để không bị điều hướng theo click cha).
  // Tự chọn biến thể đầu tiên còn hàng - trang chủ không có chỗ để khách chọn biến thể cụ thể.
  themVaoGioNhanh(sp: SanPham, event: Event): void {
    event.preventDefault();
    event.stopPropagation();

    if (!this.tokenService.isLoggedIn()) {
      this.router.navigate(['/tai-khoan/dang-nhap']);
      return;
    }

    const bienThe = sp.variants.find(v => v.dangKinhDoanh && v.soLuongTon > 0);
    if (!bienThe) {
      alert('This product is currently out of stock.');
      return;
    }

    this.dangThemGioNhanh.set(true);
    this.cartService.themSanPham({ variantId: bienThe.variantId, soLuong: 1 }).subscribe({
      next: () => {
        this.dangThemGioNhanh.set(false);
      },
      error: err => {
        this.dangThemGioNhanh.set(false);
        alert(err?.error?.message ?? 'Failed to add to cart.');
      }
    });
  }

  hienThiUuDai(cp: Coupon): string {
    return cp.loaiGiam === 'percent'
      ? `${cp.giaTri}% off`
      : `${cp.giaTri.toLocaleString('en-US')} VND off`;
  }
}
