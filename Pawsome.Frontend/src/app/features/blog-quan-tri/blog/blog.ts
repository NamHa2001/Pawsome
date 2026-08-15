import { CommonModule, Location } from '@angular/common';
import { Component, DestroyRef, OnInit, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { EMPTY, Subject, catchError, merge, of, switchMap, tap } from 'rxjs';
import { ChatAi } from '../../../shared/components/chat-ai/chat-ai';
import { Footer } from '../../../shared/components/footer/footer';
import { Header } from '../../../shared/components/header/header';
import { BlogFilterRequest, BlogPost } from './models/blog.model';
import { BlogService } from './services/blog.service';

// Thêm sẵn đoạn trích 1 lần khi nhận dữ liệu, thay vì gọi trichDoan() ngay trong
// template @for - tránh cắt lại chuỗi mỗi lần change detection chạy (VD gõ ô tìm kiếm).
interface BlogPostHienThi extends BlogPost {
  trichDoanNoiDung: string;
}

@Component({
  selector: 'app-blog',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, Header, Footer, ChatAi],
  templateUrl: './blog.html',
  styleUrl: './blog.scss'
})
export class Blog implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly location = inject(Location);
  private readonly blogService = inject(BlogService);
  private readonly destroyRef = inject(DestroyRef);

  // int trong C# tối đa 2147483647 - page vượt mốc này (dù vẫn là số nguyên JS an toàn)
  // sẽ khiến ASP.NET model binding lỗi 400 trước khi BlogService kịp tự kẹp trang, làm
  // rơi nhầm vào nhánh "Failed to load" thay vì âm thầm về trang 1 (giống blog-chi-tiet.ts).
  private static readonly PAGE_TOI_DA = 2147483647;

  readonly baiVietList = signal<BlogPostHienThi[]>([]);
  readonly tongSo = signal(0);
  readonly tongTrang = signal(0);
  readonly dangTai = signal(true);
  readonly loi = signal<string | null>(null);
  readonly chuDeList = signal<string[]>([]);
  readonly loiChuDe = signal(false);

  // Danh sách nút trang để render, có rút gọn bằng "…" khi nhiều trang - null là
  // dấu ba chấm, không phải số trang thật (giữ trang đầu/cuối + lân cận trang hiện tại).
  readonly cacTrang = computed<(number | null)[]>(() => {
    const tong = this.tongTrang();
    if (tong <= 7) return Array.from({ length: tong }, (_, i) => i + 1);

    const hienTai = this.boLoc().page ?? 1;
    const canGiu = new Set<number>([1, tong, hienTai - 1, hienTai, hienTai + 1]);
    const dsSapXep = [...canGiu].filter(t => t >= 1 && t <= tong).sort((a, b) => a - b);

    const ketQua: (number | null)[] = [];
    let trangTruoc = 0;
    for (const t of dsSapXep) {
      if (trangTruoc && t - trangTruoc > 1) ketQua.push(null);
      ketQua.push(t);
      trangTruoc = t;
    }
    return ketQua;
  });

  readonly boLoc = signal<BlogFilterRequest>({ page: 1, pageSize: 9 });
  tuKhoaNhap = '';

  private readonly thuLai$ = new Subject<void>();
  private readonly taiLaiChuDe$ = new Subject<void>();

  ngOnInit(): void {
    // switchMap: bấm "Retry" liên tục (mạng chậm) chỉ giữ lại kết quả của lần gọi cuối,
    // tránh phản hồi cũ về sau đè lên phản hồi mới hơn (giống luồng tìm kiếm chính bên dưới).
    merge(of(undefined), this.taiLaiChuDe$).pipe(
      tap(() => this.loiChuDe.set(false)),
      switchMap(() => this.blogService.layChuDeList().pipe(
        catchError(() => {
          this.loiChuDe.set(true);
          return of([] as string[]);
        })
      )),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(ds => {
      // new Set(...): phòng hờ nếu dữ liệu có 2 chủ đề trùng nhau (khác biệt
      // hoa/thường tùy collation CSDL) - @for track cd sẽ crash NG0955 nếu
      // trùng giá trị track, dù backend đã Distinct() ở tầng SQL.
      this.chuDeList.set([...new Set(ds)]);
    });

    // Đọc query param mỗi khi điều hướng đổi (đổi trang/tìm kiếm/chọn chủ đề).
    // Gộp thêm thuLai$ (nút "Retry") vào cùng luồng bằng merge() để dùng
    // chung 1 switchMap - bấm Retry sẽ chạy lại đúng bộ lọc hiện tại mà
    // không cần đổi URL.
    const doiTuTrangUrl$ = this.route.queryParamMap.pipe(
      tap(params => {
        const boLocMoi: BlogFilterRequest = {
          tuKhoa: params.get('tuKhoa') ?? undefined,
          chuDe: params.get('chuDe') ?? undefined,
          page: this.docTrangHopLe(params.get('page')),
          pageSize: 9
        };
        this.boLoc.set(boLocMoi);
        this.tuKhoaNhap = boLocMoi.tuKhoa ?? '';
      })
    );

    // switchMap: điều hướng/reload mới tự hủy request tìm kiếm đang chờ trước
    // đó - tránh phản hồi trả về không đúng thứ tự đè lên danh sách/trạng thái
    // phân trang hiện tại trên màn hình.
    merge(doiTuTrangUrl$, this.thuLai$).pipe(
      tap(() => {
        this.dangTai.set(true);
        this.loi.set(null);
      }),
      switchMap(() => this.blogService.search(this.boLoc()).pipe(
        catchError(() => {
          this.loi.set('Failed to load articles. Please try again.');
          this.dangTai.set(false);
          return EMPTY;
        })
      )),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(ket => {
      this.baiVietList.set(ket.items.map(bv => ({ ...bv, trichDoanNoiDung: this.trichDoan(bv.noiDung) })));
      this.tongSo.set(ket.totalCount);
      this.tongTrang.set(ket.totalPages);
      this.dangTai.set(false);

      // Backend tự kẹp về trang cuối hợp lệ khi URL trỏ tới trang không còn tồn tại
      // (VD bookmark cũ, hoặc lọc theo chủ đề làm giảm tổng số trang) và trả luôn dữ
      // liệu đúng của trang đó trong cùng response - chỉ cần đồng bộ lại state hiển thị
      // (đánh dấu trang đang chọn, bật/tắt nút Next) theo trang thật sự nhận được, không
      // gọi lại API lần 2 vì dữ liệu đã đúng rồi.
      if (ket.pageNumber !== (this.boLoc().page ?? 1)) {
        const boLocMoi = { ...this.boLoc(), page: ket.pageNumber };
        this.boLoc.set(boLocMoi);

        // Location.replaceState (không phải router.navigate) - chỉ sửa URL trên thanh
        // địa chỉ và lịch sử trình duyệt cho khớp trang thật, KHÔNG phát sinh điều hướng
        // qua Router nên route.queryParamMap không emit lại và không gọi thêm API lần 2.
        const urlTree = this.router.createUrlTree([], {
          relativeTo: this.route,
          queryParams: this.thanhQueryParams(boLocMoi)
        });
        this.location.replaceState(this.router.serializeUrl(urlTree));
      }
    });
  }

  thuLai(): void {
    this.thuLai$.next();
  }

  taiChuDeList(): void {
    this.taiLaiChuDe$.next();
  }

  timKiem(): void {
    this.apDungBoLoc({ tuKhoa: this.tuKhoaNhap.trim() || undefined });
  }

  chonChuDe(chuDe: string | null): void {
    this.apDungBoLoc({ chuDe: chuDe ?? undefined });
  }

  xoaLoc(): void {
    this.tuKhoaNhap = '';
    this.router.navigate([], { relativeTo: this.route, queryParams: {} });
  }

  doiTrang(trang: number): void {
    if (trang < 1 || trang > this.tongTrang()) return;
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: this.thanhQueryParams({ ...this.boLoc(), page: trang })
    });
  }

  private trichDoan(noiDung: string, doDai = 140): string {
    const sach = noiDung.replace(/\s+/g, ' ').trim();
    return sach.length > doDai ? sach.slice(0, doDai) + '…' : sach;
  }

  // Tránh gửi thẳng giá trị page từ URL (có thể bị sửa tay/hỏng, VD ?page=abc
  // -> NaN) xuống backend gây lỗi 400 - ép về số nguyên hợp lệ >= 1 trước.
  private docTrangHopLe(raw: string | null): number {
    const so = Number(raw);
    return Number.isInteger(so) && so >= 1 && so <= Blog.PAGE_TOI_DA ? so : 1;
  }

  private apDungBoLoc(thayDoi: Partial<BlogFilterRequest>): void {
    const moi = { ...this.boLoc(), ...thayDoi, page: 1 };
    this.router.navigate([], { relativeTo: this.route, queryParams: this.thanhQueryParams(moi) });
  }

  private thanhQueryParams(loc: BlogFilterRequest): Record<string, string | number> {
    const params: Record<string, string | number> = {};
    if (loc.tuKhoa) params['tuKhoa'] = loc.tuKhoa;
    if (loc.chuDe) params['chuDe'] = loc.chuDe;
    if (loc.page && loc.page > 1) params['page'] = loc.page;
    return params;
  }
}
