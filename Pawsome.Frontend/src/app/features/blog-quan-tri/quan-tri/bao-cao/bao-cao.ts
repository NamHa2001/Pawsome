import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { orderStatusCssClass, orderStatusLabel } from '../../../don-hang/lich-su-don-hang/models/lich-su-don-hang.model';
import { DanhMuc, dichTenDanhMuc, ThuongHieu } from '../../../san-pham/models/san-pham.model';
import { BrandService } from '../../../san-pham/services/brand.service';
import { CategoryService } from '../../../san-pham/services/category.service';
import {
  DoanhThuTheoDanhMuc,
  DoanhThuTheoKy,
  DoanhThuTheoThuongHieu,
  DonHangTheoTrangThai,
  LoaiXuatCsv,
  NhomTheoThoiGian,
  ReportDateFilter,
  SanPhamBanChay,
  SanPhamReportFilter,
  SapXepSanPham
} from './models/bao-cao.model';
import { ReportService } from './services/report.service';

const PAGE_SIZE_SP = 10;

type TabBaoCao = 'doanh-thu' | 'don-hang' | 'san-pham' | 'danh-muc-thuong-hieu';

function dinhDangNgay(d: Date): string {
  const nam = d.getFullYear();
  const thang = String(d.getMonth() + 1).padStart(2, '0');
  const ngay = String(d.getDate()).padStart(2, '0');
  return `${nam}-${thang}-${ngay}`;
}

@Component({
  selector: 'app-quan-tri-bao-cao',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './bao-cao.html',
  styleUrl: './bao-cao.scss'
})
export class QuanTriBaoCao {
  private readonly reportService = inject(ReportService);
  private readonly categoryService = inject(CategoryService);
  private readonly brandService = inject(BrandService);

  readonly dichTen = dichTenDanhMuc;
  readonly orderStatusLabel = orderStatusLabel;
  readonly orderStatusCssClass = orderStatusCssClass;

  // ── Tab hiển thị (mỗi lần chỉ hiện 1 mục thay vì dồn hết vào 1 trang cuộn dài) ──
  readonly TABS: { id: TabBaoCao; nhan: string }[] = [
    { id: 'doanh-thu', nhan: 'Revenue' },
    { id: 'don-hang', nhan: 'Orders' },
    { id: 'san-pham', nhan: 'Products' },
    { id: 'danh-muc-thuong-hieu', nhan: 'Categories & Brands' }
  ];
  readonly tab = signal<TabBaoCao>('doanh-thu');

  // ── Bộ lọc chung (ngày) ──────────────────────────────────────────────
  readonly tuNgay = signal('');
  readonly denNgay = signal('');
  readonly nhom = signal<NhomTheoThoiGian>('Ngay');

  // ── Doanh thu theo thời gian ─────────────────────────────────────────
  readonly doanhThu = signal<DoanhThuTheoKy[]>([]);
  readonly dangTaiDoanhThu = signal(true);
  readonly loiDoanhThu = signal<string | null>(null);
  readonly tongDoanhThu = computed(() => this.doanhThu().reduce((s, d) => s + d.doanhThu, 0));
  readonly tongSoDon = computed(() => this.doanhThu().reduce((s, d) => s + d.soDon, 0));
  readonly maxDoanhThu = computed(() => Math.max(1, ...this.doanhThu().map(d => d.doanhThu)));

  // ── Đơn hàng theo trạng thái ─────────────────────────────────────────
  readonly donHangTheoTrangThai = signal<DonHangTheoTrangThai[]>([]);
  readonly dangTaiDonHang = signal(true);
  readonly loiDonHang = signal<string | null>(null);

  // ── Sản phẩm bán chạy (có lọc + phân trang) ──────────────────────────
  readonly categories = signal<DanhMuc[]>([]);
  readonly brands = signal<ThuongHieu[]>([]);
  readonly categoryIdLoc = signal<number | null>(null);
  readonly brandIdLoc = signal<number | null>(null);
  readonly sapXepTheo = signal<SapXepSanPham>('SoLuongBan');

  readonly sanPham = signal<SanPhamBanChay[]>([]);
  readonly trangSP = signal(1);
  readonly tongTrangSP = signal(0);
  readonly dangTaiSP = signal(true);
  readonly loiSP = signal<string | null>(null);
  readonly cacTrangSP = computed(() => Array.from({ length: this.tongTrangSP() }, (_, i) => i + 1));

  // ── Doanh thu theo danh mục / thương hiệu ────────────────────────────
  readonly danhMuc = signal<DoanhThuTheoDanhMuc[]>([]);
  readonly dangTaiDanhMuc = signal(true);
  readonly loiDanhMuc = signal<string | null>(null);

  readonly thuongHieu = signal<DoanhThuTheoThuongHieu[]>([]);
  readonly dangTaiThuongHieu = signal(true);
  readonly loiThuongHieu = signal<string | null>(null);

  constructor() {
    const homNay = new Date();
    const dauThang = new Date(homNay.getFullYear(), homNay.getMonth(), 1);
    this.tuNgay.set(dinhDangNgay(dauThang));
    this.denNgay.set(dinhDangNgay(homNay));

    this.categoryService.getAll().subscribe(ds => this.categories.set(ds));
    this.brandService.getAll().subscribe(ds => this.brands.set(ds));

    this.taiTatCa();
  }

  private filterNgay(): ReportDateFilter {
    return { tuNgay: this.tuNgay() || undefined, denNgay: this.denNgay() || undefined };
  }

  private filterSanPham(): SanPhamReportFilter {
    return {
      ...this.filterNgay(),
      categoryId: this.categoryIdLoc() ?? undefined,
      brandId: this.brandIdLoc() ?? undefined,
      page: this.trangSP(),
      pageSize: PAGE_SIZE_SP,
      sapXepTheo: this.sapXepTheo()
    };
  }

  apDungBoLoc(): void {
    this.trangSP.set(1);
    this.taiTatCa();
  }

  private taiTatCa(): void {
    this.taiDoanhThu();
    this.taiDonHangTheoTrangThai();
    this.taiSanPham();
    this.taiDanhMuc();
    this.taiThuongHieu();
  }

  doiNhom(nhom: NhomTheoThoiGian): void {
    this.nhom.set(nhom);
    this.taiDoanhThu();
  }

  private taiDoanhThu(): void {
    this.dangTaiDoanhThu.set(true);
    this.loiDoanhThu.set(null);

    this.reportService.layDoanhThu(this.filterNgay(), this.nhom()).subscribe({
      next: ket => { this.doanhThu.set(ket); this.dangTaiDoanhThu.set(false); },
      error: () => { this.loiDoanhThu.set('Failed to load revenue report.'); this.dangTaiDoanhThu.set(false); }
    });
  }

  private taiDonHangTheoTrangThai(): void {
    this.dangTaiDonHang.set(true);
    this.loiDonHang.set(null);

    this.reportService.layDonHangTheoTrangThai(this.filterNgay()).subscribe({
      next: ket => { this.donHangTheoTrangThai.set(ket); this.dangTaiDonHang.set(false); },
      error: () => { this.loiDonHang.set('Failed to load order status report.'); this.dangTaiDonHang.set(false); }
    });
  }

  doiLocDanhMucSP(categoryId: string): void {
    this.categoryIdLoc.set(categoryId ? Number(categoryId) : null);
    this.trangSP.set(1);
    this.taiSanPham();
  }

  doiLocThuongHieuSP(brandId: string): void {
    this.brandIdLoc.set(brandId ? Number(brandId) : null);
    this.trangSP.set(1);
    this.taiSanPham();
  }

  doiSapXepSP(sapXep: SapXepSanPham): void {
    this.sapXepTheo.set(sapXep);
    this.trangSP.set(1);
    this.taiSanPham();
  }

  doiTrangSP(trang: number): void {
    if (trang < 1 || trang > this.tongTrangSP()) return;
    this.trangSP.set(trang);
    this.taiSanPham();
  }

  private taiSanPham(): void {
    this.dangTaiSP.set(true);
    this.loiSP.set(null);

    this.reportService.laySanPham(this.filterSanPham()).subscribe({
      next: ket => {
        this.sanPham.set(ket.items);
        this.tongTrangSP.set(ket.totalPages);
        this.trangSP.set(ket.pageNumber);
        this.dangTaiSP.set(false);
      },
      error: () => { this.loiSP.set('Failed to load product report.'); this.dangTaiSP.set(false); }
    });
  }

  private taiDanhMuc(): void {
    this.dangTaiDanhMuc.set(true);
    this.loiDanhMuc.set(null);

    this.reportService.layDanhMuc(this.filterNgay()).subscribe({
      next: ket => { this.danhMuc.set(ket); this.dangTaiDanhMuc.set(false); },
      error: () => { this.loiDanhMuc.set('Failed to load category report.'); this.dangTaiDanhMuc.set(false); }
    });
  }

  private taiThuongHieu(): void {
    this.dangTaiThuongHieu.set(true);
    this.loiThuongHieu.set(null);

    this.reportService.layThuongHieu(this.filterNgay()).subscribe({
      next: ket => { this.thuongHieu.set(ket); this.dangTaiThuongHieu.set(false); },
      error: () => { this.loiThuongHieu.set('Failed to load brand report.'); this.dangTaiThuongHieu.set(false); }
    });
  }

  xuatCsv(loai: LoaiXuatCsv): void {
    this.reportService.xuatCsv(loai, this.filterSanPham(), this.nhom());
  }
}
