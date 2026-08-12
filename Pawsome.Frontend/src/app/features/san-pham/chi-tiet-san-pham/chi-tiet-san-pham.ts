import { CurrencyPipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { environment } from '../../../../environments/environment';
import { TokenService } from '../../../core/models/token.service';
import { ChatAi } from '../../../shared/components/chat-ai/chat-ai';
import { Footer } from '../../../shared/components/footer/footer';
import { Header } from '../../../shared/components/header/header';
import { SanPham, quyDoiUSD } from '../models/san-pham.model';
import { ProductService } from '../services/product.service';
import { DanhGiaSanPham } from '../danh-gia-san-pham/danh-gia-san-pham';

@Component({
  selector: 'app-chi-tiet-san-pham',
  imports: [Header, Footer, ChatAi, RouterLink, FormsModule, CurrencyPipe, DanhGiaSanPham],
  templateUrl: './chi-tiet-san-pham.html',
  styleUrl: './chi-tiet-san-pham.scss'
})
export class ChiTietSanPham implements OnInit {
  protected readonly Math = Math;
  protected readonly Array = Array;
  protected readonly quyDoiUSD = quyDoiUSD;
  readonly saoArr = [1, 2, 3, 4, 5];

  private readonly route = inject(ActivatedRoute);
  private readonly productService = inject(ProductService);
  private readonly http = inject(HttpClient);
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

  themVaoGioTam(variantId: number): void {
    const soLuongThem = this.soLuongCuaBienThe(variantId);
    if (soLuongThem <= 0) return;

    const soLuongHienTai = parseInt(localStorage.getItem('cartCount') || '0', 10) + soLuongThem;
    localStorage.setItem('cartCount', String(soLuongHienTai));
    alert('Added to cart (temporary) - the real cart feature is being built by Phần 3, no API yet.');
  }

  toggleYeuThich(): void {
    const sp = this.sanPham();
    if (!sp || !this.tokenService.isLoggedIn()) return;

    this.dangXuLyYeuThich.set(true);
    const url = `${environment.apiUrl}/Wishlist`;

    const xong = () => this.dangXuLyYeuThich.set(false);

    if (this.dangYeuThich()) {
      this.http.delete(`${url}/${sp.productId}`).subscribe({
        next: () => { this.dangYeuThich.set(false); xong(); },
        error: xong
      });
    } else {
      this.http.post(url, { productId: sp.productId }).subscribe({
        next: () => { this.dangYeuThich.set(true); xong(); },
        error: xong
      });
    }
  }
}
