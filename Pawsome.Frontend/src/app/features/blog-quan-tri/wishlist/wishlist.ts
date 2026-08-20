import { DecimalPipe } from '@angular/common';
import { Component, DestroyRef, OnInit, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { EMPTY, Subject, catchError, switchMap, tap } from 'rxjs';
import { Footer } from '../../../shared/components/footer/footer';
import { Header } from '../../../shared/components/header/header';
import { ChatAi } from '../../../shared/components/chat-ai/chat-ai';
import { WishlistItem } from './models/wishlist.model';
import { WishlistService } from './services/wishlist.service';

const PAGE_SIZE = 12;

@Component({
  selector: 'app-wishlist',
  standalone: true,
  imports: [DecimalPipe, RouterLink, Header, Footer, ChatAi],
  templateUrl: './wishlist.html',
  styleUrl: './wishlist.scss'
})
export class Wishlist implements OnInit {
  private readonly wishlistService = inject(WishlistService);
  private readonly destroyRef = inject(DestroyRef);

  readonly danhSach = signal<WishlistItem[]>([]);
  readonly tongSo = signal(0);
  readonly trang = signal(1);
  readonly tongTrang = signal(0);
  readonly dangTai = signal(true);
  readonly loi = signal<string | null>(null);
  readonly loiHanhDong = signal<string | null>(null);
  readonly dangXoa = signal<ReadonlySet<number>>(new Set());

  // Danh sách yêu thích cá nhân hiếm khi nhiều trang - không cần rút gọn kiểu "…" như
  // trang Blog công khai, chỉ cần tính sẵn 1 lần thay vì tạo mảng mới mỗi lần CD chạy.
  readonly cacTrang = computed(() => Array.from({ length: this.tongTrang() }, (_, i) => i + 1));

  private readonly taiTrang$ = new Subject<number>();

  ngOnInit(): void {
    // switchMap: đổi trang liên tục tự hủy request trước đó đang chờ - tránh phản hồi
    // trả về không đúng thứ tự đè lên danh sách/trạng thái phân trang hiện tại.
    this.taiTrang$.pipe(
      tap(() => {
        this.dangTai.set(true);
        this.loi.set(null);
      }),
      switchMap(trang => this.wishlistService.layDanhSach(trang, PAGE_SIZE).pipe(
        catchError(() => {
          this.loi.set('Failed to load your wishlist. Please try again.');
          this.dangTai.set(false);
          return EMPTY;
        })
      )),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(ket => {
      this.danhSach.set(ket.items);
      this.tongSo.set(ket.totalCount);
      this.tongTrang.set(ket.totalPages);
      // Backend tự kẹp về trang hợp lệ (VD trang đang xem vừa bị xóa hết item) - luôn tin
      // theo pageNumber backend trả về, không phải giá trị đã yêu cầu.
      this.trang.set(ket.pageNumber);
      this.dangTai.set(false);
    });

    this.taiTrang$.next(1);
  }

  thuLai(): void {
    this.taiTrang$.next(this.trang());
  }

  doiTrang(trang: number): void {
    if (trang < 1 || trang > this.tongTrang()) return;
    this.taiTrang$.next(trang);
  }

  dangXoaDong(productId: number): boolean {
    return this.dangXoa().has(productId);
  }

  xoa(item: WishlistItem): void {
    if (this.dangXoaDong(item.productId)) return;

    this.loiHanhDong.set(null);
    this.dangXoa.update(ds => new Set(ds).add(item.productId));

    this.wishlistService.xoa(item.productId).subscribe({
      next: () => {
        this.ketThucXoa(item.productId);
        // Item cuối cùng của trang bị xóa và không phải trang 1 - lùi về trang trước
        // thay vì hiển thị 1 trang rỗng trong khi vẫn còn item ở trang trước đó.
        const trangSeLui = this.danhSach().length === 1 && this.trang() > 1;
        this.taiTrang$.next(trangSeLui ? this.trang() - 1 : this.trang());
      },
      error: () => {
        this.loiHanhDong.set('Failed to remove item. Please try again.');
        this.ketThucXoa(item.productId);
      }
    });
  }

  private ketThucXoa(productId: number): void {
    this.dangXoa.update(ds => {
      const moi = new Set(ds);
      moi.delete(productId);
      return moi;
    });
  }
}
