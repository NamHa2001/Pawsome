import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { Observable } from 'rxjs';
import { DanhGia } from '../../../san-pham/models/san-pham.model';
import { ReviewModerationService } from './services/review-moderation.service';

@Component({
  selector: 'app-quan-tri-kiem-duyet',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './kiem-duyet-danh-gia.html',
  styleUrl: './kiem-duyet-danh-gia.scss'
})
export class QuanTriKiemDuyet {
  private readonly reviewModerationService = inject(ReviewModerationService);

  readonly danhSach = signal<DanhGia[]>([]);
  readonly dangTai = signal(true);
  readonly loiTai = signal<string | null>(null);
  readonly loiHanhDong = signal<string | null>(null);
  readonly dangXuLy = signal<ReadonlySet<number>>(new Set());

  constructor() {
    this.taiDanhSach();
  }

  taiDanhSach(): void {
    this.dangTai.set(true);
    this.loiTai.set(null);

    this.reviewModerationService.layDanhSachChoDuyet().subscribe({
      next: ds => {
        this.danhSach.set(ds);
        this.dangTai.set(false);
      },
      error: () => {
        this.loiTai.set('Failed to load pending reviews.');
        this.dangTai.set(false);
      }
    });
  }

  dangXuLyDong(reviewId: number): boolean {
    return this.dangXuLy().has(reviewId);
  }

  duyet(reviewId: number): void {
    this.thucHienHanhDong(reviewId, this.reviewModerationService.duyet(reviewId), 'Failed to approve the review.');
  }

  tuChoi(reviewId: number): void {
    if (!confirm('Reject this review?')) return;

    this.thucHienHanhDong(reviewId, this.reviewModerationService.tuChoi(reviewId), 'Failed to reject the review.');
  }

  private thucHienHanhDong(reviewId: number, hanhDong$: Observable<DanhGia>, thongBaoLoi: string): void {
    this.batDauXuLy(reviewId);

    hanhDong$.subscribe({
      next: () => {
        this.danhSach.update(ds => ds.filter(d => d.reviewId !== reviewId));
        this.ketThucXuLy(reviewId);
      },
      error: () => {
        this.loiHanhDong.set(thongBaoLoi);
        this.ketThucXuLy(reviewId);
      }
    });
  }

  private batDauXuLy(reviewId: number): void {
    this.loiHanhDong.set(null);
    this.dangXuLy.update(ds => new Set(ds).add(reviewId));
  }

  private ketThucXuLy(reviewId: number): void {
    this.dangXuLy.update(ds => {
      const moi = new Set(ds);
      moi.delete(reviewId);
      return moi;
    });
  }
}
