import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BlogPost, BlogPostRequest } from '../../blog/models/blog.model';
import { BlogService } from '../../blog/services/blog.service';

const PAGE_SIZE = 10;

interface BlogFormState {
  tieuDe: string;
  chuDe: string;
  anhDaiDien: string;
  noiDung: string;
}

const FORM_MAC_DINH: BlogFormState = { tieuDe: '', chuDe: '', anhDaiDien: '', noiDung: '' };

@Component({
  selector: 'app-quan-tri-blog',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './quan-tri-blog.html',
  styleUrl: './quan-tri-blog.scss'
})
export class QuanTriBlog {
  private readonly blogService = inject(BlogService);

  readonly danhSach = signal<BlogPost[]>([]);
  readonly dangTai = signal(true);
  readonly loi = signal<string | null>(null);
  readonly trang = signal(1);
  readonly tongTrang = signal(0);
  readonly tongSo = signal(0);
  readonly tuKhoa = signal('');
  readonly chuDeList = signal<string[]>([]);
  readonly thongBao = signal<string | null>(null);

  readonly cacTrang = computed(() => Array.from({ length: this.tongTrang() }, (_, i) => i + 1));

  // Add/edit form
  readonly formMode = signal<'them' | 'sua' | null>(null);
  readonly idDangSua = signal<number | null>(null);
  readonly form = signal<BlogFormState>({ ...FORM_MAC_DINH });
  readonly loiForm = signal<string | null>(null);
  readonly dangLuu = signal(false);
  readonly dangXoa = signal<number | null>(null);
  readonly dangUpload = signal(false);

  constructor() {
    this.blogService.layChuDeList().subscribe(ds => this.chuDeList.set(ds));
    this.taiDanhSach();
  }

  taiDanhSach(): void {
    this.dangTai.set(true);
    this.loi.set(null);

    this.blogService.search({
      tuKhoa: this.tuKhoa() || undefined,
      page: this.trang(),
      pageSize: PAGE_SIZE
    }).subscribe({
      next: ket => {
        this.danhSach.set(ket.items);
        this.tongSo.set(ket.totalCount);
        this.tongTrang.set(ket.totalPages);
        this.trang.set(ket.pageNumber);
        this.dangTai.set(false);
      },
      error: () => {
        this.loi.set('Failed to load articles.');
        this.dangTai.set(false);
      }
    });
  }

  timKiem(): void {
    this.trang.set(1);
    this.taiDanhSach();
  }

  doiTrang(trang: number): void {
    if (trang < 1 || trang > this.tongTrang()) return;
    this.trang.set(trang);
    this.taiDanhSach();
  }

  // ── Add / edit ──
  moFormThem(): void {
    this.formMode.set('them');
    this.idDangSua.set(null);
    this.form.set({ ...FORM_MAC_DINH });
    this.loiForm.set(null);
  }

  moFormSua(bv: BlogPost): void {
    this.formMode.set('sua');
    this.idDangSua.set(bv.postId);
    this.form.set({
      tieuDe: bv.tieuDe,
      chuDe: bv.chuDe ?? '',
      anhDaiDien: bv.anhDaiDien ?? '',
      noiDung: bv.noiDung
    });
    this.loiForm.set(null);
  }

  dongForm(): void {
    this.formMode.set(null);
    this.loiForm.set(null);
  }

  chonAnh(sk: Event): void {
    const input = sk.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    this.dangUpload.set(true);
    this.loiForm.set(null);

    this.blogService.uploadAnh(file).subscribe({
      next: url => {
        this.form.update(f => ({ ...f, anhDaiDien: url }));
        this.dangUpload.set(false);
      },
      error: err => {
        this.loiForm.set(err?.error?.message ?? 'Tải ảnh lên thất bại.');
        this.dangUpload.set(false);
      }
    });

    // Cho phép chọn lại đúng file cũ lần nữa (VD upload lỗi rồi thử lại) - nếu không
    // reset, input giữ nguyên giá trị nên (change) sẽ không bắn lại khi chọn lại y hệt file đó.
    input.value = '';
  }

  xoaAnh(): void {
    this.form.update(f => ({ ...f, anhDaiDien: '' }));
  }

  luu(): void {
    const f = this.form();
    if (!f.tieuDe.trim()) { this.loiForm.set('Please enter a title.'); return; }
    if (!f.noiDung.trim()) { this.loiForm.set('Please enter the article content.'); return; }

    const dto: BlogPostRequest = {
      tieuDe: f.tieuDe.trim(),
      noiDung: f.noiDung.trim(),
      chuDe: f.chuDe.trim() || null,
      anhDaiDien: f.anhDaiDien.trim() || null
    };

    this.dangLuu.set(true);

    const luu$ = this.formMode() === 'them'
      ? this.blogService.taoBaiViet(dto)
      : this.blogService.suaBaiViet(this.idDangSua()!, dto);

    luu$.subscribe({
      next: res => {
        this.dangLuu.set(false);
        this.formMode.set(null);
        this.thongBao.set(res.message ?? 'Saved.');
        this.blogService.layChuDeList().subscribe(ds => this.chuDeList.set(ds));
        this.taiDanhSach();
      },
      error: err => {
        this.dangLuu.set(false);
        this.loiForm.set(err?.error?.message ?? 'Failed to save the article.');
      }
    });
  }

  xoa(bv: BlogPost): void {
    if (!confirm(`Delete article "${bv.tieuDe}"? This cannot be undone.`)) return;

    this.dangXoa.set(bv.postId);
    this.blogService.xoaBaiViet(bv.postId).subscribe({
      next: () => {
        this.dangXoa.set(null);
        this.thongBao.set('Article deleted.');
        this.taiDanhSach();
      },
      error: err => {
        this.dangXoa.set(null);
        this.loi.set(err?.error?.message ?? 'Failed to delete the article.');
      }
    });
  }
}
