import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ProductService } from '../../../features/san-pham/services/product.service';
import { SanPham } from '../../../features/san-pham/models/san-pham.model';

const SO_SAN_PHAM_FOOTER = 12;

@Component({
  selector: 'app-footer',
  imports: [RouterLink],
  templateUrl: './footer.html',
  styleUrl: './footer.scss'
})
export class Footer {
  private readonly productService = inject(ProductService);

  readonly tabDangChon = signal<'about-us' | 'products' | 'info' | 'categories'>('about-us');
  readonly sanPhamNoiBatFooter = signal<SanPham[]>([]);

  constructor() {
    // Footer chỉ có chỗ cho vài dòng - lấy 1 lô sản phẩm rồi ưu tiên tên ngắn nhất, giới hạn
    // SO_SAN_PHAM_FOOTER sản phẩm, để không bị tràn/xuống dòng dài trong cột footer hẹp.
    this.productService.search({ page: 1, pageSize: 30 }).subscribe(res => {
      const ngan = [...res.items].sort((a, b) => a.ten.length - b.ten.length).slice(0, SO_SAN_PHAM_FOOTER);
      this.sanPhamNoiBatFooter.set(ngan);
    });
  }

  chonTab(tab: 'about-us' | 'products' | 'info' | 'categories'): void {
    this.tabDangChon.set(tab);
  }

  lenDauTrang(): void {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
}
