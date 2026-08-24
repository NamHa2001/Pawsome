import { DecimalPipe } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TokenService } from '../../../core/models/token.service';
import { ChatAi } from '../../../shared/components/chat-ai/chat-ai';
import { Footer } from '../../../shared/components/footer/footer';
import { Header } from '../../../shared/components/header/header';
import { SanPham } from '../models/san-pham.model';
import { ProductService } from '../services/product.service';
import { DanhGiaSanPham } from '../danh-gia-san-pham/danh-gia-san-pham';
import { CartService } from '../../gio-hang/gio-hang/services/cart.service';
import { WishlistService } from '../../blog-quan-tri/wishlist/services/wishlist.service';

@Component({
  selector: 'app-chi-tiet-san-pham',
  imports: [Header, Footer, ChatAi, RouterLink, FormsModule, DecimalPipe, DanhGiaSanPham],
  templateUrl: './chi-tiet-san-pham.html',
  styleUrl: './chi-tiet-san-pham.scss'
})
export class ChiTietSanPham implements OnInit {
  protected readonly Math = Math;
  protected readonly Array = Array;
  readonly saoArr = [1, 2, 3, 4, 5];

  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly productService = inject(ProductService);
  private readonly cartService = inject(CartService);
  private readonly wishlistService = inject(WishlistService);
  readonly tokenService = inject(TokenService);

  readonly sanPham = signal<SanPham | null>(null);
  readonly dangTai = signal(true);
  readonly khongTimThay = signal(false);
  readonly anhDangChon = signal<string | null>(null);

  readonly sanPhamCungDanhMuc = signal<SanPham[]>([]);
  readonly tabMoTa = signal<'overview' | 'benefits' | 'direction' | 'safety' | 'ingredients'>('overview');
  readonly tabGoiY = signal<'bought' | 'related'>('bought');

  readonly soLuongChon = signal<Record<number, number>>({});

  soLuongCuaBienThe(variantId: number): number {
    return this.soLuongChon()[variantId] ?? 1;
  }

  doiSoLuong(variantId: number, giaTri: number): void {
    this.soLuongChon.update(cur => ({ ...cur, [variantId]: giaTri }));
  }

  giaVipPlaceholder(giaThat: number): number {
    return Math.round(giaThat * 0.6);
  }

  readonly dangYeuThich = signal(false);
  readonly dangXuLyYeuThich = signal(false);

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = Number(params.get('id'));
      if (id) {
        this.taiSanPham(id);
      }
    });
  }

  private taiSanPham(id: number): void {
    this.dangTai.set(true);
    this.khongTimThay.set(false);

    this.productService.getById(id).subscribe({
      next: sp => {
        this.dangTai.set(false);
        if (!sp) {
          this.khongTimThay.set(true);
          return;
        }
        this.sanPham.set(sp);
        this.anhDangChon.set(sp.images.find(i => i.laAnhChinh)?.url ?? sp.images[0]?.url ?? null);
        this.taiSanPhamCungDanhMuc(sp);
        this.kiemTraDaYeuThich(sp.productId);
      },
      error: () => {
        this.dangTai.set(false);
        this.khongTimThay.set(true);
      }
    });
  }

  private taiSanPhamCungDanhMuc(sp: SanPham): void {
    this.productService.search({ categoryId: sp.categoryId, page: 1, pageSize: 5 }).subscribe(ket => {
      this.sanPhamCungDanhMuc.set(ket.items.filter(p => p.productId !== sp.productId).slice(0, 4));
    });
  }

  // Chưa có API kiểm tra 1 sản phẩm cụ thể (WishlistService của Phần 5 chỉ có lấy cả danh sách/xóa) -
  // tạm lấy cả danh sách yêu thích rồi so productId, danh sách cá nhân này thường không lớn.
  private kiemTraDaYeuThich(productId: number): void {
    if (!this.tokenService.isLoggedIn()) return;

    this.wishlistService.layDanhSach(1, 100).subscribe(ket => {
      this.dangYeuThich.set(ket.items.some(i => i.productId === productId));
    });
  }

  readonly dangThemGio = signal(false);

  themVaoGioTam(variantId: number): void {
    const soLuongThem = this.soLuongCuaBienThe(variantId);
    if (soLuongThem <= 0) return;

    if (!this.tokenService.isLoggedIn()) {
      this.router.navigate(['/tai-khoan/dang-nhap']);
      return;
    }

    this.dangThemGio.set(true);
    this.cartService.themSanPham({ variantId, soLuong: soLuongThem }).subscribe({
      next: () => {
        this.dangThemGio.set(false);
        alert('Added to cart successfully.');
      },
      error: err => {
        this.dangThemGio.set(false);
        alert(err?.error?.message ?? 'Failed to add to cart.');
      }
    });
  }

  toggleYeuThich(): void {
    const sp = this.sanPham();
    if (!sp) return;

    if (!this.tokenService.isLoggedIn()) {
      this.router.navigate(['/tai-khoan/dang-nhap']);
      return;
    }

    this.dangXuLyYeuThich.set(true);
    const xong = () => this.dangXuLyYeuThich.set(false);

    if (this.dangYeuThich()) {
      this.wishlistService.xoa(sp.productId).subscribe({
        next: () => { this.dangYeuThich.set(false); xong(); },
        error: xong
      });
    } else {
      this.wishlistService.them(sp.productId).subscribe({
        next: () => { this.dangYeuThich.set(true); xong(); },
        error: xong
      });
    }
  }
}
