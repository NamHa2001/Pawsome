import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { EMPTY, catchError, switchMap, tap } from 'rxjs';
import { ChatAi } from '../../../shared/components/chat-ai/chat-ai';
import { Footer } from '../../../shared/components/footer/footer';
import { Header } from '../../../shared/components/header/header';
import { BlogPost } from '../blog/models/blog.model';
import { BlogService } from '../blog/services/blog.service';

@Component({
  selector: 'app-blog-chi-tiet',
  standalone: true,
  imports: [CommonModule, RouterLink, Header, Footer, ChatAi],
  templateUrl: './blog-chi-tiet.html',
  styleUrl: './blog-chi-tiet.scss'
})
export class BlogChiTiet implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly blogService = inject(BlogService);
  private readonly destroyRef = inject(DestroyRef);

  // int trong C# tối đa 2147483647 - id vượt mốc này chắc chắn không map được vào
  // [HttpGet("{id}")] backend (ASP.NET trả 400 do lỗi model binding, không phải 404),
  // nên phải chặn ở đây để không rơi nhầm vào nhánh "Failed to load" thay vì "not found".
  private static readonly ID_TOI_DA = 2147483647;

  readonly baiViet = signal<BlogPost | null>(null);
  readonly dangTai = signal(true);
  readonly khongTimThay = signal(false);
  readonly loi = signal<string | null>(null);

  ngOnInit(): void {
    // switchMap: chuyển nhanh giữa 2 bài viết (VD bấm liên tiếp, hoặc
    // back/forward trình duyệt) trước khi request trước hoàn tất sẽ tự hủy
    // request cũ - tránh bài viết cũ tải chậm hơn đè lên bài viết đang xem.
    this.route.paramMap.pipe(
      tap(() => {
        this.dangTai.set(true);
        this.khongTimThay.set(false);
        this.loi.set(null);
      }),
      switchMap(params => {
        const id = Number(params.get('id'));

        // Chặn sớm URL sai dạng (VD /blog/abc) - không gọi API với NaN, vì
        // backend sẽ trả 400 (lỗi model binding) chứ không phải 404, khiến
        // rơi nhầm vào nhánh "Failed to load" thay vì "Article not found."
        if (!Number.isInteger(id) || id < 1 || id > BlogChiTiet.ID_TOI_DA) {
          this.khongTimThay.set(true);
          this.dangTai.set(false);
          return EMPTY;
        }

        return this.blogService.layChiTiet(id).pipe(
          catchError((err: HttpErrorResponse) => {
            // 404 = bài viết không tồn tại, khác với lỗi mạng/server - phải
            // phân biệt để hiển thị đúng "Article not found."
            if (err.status === 404) {
              this.khongTimThay.set(true);
            } else {
              this.loi.set('Failed to load the article. Please try again.');
            }
            this.dangTai.set(false);
            return EMPTY;
          })
        );
      }),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(bv => {
      this.baiViet.set(bv);
      this.khongTimThay.set(!bv);
      this.dangTai.set(false);
    });
  }

  private static readonly LINK_CHIA_SE: Record<'facebook' | 'zalo', (url: string) => string> = {
    facebook: url => `https://www.facebook.com/sharer/sharer.php?u=${url}`,
    zalo: url => `https://sp.zalo.me/share?u=${url}`
  };

  chiaSe(mang: 'facebook' | 'zalo'): void {
    const url = encodeURIComponent(window.location.href);
    window.open(BlogChiTiet.LINK_CHIA_SE[mang](url), '_blank', 'noopener,width=600,height=500');
  }
}
