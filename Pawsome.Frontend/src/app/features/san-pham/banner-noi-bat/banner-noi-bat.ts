import { DecimalPipe } from '@angular/common';
import { Component, effect, inject, input, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { SanPham, giaVipPlaceholder } from '../models/san-pham.model';
import { ProductService } from '../services/product.service';

@Component({
  selector: 'app-banner-noi-bat',
  imports: [RouterLink, DecimalPipe],
  templateUrl: './banner-noi-bat.html',
  styleUrl: './banner-noi-bat.scss'
})
export class BannerNoiBat {
  private readonly productService = inject(ProductService);

  readonly productIdTrai = input.required<number>();
  readonly productIdPhai = input.required<number>();
  readonly anhTrungTam = input.required<string>();
  readonly altTrungTam = input.required<string>();
  readonly anhNhoTrai = input.required<string>();
  readonly anhNhoPhai = input.required<string>();
  readonly anhHeroTrai = input.required<string[]>();
  readonly anhHeroPhai = input.required<string[]>();

  protected readonly giaVipPlaceholder = giaVipPlaceholder;

  // Tên + giá trước đây bị hardcode cứng "Heartgard Plus" cho cả 2 bên, sai lệch với ảnh sản phẩm
  // thật (productIdTrai/productIdPhai) - giờ lấy đúng sản phẩm thật theo từng ID để tên/giá luôn
  // khớp với ảnh.
  readonly sanPhamTrai = signal<SanPham | null>(null);
  readonly sanPhamPhai = signal<SanPham | null>(null);

  constructor() {
    effect(() => {
      this.productService.getById(this.productIdTrai()).subscribe(sp => this.sanPhamTrai.set(sp));
    });
    effect(() => {
      this.productService.getById(this.productIdPhai()).subscribe(sp => this.sanPhamPhai.set(sp));
    });
  }
}
