import { DatePipe } from '@angular/common';
import { Component, computed, inject, input, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TokenService } from '../../../core/models/token.service';
import { DanhGia } from '../models/san-pham.model';
import { ReviewService } from '../services/review.service';
import { DanhGiaTongQuan } from '../danh-gia-tong-quan/danh-gia-tong-quan';

@Component({
  selector: 'app-danh-gia-san-pham',
  imports: [FormsModule, DatePipe, DanhGiaTongQuan],
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
  diemChatLuongMoi = 5;
  diemGiaTriMoi = 5;
  diemHaiLongMoi = 5;
  readonly dangGuiDanhGia = signal(false);
  readonly loiGuiDanhGia = signal<string | null>(null);
  readonly guiThanhCong = signal(false);

  readonly coTheDanhGia = signal(false);
  readonly dangKiemTraDieuKien = signal(true);

  // Khoá riêng từng review đang vote (không khoá cả trang) - vote review A không có lý do gì
  // chặn vote review B, đó là 2 dòng độc lập trong DB. Cùng pattern Set<id> đã dùng ở
  // quan-tri/nguoi-dung (dangCapNhat) cho các nút thao tác theo dòng.
  readonly dangVoteReviewIds = signal<ReadonlySet<number>>(new Set());
  readonly loiVote = signal<string | null>(null);

  ngOnInit(): void {
    this.taiDanhGia(1);

    if (this.tokenService.isLoggedIn()) {
      this.reviewService.coTheDanhGia(this.productId()).subscribe({
        next: ok => {
          this.coTheDanhGia.set(ok);
          this.dangKiemTraDieuKien.set(false);
        },
        error: () => this.dangKiemTraDieuKien.set(false)
      });
    } else {
      this.dangKiemTraDieuKien.set(false);
    }
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
    if (!confirm('Are you sure you want to submit this review?')) return;

    this.dangGuiDanhGia.set(true);
    this.loiGuiDanhGia.set(null);

    this.reviewService.create({
      productId: this.productId(),
      soSao: this.soSaoMoi,
      binhLuan: this.binhLuanMoi || undefined,
      diemChatLuong: this.diemChatLuongMoi,
      diemGiaTri: this.diemGiaTriMoi,
      diemHaiLongThuCung: this.diemHaiLongMoi
    }).subscribe({
      next: () => {
        this.dangGuiDanhGia.set(false);
        this.guiThanhCong.set(true);
        this.binhLuanMoi = '';
        this.soSaoMoi = 5;
        this.diemChatLuongMoi = 5;
        this.diemGiaTriMoi = 5;
        this.diemHaiLongMoi = 5;
      },
      error: err => {
        this.dangGuiDanhGia.set(false);
        this.loiGuiDanhGia.set(err?.error?.message ?? 'Failed to submit review. Please try again.');
      }
    });
  }

  // Không cho tự vote đánh giá của chính mình - khớp đúng chặn ở backend (ReviewService.VoteAsync),
  // ẩn nút đi luôn cho rõ thay vì để khách bấm rồi mới báo lỗi.
  laDanhGiaCuaToi(r: DanhGia): boolean {
    return this.tokenService.getUser()?.userId === r.userId;
  }

  dangVoteReview(reviewId: number): boolean {
    return this.dangVoteReviewIds().has(reviewId);
  }

  vote(r: DanhGia, huuIch: boolean): void {
    if (!this.tokenService.isLoggedIn() || this.dangVoteReview(r.reviewId)) return;

    this.dangVoteReviewIds.update(ds => new Set(ds).add(r.reviewId));
    this.loiVote.set(null);
    this.reviewService.vote(r.reviewId, huuIch).subscribe({
      next: ketQua => {
        this.reviews.update(ds => ds.map(x => x.reviewId === ketQua.reviewId ? ketQua : x));
        this.ketThucVote(r.reviewId);
      },
      error: err => {
        this.ketThucVote(r.reviewId);
        this.loiVote.set(err?.error?.message ?? 'Failed to vote. Please try again.');
      }
    });
  }

  private ketThucVote(reviewId: number): void {
    this.dangVoteReviewIds.update(ds => {
      const moi = new Set(ds);
      moi.delete(reviewId);
      return moi;
    });
  }
}
