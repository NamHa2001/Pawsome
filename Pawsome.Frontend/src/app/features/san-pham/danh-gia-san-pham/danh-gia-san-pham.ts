import { DatePipe, DecimalPipe } from '@angular/common';
import { Component, computed, inject, input, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { TokenService } from '../../../core/models/token.service';
import { DanhGia } from '../models/san-pham.model';
import { ReviewService } from '../services/review.service';

@Component({
  selector: 'app-danh-gia-san-pham',
  imports: [RouterLink, FormsModule, DatePipe, DecimalPipe],
  templateUrl: './danh-gia-san-pham.html',
  styleUrl: './danh-gia-san-pham.scss'
})
export class DanhGiaSanPham implements OnInit {
  protected readonly Array = Array;

  readonly productId = input.required<number>();
  readonly diemDanhGiaTb = input(0);
  readonly soLuongDanhGia = input(0);

  protected readonly saoArr = [1, 2, 3, 4, 5];

  private readonly reviewService = inject(ReviewService);
  readonly tokenService = inject(TokenService);

  readonly reviews = signal<DanhGia[]>([]);
  readonly reviewPage = signal(1);
  readonly reviewTotalPages = signal(0);
  readonly phanBoSao = signal<Record<number, number>>({ 1: 0, 2: 0, 3: 0, 4: 0, 5: 0 });

  readonly sapXepDanhGia = signal<'recent' | 'highest' | 'lowest'>('recent');
  readonly reviewsDaSapXep = computed<DanhGia[]>(() => {
    const list = [...this.reviews()];
    switch (this.sapXepDanhGia()) {
      case 'highest': return list.sort((a, b) => b.soSao - a.soSao);
      case 'lowest': return list.sort((a, b) => a.soSao - b.soSao);
      default: return list;
    }
  });

  readonly tongPhanTramSao = computed<Record<number, number>>(() => {
    const pb = this.phanBoSao();
    const tong = Object.values(pb).reduce((a, b) => a + b, 0);
    if (tong === 0) return { 1: 0, 2: 0, 3: 0, 4: 0, 5: 0 };
    return {
      1: (pb[1] / tong) * 100,
      2: (pb[2] / tong) * 100,
      3: (pb[3] / tong) * 100,
      4: (pb[4] / tong) * 100,
      5: (pb[5] / tong) * 100
    };
  });

  soSaoMoi = 5;
  binhLuanMoi = '';
  readonly dangGuiDanhGia = signal(false);
  readonly loiGuiDanhGia = signal<string | null>(null);
  readonly guiThanhCong = signal(false);

  ngOnInit(): void {
    this.taiDanhGia(1);
  }

  private taiDanhGia(page: number): void {
    const id = this.productId();

    this.reviewService.getByProduct(id, page, 20).subscribe(ket => {
      this.reviews.set(ket.items);
      this.reviewPage.set(ket.pageNumber);
      this.reviewTotalPages.set(ket.totalPages);
    });

    if (page === 1) {
      this.reviewService.getByProduct(id, 1, 100).subscribe(ket => {
        const pb: Record<number, number> = { 1: 0, 2: 0, 3: 0, 4: 0, 5: 0 };
        for (const r of ket.items) {
          pb[r.soSao] = (pb[r.soSao] ?? 0) + 1;
        }
        this.phanBoSao.set(pb);
      });
    }
  }

  doiTrangDanhGia(page: number): void {
    if (page < 1 || page > this.reviewTotalPages()) return;
    this.taiDanhGia(page);
  }

  guiDanhGia(): void {
    this.dangGuiDanhGia.set(true);
    this.loiGuiDanhGia.set(null);

    this.reviewService.create({ productId: this.productId(), soSao: this.soSaoMoi, binhLuan: this.binhLuanMoi || undefined })
      .subscribe({
        next: () => {
          this.dangGuiDanhGia.set(false);
          this.guiThanhCong.set(true);
          this.binhLuanMoi = '';
          this.soSaoMoi = 5;
        },
        error: () => {
          this.dangGuiDanhGia.set(false);
          this.loiGuiDanhGia.set('Failed to submit review. Please try again.');
        }
      });
  }
}
