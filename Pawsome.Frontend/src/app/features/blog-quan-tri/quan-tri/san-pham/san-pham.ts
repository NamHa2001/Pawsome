import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import {
  BienTheSanPham, DanhMuc, dichTenDanhMuc, HinhAnhSanPham, SanPham, ThuongHieu, TinhTrangSucKhoe
} from '../../../san-pham/models/san-pham.model';
import { BrandService } from '../../../san-pham/services/brand.service';
import { CategoryService } from '../../../san-pham/services/category.service';
import { ConditionService } from '../../../san-pham/services/condition.service';
import { ProductService } from '../../../san-pham/services/product.service';
import { BrandRequest, CategoryRequest } from './models/quan-tri-san-pham.model';
import { AdminProductService } from './services/admin-product.service';

const PAGE_SIZE = 10;

// Khu Admin dùng tiếng Anh (xem trang-chu.html/danh-sach-san-pham.ts đã dịch tên 4 danh mục
// gốc qua dichTenDanhMuc) - nhưng dichTenDanhMuc không dịch được phần mô tả tự do. Mô tả 4
// danh mục gốc là dữ liệu seed cố định (Pawsome_Database.sql), không phải nội dung người
// dùng nhập tự do, nên dịch cứng luôn ở đây cho khớp giao diện Admin, không đụng tới dữ liệu
// gốc trong CSDL (khách hàng/trang công khai vẫn thấy tiếng Việt như cũ).
const MO_TA_DANH_MUC_TIENG_ANH: Record<string, string> = {
  'Sản phẩm chăm sóc sức khỏe cho chó': 'Health care products for dogs',
  'Sản phẩm chăm sóc sức khỏe cho mèo': 'Health care products for cats',
  'Sản phẩm chăm sóc sức khỏe cho ngựa': 'Health care products for horses',
  'Sản phẩm chăm sóc sức khỏe cho chim': 'Health care products for birds'
};

function dichMoTaDanhMuc(moTa: string): string {
  return MO_TA_DANH_MUC_TIENG_ANH[moTa] ?? moTa;
}

interface ProductFormState {
  ten: string;
  moTa: string;
  categoryId: number | null;
  brandId: number | null;
  lieuLuong: string;
  conditionIds: number[];
}

const PRODUCT_FORM_MAC_DINH: ProductFormState = {
  ten: '', moTa: '', categoryId: null, brandId: null, lieuLuong: '', conditionIds: []
};

interface VariantFormRow {
  tenBienThe: string;
  gia: number;
  soLuongTon: number;
  sku: string;
}

const VARIANT_FORM_MAC_DINH: VariantFormRow = { tenBienThe: '', gia: 0, soLuongTon: 0, sku: '' };

interface ImageFormRow {
  url: string;
  laAnhChinh: boolean;
}

interface CategoryFormState {
  tenDanhMuc: string;
  danhMucChaId: number | null;
  moTa: string;
}

const CATEGORY_FORM_MAC_DINH: CategoryFormState = { tenDanhMuc: '', danhMucChaId: null, moTa: '' };

interface BrandFormState {
  tenThuongHieu: string;
  logoUrl: string;
}

const BRAND_FORM_MAC_DINH: BrandFormState = { tenThuongHieu: '', logoUrl: '' };

@Component({
  selector: 'app-quan-tri-san-pham',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './san-pham.html',
  styleUrl: './san-pham.scss'
})
export class QuanTriSanPham {
  private readonly productService = inject(ProductService);
  private readonly categoryService = inject(CategoryService);
  private readonly brandService = inject(BrandService);
  private readonly conditionService = inject(ConditionService);
  private readonly adminProductService = inject(AdminProductService);

  readonly tab = signal<'products' | 'categories' | 'brands'>('products');

  readonly dichTen = dichTenDanhMuc;
  readonly dichMoTa = dichMoTaDanhMuc;

  readonly categories = signal<DanhMuc[]>([]);
  readonly brands = signal<ThuongHieu[]>([]);
  readonly conditions = signal<TinhTrangSucKhoe[]>([]);

  // ── Danh sách sản phẩm ─────────────────────────────────────────────────
  readonly dsSanPham = signal<SanPham[]>([]);
  readonly dangTaiSP = signal(true);
  readonly loiSP = signal<string | null>(null);
  readonly trangSP = signal(1);
  readonly tongTrangSP = signal(0);
  readonly tongSoSP = signal(0);
  readonly tuKhoaSP = signal('');
  readonly locCategoryId = signal<number | null>(null);

  readonly cacTrangSP = computed(() => Array.from({ length: this.tongTrangSP() }, (_, i) => i + 1));

  // ── Form thêm/sửa sản phẩm ─────────────────────────────────────────────
  readonly formSPMode = signal<'them' | 'sua' | null>(null);
  readonly idDangSuaSP = signal<number | null>(null);
  readonly formSP = signal<ProductFormState>({ ...PRODUCT_FORM_MAC_DINH });
  readonly variantsForm = signal<VariantFormRow[]>([{ ...VARIANT_FORM_MAC_DINH }]);
  readonly imagesForm = signal<ImageFormRow[]>([]);
  readonly loiFormSP = signal<string | null>(null);
  readonly dangLuuSP = signal(false);
  readonly dangXoaSP = signal<number | null>(null);

  // ── Chế độ chi tiết (quản lý biến thể/ảnh của 1 sản phẩm) ─────────────
  readonly cheDo = signal<'danh-sach' | 'chi-tiet'>('danh-sach');
  readonly sanPhamChiTiet = signal<SanPham | null>(null);
  readonly dangTaiChiTiet = signal(false);
  readonly loiChiTiet = signal<string | null>(null);

  readonly dangMoThemBienThe = signal(false);
  readonly formThemBienThe = signal<VariantFormRow>({ ...VARIANT_FORM_MAC_DINH });
  readonly idDangSuaBienThe = signal<number | null>(null);
  readonly formSuaBienThe = signal<VariantFormRow>({ ...VARIANT_FORM_MAC_DINH });
  readonly loiBienThe = signal<string | null>(null);
  readonly dangLuuBienThe = signal(false);
  readonly dangAnBienThe = signal<number | null>(null);

  readonly dangMoThemAnh = signal(false);
  readonly formThemAnh = signal<ImageFormRow>({ url: '', laAnhChinh: false });
  readonly loiAnh = signal<string | null>(null);
  readonly dangLuuAnh = signal(false);
  readonly dangXoaAnh = signal<number | null>(null);

  readonly thongBao = signal<string | null>(null);

  // ── Danh mục ───────────────────────────────────────────────────────────
  readonly dangMoFormThemDM = signal(false);
  readonly formThemDM = signal<CategoryFormState>({ ...CATEGORY_FORM_MAC_DINH });
  readonly idDangSuaDM = signal<number | null>(null);
  readonly formSuaDM = signal<CategoryFormState>({ ...CATEGORY_FORM_MAC_DINH });
  readonly loiFormDM = signal<string | null>(null);
  readonly dangLuuDM = signal(false);
  readonly dangXoaDM = signal<number | null>(null);

  // ── Thương hiệu ────────────────────────────────────────────────────────
  readonly dangMoFormThemTH = signal(false);
  readonly formThemTH = signal<BrandFormState>({ ...BRAND_FORM_MAC_DINH });
  readonly idDangSuaTH = signal<number | null>(null);
  readonly formSuaTH = signal<BrandFormState>({ ...BRAND_FORM_MAC_DINH });
  readonly loiFormTH = signal<string | null>(null);
  readonly dangLuuTH = signal(false);
  readonly dangXoaTH = signal<number | null>(null);

  constructor() {
    this.taiDuLieuThamChieu();
    this.taiDanhSachSP();
  }

  private taiDuLieuThamChieu(): void {
    this.categoryService.getAll().subscribe(ds => this.categories.set(ds));
    this.brandService.getAll().subscribe(ds => this.brands.set(ds));
    this.conditionService.getAll().subscribe(ds => this.conditions.set(ds));
  }

  doiTab(tab: 'products' | 'categories' | 'brands'): void {
    this.tab.set(tab);
    this.thongBao.set(null);
  }

  tenDanhMuc(categoryId: number): string {
    const ten = this.categories().find(c => c.categoryId === categoryId)?.tenDanhMuc;
    return ten ? dichTenDanhMuc(ten) : `#${categoryId}`;
  }

  tenThuongHieu(brandId: number | null): string {
    if (!brandId) return '—';
    return this.brands().find(b => b.brandId === brandId)?.tenThuongHieu ?? `#${brandId}`;
  }

  // ═══════════════════════ DANH SÁCH SẢN PHẨM ═══════════════════════════

  taiDanhSachSP(): void {
    this.dangTaiSP.set(true);
    this.loiSP.set(null);

    this.productService.search({
      tuKhoa: this.tuKhoaSP() || undefined,
      categoryId: this.locCategoryId() ?? undefined,
      page: this.trangSP(),
      pageSize: PAGE_SIZE
    }).subscribe({
      next: ket => {
        this.dsSanPham.set(ket.items);
        this.tongSoSP.set(ket.totalCount);
        this.tongTrangSP.set(ket.totalPages);
        this.trangSP.set(ket.pageNumber);
        this.dangTaiSP.set(false);
      },
      error: () => {
        this.loiSP.set('Failed to load products.');
        this.dangTaiSP.set(false);
      }
    });
  }

  timKiemSP(): void {
    this.trangSP.set(1);
    this.taiDanhSachSP();
  }

  doiLocDanhMuc(categoryId: string): void {
    this.locCategoryId.set(categoryId ? Number(categoryId) : null);
    this.trangSP.set(1);
    this.taiDanhSachSP();
  }

  doiTrangSP(trang: number): void {
    if (trang < 1 || trang > this.tongTrangSP()) return;
    this.trangSP.set(trang);
    this.taiDanhSachSP();
  }

  // ═══════════════════════ THÊM / SỬA SẢN PHẨM ══════════════════════════

  moFormThemSP(): void {
    this.formSPMode.set('them');
    this.idDangSuaSP.set(null);
    this.formSP.set({ ...PRODUCT_FORM_MAC_DINH });
    this.variantsForm.set([{ ...VARIANT_FORM_MAC_DINH }]);
    this.imagesForm.set([]);
    this.loiFormSP.set(null);
  }

  moFormSuaSP(sp: SanPham): void {
    this.formSPMode.set('sua');
    this.idDangSuaSP.set(sp.productId);
    this.formSP.set({
      ten: sp.ten,
      moTa: sp.moTa ?? '',
      categoryId: sp.categoryId,
      brandId: sp.brandId,
      lieuLuong: sp.lieuLuong ?? '',
      conditionIds: sp.conditions.map(c => c.conditionId)
    });
    this.loiFormSP.set(null);
  }

  dongFormSP(): void {
    this.formSPMode.set(null);
    this.loiFormSP.set(null);
  }

  toggleConditionForm(conditionId: number): void {
    this.formSP.update(f => {
      const co = f.conditionIds.includes(conditionId);
      return { ...f, conditionIds: co ? f.conditionIds.filter(id => id !== conditionId) : [...f.conditionIds, conditionId] };
    });
  }

  themDongBienThe(): void {
    this.variantsForm.update(ds => [...ds, { ...VARIANT_FORM_MAC_DINH }]);
  }

  xoaDongBienThe(index: number): void {
    this.variantsForm.update(ds => ds.filter((_, i) => i !== index));
  }

  capNhatDongBienThe(index: number, field: keyof VariantFormRow, value: string | number): void {
    this.variantsForm.update(ds => ds.map((v, i) => i === index ? { ...v, [field]: value } : v));
  }

  themDongAnh(): void {
    this.imagesForm.update(ds => [...ds, { url: '', laAnhChinh: ds.length === 0 }]);
  }

  xoaDongAnh(index: number): void {
    this.imagesForm.update(ds => ds.filter((_, i) => i !== index));
  }

  capNhatDongAnh(index: number, field: keyof ImageFormRow, value: string | boolean): void {
    this.imagesForm.update(ds => ds.map((img, i) => i === index ? { ...img, [field]: value } : img));
  }

  luuSP(): void {
    const f = this.formSP();
    if (!f.ten.trim()) { this.loiFormSP.set('Please enter a product name.'); return; }
    if (!f.categoryId) { this.loiFormSP.set('Please choose a category.'); return; }

    if (this.formSPMode() === 'them') {
      const cacBienThe = this.variantsForm();
      if (cacBienThe.length === 0 || cacBienThe.some(v => !v.tenBienThe.trim() || v.gia <= 0)) {
        this.loiFormSP.set('Each variant needs a name and a price greater than 0.');
        return;
      }

      this.dangLuuSP.set(true);
      this.adminProductService.taoSanPham({
        ten: f.ten.trim(),
        moTa: f.moTa.trim() || null,
        categoryId: f.categoryId,
        brandId: f.brandId,
        lieuLuong: f.lieuLuong.trim() || null,
        variants: cacBienThe.map(v => ({
          tenBienThe: v.tenBienThe.trim(), gia: v.gia, soLuongTon: v.soLuongTon, sku: v.sku.trim() || null
        })),
        images: this.imagesForm().filter(img => img.url.trim()).map(img => ({ url: img.url.trim(), laAnhChinh: img.laAnhChinh })),
        conditionIds: f.conditionIds
      }).subscribe({
        next: res => {
          this.dangLuuSP.set(false);
          this.formSPMode.set(null);
          this.thongBao.set(res.message ?? 'Product created.');
          this.taiDanhSachSP();
        },
        error: err => {
          this.dangLuuSP.set(false);
          this.loiFormSP.set(err?.error?.message ?? 'Failed to create product.');
        }
      });
    } else {
      const id = this.idDangSuaSP();
      if (!id) return;

      this.dangLuuSP.set(true);
      this.adminProductService.suaSanPham(id, {
        ten: f.ten.trim(),
        moTa: f.moTa.trim() || null,
        categoryId: f.categoryId,
        brandId: f.brandId,
        lieuLuong: f.lieuLuong.trim() || null,
        conditionIds: f.conditionIds
      }).subscribe({
        next: res => {
          this.dangLuuSP.set(false);
          this.formSPMode.set(null);
          this.thongBao.set(res.message ?? 'Product updated.');
          this.taiDanhSachSP();
        },
        error: err => {
          this.dangLuuSP.set(false);
          this.loiFormSP.set(err?.error?.message ?? 'Failed to update product.');
        }
      });
    }
  }

  xoaSP(sp: SanPham): void {
    if (!confirm(`Hide product "${sp.ten}"? It will no longer be visible to customers.`)) return;

    this.dangXoaSP.set(sp.productId);
    this.adminProductService.xoaSanPham(sp.productId).subscribe({
      next: () => {
        this.dangXoaSP.set(null);
        this.thongBao.set('Product hidden.');
        this.taiDanhSachSP();
      },
      error: err => {
        this.dangXoaSP.set(null);
        this.loiSP.set(err?.error?.message ?? 'Failed to hide product.');
      }
    });
  }

  // ═══════════════════ CHI TIẾT: BIẾN THỂ / ẢNH ═════════════════════════

  moChiTiet(sp: SanPham): void {
    this.cheDo.set('chi-tiet');
    this.taiChiTiet(sp.productId);
  }

  quayVeDanhSach(): void {
    this.cheDo.set('danh-sach');
    this.sanPhamChiTiet.set(null);
    this.taiDanhSachSP();
  }

  private taiChiTiet(productId: number): void {
    this.dangTaiChiTiet.set(true);
    this.loiChiTiet.set(null);

    this.productService.getById(productId).subscribe({
      next: sp => {
        this.sanPhamChiTiet.set(sp);
        this.dangTaiChiTiet.set(false);
      },
      error: () => {
        this.loiChiTiet.set('Failed to load product detail.');
        this.dangTaiChiTiet.set(false);
      }
    });
  }

  moThemBienThe(): void {
    this.dangMoThemBienThe.set(true);
    this.formThemBienThe.set({ ...VARIANT_FORM_MAC_DINH });
    this.loiBienThe.set(null);
  }

  dongThemBienThe(): void {
    this.dangMoThemBienThe.set(false);
    this.loiBienThe.set(null);
  }

  luuThemBienThe(): void {
    const sp = this.sanPhamChiTiet();
    const f = this.formThemBienThe();
    if (!sp) return;
    if (!f.tenBienThe.trim() || f.gia <= 0) { this.loiBienThe.set('Enter a variant name and a price greater than 0.'); return; }

    this.dangLuuBienThe.set(true);
    this.adminProductService.themBienThe(sp.productId, {
      tenBienThe: f.tenBienThe.trim(), gia: f.gia, soLuongTon: f.soLuongTon, sku: f.sku.trim() || null
    }).subscribe({
      next: () => {
        this.dangLuuBienThe.set(false);
        this.dangMoThemBienThe.set(false);
        this.taiChiTiet(sp.productId);
      },
      error: err => {
        this.dangLuuBienThe.set(false);
        this.loiBienThe.set(err?.error?.message ?? 'Failed to add variant.');
      }
    });
  }

  moSuaBienThe(v: BienTheSanPham): void {
    this.idDangSuaBienThe.set(v.variantId);
    this.formSuaBienThe.set({ tenBienThe: v.tenBienThe, gia: v.gia, soLuongTon: v.soLuongTon, sku: v.sku ?? '' });
    this.loiBienThe.set(null);
  }

  huySuaBienThe(): void {
    this.idDangSuaBienThe.set(null);
    this.loiBienThe.set(null);
  }

  luuSuaBienThe(): void {
    const sp = this.sanPhamChiTiet();
    const id = this.idDangSuaBienThe();
    const f = this.formSuaBienThe();
    if (!sp || !id) return;
    if (!f.tenBienThe.trim() || f.gia <= 0) { this.loiBienThe.set('Enter a variant name and a price greater than 0.'); return; }

    this.dangLuuBienThe.set(true);
    this.adminProductService.suaBienThe(sp.productId, id, {
      tenBienThe: f.tenBienThe.trim(), gia: f.gia, soLuongTon: f.soLuongTon, sku: f.sku.trim() || null
    }).subscribe({
      next: () => {
        this.dangLuuBienThe.set(false);
        this.idDangSuaBienThe.set(null);
        this.taiChiTiet(sp.productId);
      },
      error: err => {
        this.dangLuuBienThe.set(false);
        this.loiBienThe.set(err?.error?.message ?? 'Failed to update variant.');
      }
    });
  }

  anBienThe(v: BienTheSanPham): void {
    const sp = this.sanPhamChiTiet();
    if (!sp) return;
    if (!confirm(`Hide variant "${v.tenBienThe}"?`)) return;

    this.dangAnBienThe.set(v.variantId);
    this.adminProductService.xoaBienThe(sp.productId, v.variantId).subscribe({
      next: () => {
        this.dangAnBienThe.set(null);
        this.taiChiTiet(sp.productId);
      },
      error: err => {
        this.dangAnBienThe.set(null);
        this.loiChiTiet.set(err?.error?.message ?? 'Failed to hide variant.');
      }
    });
  }

  moThemAnh(): void {
    this.dangMoThemAnh.set(true);
    this.formThemAnh.set({ url: '', laAnhChinh: false });
    this.loiAnh.set(null);
  }

  dongThemAnh(): void {
    this.dangMoThemAnh.set(false);
    this.loiAnh.set(null);
  }

  luuThemAnh(): void {
    const sp = this.sanPhamChiTiet();
    const f = this.formThemAnh();
    if (!sp) return;
    if (!f.url.trim()) { this.loiAnh.set('Please enter the image URL.'); return; }

    this.dangLuuAnh.set(true);
    this.adminProductService.themAnh(sp.productId, { url: f.url.trim(), laAnhChinh: f.laAnhChinh }).subscribe({
      next: () => {
        this.dangLuuAnh.set(false);
        this.dangMoThemAnh.set(false);
        this.taiChiTiet(sp.productId);
      },
      error: err => {
        this.dangLuuAnh.set(false);
        this.loiAnh.set(err?.error?.message ?? 'Failed to add image.');
      }
    });
  }

  xoaAnh(img: HinhAnhSanPham): void {
    const sp = this.sanPhamChiTiet();
    if (!sp) return;
    if (!confirm('Delete this image?')) return;

    this.dangXoaAnh.set(img.imageId);
    this.adminProductService.xoaAnh(sp.productId, img.imageId).subscribe({
      next: () => {
        this.dangXoaAnh.set(null);
        this.taiChiTiet(sp.productId);
      },
      error: err => {
        this.dangXoaAnh.set(null);
        this.loiChiTiet.set(err?.error?.message ?? 'Failed to delete image.');
      }
    });
  }

  // ═══════════════════════════ DANH MỤC ══════════════════════════════════

  moFormThemDM(): void {
    this.formThemDM.set({ ...CATEGORY_FORM_MAC_DINH });
    this.loiFormDM.set(null);
    this.dangMoFormThemDM.set(true);
  }

  dongFormThemDM(): void {
    this.dangMoFormThemDM.set(false);
    this.loiFormDM.set(null);
  }

  luuThemDM(): void {
    const f = this.formThemDM();
    if (!f.tenDanhMuc.trim()) { this.loiFormDM.set('Please enter a category name.'); return; }

    const dto: CategoryRequest = { tenDanhMuc: f.tenDanhMuc.trim(), danhMucChaId: f.danhMucChaId, moTa: f.moTa.trim() || null };
    this.dangLuuDM.set(true);
    this.adminProductService.taoDanhMuc(dto).subscribe({
      next: res => {
        this.dangLuuDM.set(false);
        this.dangMoFormThemDM.set(false);
        this.thongBao.set(res.message ?? 'Category created.');
        this.taiDuLieuThamChieu();
      },
      error: err => {
        this.dangLuuDM.set(false);
        this.loiFormDM.set(err?.error?.message ?? 'Failed to create category.');
      }
    });
  }

  moFormSuaDM(dm: DanhMuc): void {
    this.idDangSuaDM.set(dm.categoryId);
    this.formSuaDM.set({ tenDanhMuc: dm.tenDanhMuc, danhMucChaId: dm.danhMucChaId, moTa: dm.moTa ?? '' });
    this.loiFormDM.set(null);
  }

  huySuaDM(): void {
    this.idDangSuaDM.set(null);
    this.loiFormDM.set(null);
  }

  luuSuaDM(id: number): void {
    const f = this.formSuaDM();
    if (!f.tenDanhMuc.trim()) { this.loiFormDM.set('Please enter a category name.'); return; }

    const dto: CategoryRequest = { tenDanhMuc: f.tenDanhMuc.trim(), danhMucChaId: f.danhMucChaId, moTa: f.moTa.trim() || null };
    this.dangLuuDM.set(true);
    this.adminProductService.suaDanhMuc(id, dto).subscribe({
      next: res => {
        this.dangLuuDM.set(false);
        this.idDangSuaDM.set(null);
        this.thongBao.set(res.message ?? 'Category updated.');
        this.taiDuLieuThamChieu();
      },
      error: err => {
        this.dangLuuDM.set(false);
        this.loiFormDM.set(err?.error?.message ?? 'Update failed.');
      }
    });
  }

  xoaDM(dm: DanhMuc): void {
    if (!confirm(`Delete category "${dm.tenDanhMuc}"?`)) return;

    this.dangXoaDM.set(dm.categoryId);
    this.adminProductService.xoaDanhMuc(dm.categoryId).subscribe({
      next: () => {
        this.dangXoaDM.set(null);
        this.thongBao.set('Category deleted.');
        this.taiDuLieuThamChieu();
      },
      error: err => {
        this.dangXoaDM.set(null);
        this.loiFormDM.set(err?.error?.message ?? 'Failed to delete category.');
      }
    });
  }

  // ═══════════════════════════ THƯƠNG HIỆU ══════════════════════════════

  moFormThemTH(): void {
    this.formThemTH.set({ ...BRAND_FORM_MAC_DINH });
    this.loiFormTH.set(null);
    this.dangMoFormThemTH.set(true);
  }

  dongFormThemTH(): void {
    this.dangMoFormThemTH.set(false);
    this.loiFormTH.set(null);
  }

  luuThemTH(): void {
    const f = this.formThemTH();
    if (!f.tenThuongHieu.trim()) { this.loiFormTH.set('Please enter a brand name.'); return; }

    const dto: BrandRequest = { tenThuongHieu: f.tenThuongHieu.trim(), logoUrl: f.logoUrl.trim() || null };
    this.dangLuuTH.set(true);
    this.adminProductService.taoThuongHieu(dto).subscribe({
      next: res => {
        this.dangLuuTH.set(false);
        this.dangMoFormThemTH.set(false);
        this.thongBao.set(res.message ?? 'Brand created.');
        this.taiDuLieuThamChieu();
      },
      error: err => {
        this.dangLuuTH.set(false);
        this.loiFormTH.set(err?.error?.message ?? 'Failed to create brand.');
      }
    });
  }

  moFormSuaTH(th: ThuongHieu): void {
    this.idDangSuaTH.set(th.brandId);
    this.formSuaTH.set({ tenThuongHieu: th.tenThuongHieu, logoUrl: th.logoUrl ?? '' });
    this.loiFormTH.set(null);
  }

  huySuaTH(): void {
    this.idDangSuaTH.set(null);
    this.loiFormTH.set(null);
  }

  luuSuaTH(id: number): void {
    const f = this.formSuaTH();
    if (!f.tenThuongHieu.trim()) { this.loiFormTH.set('Please enter a brand name.'); return; }

    const dto: BrandRequest = { tenThuongHieu: f.tenThuongHieu.trim(), logoUrl: f.logoUrl.trim() || null };
    this.dangLuuTH.set(true);
    this.adminProductService.suaThuongHieu(id, dto).subscribe({
      next: res => {
        this.dangLuuTH.set(false);
        this.idDangSuaTH.set(null);
        this.thongBao.set(res.message ?? 'Brand updated.');
        this.taiDuLieuThamChieu();
      },
      error: err => {
        this.dangLuuTH.set(false);
        this.loiFormTH.set(err?.error?.message ?? 'Update failed.');
      }
    });
  }

  xoaTH(th: ThuongHieu): void {
    if (!confirm(`Delete brand "${th.tenThuongHieu}"?`)) return;

    this.dangXoaTH.set(th.brandId);
    this.adminProductService.xoaThuongHieu(th.brandId).subscribe({
      next: () => {
        this.dangXoaTH.set(null);
        this.thongBao.set('Brand deleted.');
        this.taiDuLieuThamChieu();
      },
      error: err => {
        this.dangXoaTH.set(null);
        this.loiFormTH.set(err?.error?.message ?? 'Failed to delete brand.');
      }
    });
  }
}
