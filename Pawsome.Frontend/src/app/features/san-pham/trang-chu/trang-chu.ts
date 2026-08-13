import { CurrencyPipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, OnDestroy, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ChatAi } from '../../../shared/components/chat-ai/chat-ai';
import { Footer } from '../../../shared/components/footer/footer';
import { Header } from '../../../shared/components/header/header';
import { DanhMuc, SanPham, dichTenDanhMuc, quyDoiUSD } from '../models/san-pham.model';
import { BrandService } from '../services/brand.service';
import { CategoryService } from '../services/category.service';
import { ProductService } from '../services/product.service';
import { BannerNoiBat } from '../banner-noi-bat/banner-noi-bat';
import { WelcomeBonus } from '../welcome-bonus/welcome-bonus';
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
  imports: [Header, Footer, ChatAi, RouterLink, CurrencyPipe, BannerNoiBat, WelcomeBonus],
  templateUrl: './trang-chu.html',
  styleUrl: './trang-chu.scss'
})
export class TrangChu implements OnInit, OnDestroy {
  private readonly categoryService = inject(CategoryService);
  private readonly productService = inject(ProductService);
  private readonly brandService = inject(BrandService);
  private readonly couponService = inject(CouponService);
  private readonly http = inject(HttpClient);

  readonly danhMucList = signal<DanhMuc[]>([]);
  readonly nhomTheoDanhMuc = signal<NhomDanhMuc[]>([]);
  readonly thuongHieuList = signal<{ brandId: number; tenThuongHieu: string; logoUrl: string | null }[]>([]);
  readonly blogList = signal<BaiVietBlog[]>([]);
  readonly couponNoiBat = signal<Coupon | null>(null);

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

    this.timerSlide = setInterval(() => this.doiSlide(1), 10000);
  }

  ngOnDestroy(): void {
    if (this.timerSlide) clearInterval(this.timerSlide);
  }

  chonSlide(i: number): void {
    this.slideIndex.set(i);
  }

  protected readonly dichTenDanhMuc = dichTenDanhMuc;
  protected readonly quyDoiUSD = quyDoiUSD;

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

  hienThiUuDai(cp: Coupon): string {
    return cp.loaiGiam === 'percent'
      ? `${cp.giaTri}% off`
      : `$${quyDoiUSD(cp.giaTri).toFixed(2)} off`;
  }
}
