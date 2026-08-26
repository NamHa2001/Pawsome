import { HttpClient } from '@angular/common/http';
import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { environment } from '../../../../environments/environment';

interface BaiVietBlog {
  postId: number;
  tieuDe: string;
  anhDaiDien: string | null;
  ngayDang: string;
}

// "Read Our Latest Blogs & Articles" tách thành component riêng (giống deal-thang/danh-gia-noi-bat)
// - tự lấy 3 bài blog mới nhất của mình, không cần trang-chu truyền vào. Đọc API /Blog của Phần 5
// (chỉ đọc để hiển thị, không ghi - đúng Pawsome_KhungDuAn.md mục 5.1).
@Component({
  selector: 'app-blog-noi-bat',
  imports: [RouterLink],
  templateUrl: './blog-noi-bat.html',
  styleUrl: './blog-noi-bat.scss'
})
export class BlogNoiBat implements OnInit {
  private readonly http = inject(HttpClient);

  readonly blogList = signal<BaiVietBlog[]>([]);
  private readonly anhBlogMauArr = ['/img/blog1.png', '/img/blog2.png', '/img/blog3.png'];

  ngOnInit(): void {
    this.http.get<{ data: { items: BaiVietBlog[] } }>(`${environment.apiUrl}/Blog`, { params: { page: 1, pageSize: 3 } })
      .subscribe({
        next: res => this.blogList.set(res.data?.items ?? []),
        error: () => this.blogList.set([])
      });
  }

  anhBlogMacDinh(index: number): string {
    return this.anhBlogMauArr[index % this.anhBlogMauArr.length];
  }
}
