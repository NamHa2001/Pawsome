import { DecimalPipe } from '@angular/common';
import { Component, computed, effect, inject, input, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { SanPham } from '../models/san-pham.model';
import { ProductService } from '../services/product.service';
import { CartService } from '../../gio-hang/gio-hang/services/cart.service';
import { TokenService } from '../../../core/models/token.service';

@Component({
  selector: 'app-goi-y-san-pham',
  imports: [RouterLink, DecimalPipe],
  templateUrl: './goi-y-san-pham.html',
  styleUrl: './goi-y-san-pham.scss'
})
export class GoiYSanPham {
  private readonly productService = inject(ProductService);
  private readonly cartService = inject(CartService);
  private readonly tokenService = inject(TokenService);
  private readonly router = inject(Router);

  readonly productId = input.required<number>();
  readonly categoryId = input.required<number>();

  readonly sanPhamCungDanhMuc = signal<SanPham[]>([]);
  readonly sanPhamBanChay = signal<SanPham[]>([]);
  readonly tabGoiY = signal<'bought' | 'related'>('bought');
  readonly dangThemGio = signal(false);

  readonly danhSachGoiYHienThi = computed(() =>
    this.tabGoiY() === 'bought' ? this.sanPhamBanChay() : this.sanPhamCungDanhMuc()
  );

  constructor() {
    effect(() => {
      const productId = this.productId();
      const categoryId = this.categoryId();

      this.productService.search({ categoryId, page: 1, pageSize: 5 }).subscribe(ket => {
        this.sanPhamCungDanhMuc.set(ket.items.filter(p => p.productId !== productId).slice(0, 4));
      });

      // "Frequently Bought" - khác "Related Items" (cùng danh mục, mới nhất): sản phẩm bán chạy
      // nhất (tổng số lượng đã bán từ đơn hàng thật) trong cùng danh mục. Có thể rỗng nếu danh
      // mục chưa có đơn hàng nào - không bịa dữ liệu thay thế, để trống đúng như thực tế.
      this.productService.getBanChay(productId, 4).subscribe(ds => this.sanPhamBanChay.set(ds));
    });
  }

  // Nút "SHOP NOW" trên thẻ gợi ý - thêm thẳng vào giỏ hàng thay vì chỉ điều hướng như trước
  // (cả thẻ là 1 thẻ <a> bọc ngoài, nút bên trong phải preventDefault + stopPropagation để không
  // bị điều hướng theo click cha). Tự chọn biến thể đầu tiên còn hàng, số lượng 1.
  themVaoGioNhanh(item: SanPham, event: Event): void {
    event.preventDefault();
    event.stopPropagation();

    if (!this.tokenService.isLoggedIn()) {
      this.router.navigate(['/tai-khoan/dang-nhap']);
      return;
    }

    const bienThe = item.variants.find(v => v.dangKinhDoanh && v.soLuongTon > 0);
    if (!bienThe) {
      alert('This product is currently out of stock.');
      return;
    }

    this.dangThemGio.set(true);
    this.cartService.themSanPham({ variantId: bienThe.variantId, soLuong: 1 }).subscribe({
      next: () => {
        this.dangThemGio.set(false);
      },
      error: err => {
        this.dangThemGio.set(false);
        alert(err?.error?.message ?? 'Failed to add to cart.');
      }
    });
  }
}
