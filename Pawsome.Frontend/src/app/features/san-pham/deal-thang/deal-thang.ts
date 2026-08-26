import { DecimalPipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { SanPham } from '../models/san-pham.model';
import { ProductService } from '../services/product.service';

// "Deal Of The Month" tách thành component riêng (giống banner-noi-bat/welcome-bonus) - vừa gọn
// trang-chu.ts, vừa giảm CSS trang-chu.scss xuống dưới ngưỡng budget của Angular (8kB/component).
// Trước đây hardcode cứng tên/giá 3 sản phẩm (Dorwest/Aristopet/Milpro), không có link nào cả -
// giờ lấy đúng 3 sản phẩm thật này để tên/giá khớp thật và bấm vào được.
@Component({
  selector: 'app-deal-thang',
  imports: [RouterLink, DecimalPipe],
  templateUrl: './deal-thang.html',
  styleUrl: './deal-thang.scss'
})
export class DealThang implements OnInit {
  private readonly productService = inject(ProductService);

  readonly sanPhamDealThang = signal<SanPham[]>([]);
  private readonly DEAL_THANG_PRODUCT_IDS = [10, 11, 12];

  ngOnInit(): void {
    forkJoin(this.DEAL_THANG_PRODUCT_IDS.map(id => this.productService.getById(id)))
      .subscribe(ds => this.sanPhamDealThang.set(ds.filter((sp): sp is SanPham => sp !== null)));
  }

  // Tên đầy đủ dài (vd "Aristopet Horse Wormer") không vừa khung tròn nhỏ - từ đầu tiên trong tên
  // sản phẩm ở catalog này luôn chính là tên thương hiệu (Dorwest/Aristopet/Milpro...), lấy gọn lại.
  tenNganDeal(ten: string): string {
    return ten.split(' ')[0];
  }
}
