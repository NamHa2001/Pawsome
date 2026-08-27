import { DecimalPipe } from '@angular/common';
import { Component, inject, input, signal } from '@angular/core';
import { Router } from '@angular/router';
import { BienTheSanPham, giaVipPlaceholder } from '../models/san-pham.model';
import { CartService } from '../../gio-hang/gio-hang/services/cart.service';
import { TokenService } from '../../../core/models/token.service';

@Component({
  selector: 'app-chon-bien-the',
  imports: [DecimalPipe],
  templateUrl: './chon-bien-the.html',
  styleUrl: './chon-bien-the.scss'
})
export class ChonBienThe {
  protected readonly Math = Math;
  protected readonly giaVipPlaceholder = giaVipPlaceholder;

  private readonly cartService = inject(CartService);
  private readonly tokenService = inject(TokenService);
  private readonly router = inject(Router);

  readonly tenSanPham = input.required<string>();
  readonly variants = input.required<BienTheSanPham[]>();
  readonly anh = input.required<string>();

  readonly soLuongChon = signal<Record<number, number>>({});
  readonly dangThemGio = signal(false);

  soLuongCuaBienThe(variantId: number): number {
    return this.soLuongChon()[variantId] ?? 1;
  }

  doiSoLuong(variantId: number, giaTri: number): void {
    this.soLuongChon.update(cur => ({ ...cur, [variantId]: giaTri }));
  }

  themVaoGioTam(variantId: number): void {
    if (this.dangThemGio()) return;

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
      },
      error: err => {
        this.dangThemGio.set(false);
        alert(err?.error?.message ?? 'Failed to add to cart.');
      }
    });
  }
}