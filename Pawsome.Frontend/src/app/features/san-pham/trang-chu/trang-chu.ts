import { DecimalPipe } from '@angular/common';
import { Component, OnDestroy, OnInit, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { TokenService } from '../../../core/models/token.service';
import { ChatAi } from '../../../shared/components/chat-ai/chat-ai';
import { Footer } from '../../../shared/components/footer/footer';
import { Header } from '../../../shared/components/header/header';
import { DanhMuc, SanPham, dichTenDanhMuc, giaVipPlaceholder, phanTramSao } from '../models/san-pham.model';
import { BrandService } from '../services/brand.service';
import { CategoryService } from '../services/category.service';
import { ProductService } from '../services/product.service';
import { BannerNoiBat } from '../banner-noi-bat/banner-noi-bat';
import { WelcomeBonus } from '../welcome-bonus/welcome-bonus';
import { DealThang } from '../deal-thang/deal-thang';
import { DanhGiaNoiBat } from '../danh-gia-noi-bat/danh-gia-noi-bat';
import { BlogNoiBat } from '../blog-noi-bat/blog-noi-bat';
import { BannerVipUuDai } from '../banner-vip-uu-dai/banner-vip-uu-dai';
import { CartService } from '../../gio-hang/gio-hang/services/cart.service';

interface NhomDanhMuc {
  danhMuc: DanhMuc;
  sanPham: SanPham[];
}

@Component({
  selector: 'app-trang-chu',
  imports: [Header, Footer, ChatAi, RouterLink, DecimalPipe, BannerNoiBat, WelcomeBonus, DealThang, DanhGiaNoiBat, BlogNoiBat, BannerVipUuDai],
  templateUrl: './trang-chu.html',
  styleUrl: './trang-chu.scss'
})
export class TrangChu implements OnInit, OnDestroy {
  private readonly categoryService = inject(CategoryService);
  private readonly productService = inject(ProductService);
  private readonly brandService = inject(BrandService);
  private readonly cartService = inject(CartService);
  private readonly tokenService = inject(TokenService);
  private readonly router = inject(Router);

  readonly danhMucList = signal<DanhMuc[]>([]);
  readonly nhomTheoDanhMuc = signal<NhomDanhMuc[]>([]);
  readonly thuongHieuList = signal<{ brandId: number; tenThuongHieu: string; logoUrl: string | null }[]>([]);
  readonly saoArr = [1, 2, 3, 4, 5];

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

    // BannerSlideshow ẩn hẳn ở mobile (trang-chu.scss, ≤900px) - không chạy timer vô ích khi
    // slideshow không hề hiển thị.
    if (window.matchMedia('(min-width: 901px)').matches) {
      this.timerSlide = setInterval(() => this.doiSlide(1), 10000);
    }
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

  diToiPawVip(event: Event): void {
    event.preventDefault();
    event.stopPropagation();
    this.router.navigate(['/gio-hang/pawvip']);
  }

  private doiSlide(buoc: number): void {
    const tong = this.slideAnh.length;
    this.slideIndex.update(i => (i + buoc + tong) % tong);
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
}
