import { DecimalPipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TokenService } from '../../../core/models/token.service';
import { ChatAi } from '../../../shared/components/chat-ai/chat-ai';
import { Footer } from '../../../shared/components/footer/footer';
import { Header } from '../../../shared/components/header/header';
import { BoLocSanPham, DanhMuc, SanPham, ThuongHieu, TinhTrangSucKhoe, dichTenDanhMuc, giaVipPlaceholder, phanTramSao } from '../models/san-pham.model';
import { BrandService } from '../services/brand.service';
import { CategoryService } from '../services/category.service';
import { ConditionService } from '../services/condition.service';
import { ProductService } from '../services/product.service';
import { CartService } from '../../gio-hang/gio-hang/services/cart.service';

@Component({
  selector: 'app-danh-sach-san-pham',
  imports: [Header, Footer, ChatAi, RouterLink, FormsModule, DecimalPipe],
  templateUrl: './danh-sach-san-pham.html',
  styleUrl: './danh-sach-san-pham.scss'
})
export class DanhSachSanPham implements OnInit {
  protected readonly Math = Math;
  protected readonly Array = Array;
  protected readonly dichTenDanhMuc = dichTenDanhMuc;
  protected readonly giaVipPlaceholder = giaVipPlaceholder;
  protected readonly phanTramSao = phanTramSao;
  readonly saoArr = [1, 2, 3, 4, 5];

  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly productService = inject(ProductService);
  private readonly categoryService = inject(CategoryService);
  private readonly brandService = inject(BrandService);
  private readonly conditionService = inject(ConditionService);
  private readonly cartService = inject(CartService);
  private readonly tokenService = inject(TokenService);

  readonly sanPhamList = signal<SanPham[]>([]);
  readonly tongSo = signal(0);
  readonly tongTrang = signal(0);
  readonly dangTai = signal(true);

  readonly danhMucList = signal<DanhMuc[]>([]);
  readonly thuongHieuList = signal<ThuongHieu[]>([]);
  readonly tinhTrangList = signal<TinhTrangSucKhoe[]>([]);

  readonly boLoc = signal<BoLocSanPham>({ page: 1, pageSize: 12 });

  ngOnInit(): void {
    this.categoryService.getAll().subscribe(ds => this.danhMucList.set(ds));
    this.brandService.getAll().subscribe(ds => this.thuongHieuList.set(ds));
    this.conditionService.getAll().subscribe(ds => this.tinhTrangList.set(ds));

    this.route.queryParamMap.subscribe(params => {
      this.boLoc.set({
        tuKhoa: params.get('tuKhoa') ?? undefined,
        categoryId: params.get('categoryId') ? Number(params.get('categoryId')) : undefined,
        brandId: params.get('brandId') ? Number(params.get('brandId')) : undefined,
        giaMin: params.get('giaMin') ? Number(params.get('giaMin')) : undefined,
        giaMax: params.get('giaMax') ? Number(params.get('giaMax')) : undefined,
        danhGiaMin: params.get('danhGiaMin') ? Number(params.get('danhGiaMin')) : undefined,
        conditionId: params.get('conditionId') ? Number(params.get('conditionId')) : undefined,
        page: params.get('page') ? Number(params.get('page')) : 1,
        pageSize: 12
      });
      this.taiSanPham();
    });
  }

  private taiSanPham(): void {
    this.dangTai.set(true);
    this.productService.search(this.boLoc()).subscribe(ket => {
      this.sanPhamList.set(ket.items);
      this.tongSo.set(ket.totalCount);
      this.tongTrang.set(ket.totalPages);
      this.dangTai.set(false);
    });
  }

  apDungBoLoc(thayDoi: Partial<BoLocSanPham>): void {
    const moi = { ...this.boLoc(), ...thayDoi, page: 1 };
    this.router.navigate([], { relativeTo: this.route, queryParams: this.thanhQueryParams(moi) });
  }

  doiTrang(trang: number): void {
    if (trang < 1 || trang > this.tongTrang()) return;
    const moi = { ...this.boLoc(), page: trang };
    this.router.navigate([], { relativeTo: this.route, queryParams: this.thanhQueryParams(moi) });
  }

  xoaLoc(): void {
    this.router.navigate([], { relativeTo: this.route, queryParams: {} });
  }

  readonly dangThemGioNhanh = signal(false);

  // Nút "ADD TO CART" trên thẻ sản phẩm - thêm thẳng vào giỏ hàng thay vì chỉ điều hướng như trước
  // (cả thẻ đã là 1 thẻ <a> bọc ngoài để bấm ảnh/tên vào trang chi tiết, nên nút bên trong phải
  // preventDefault + stopPropagation để không bị điều hướng theo click cha). Tự chọn biến thể đầu
  // tiên còn hàng - trang danh sách không có chỗ để khách chọn biến thể cụ thể.
  themVaoGioNhanh(sp: SanPham, event: Event): void {
    event.preventDefault();
    event.stopPropagation();

    if (this.dangThemGioNhanh()) return;

    if (!this.tokenService.isLoggedIn()) {
      this.router.navigate(['/tai-khoan/dang-nhap']);
      return;
    }

    const bienThe = sp.variants.find(v => v.dangKinhDoanh && v.soLuongTon > 0);
    if (!bienThe) {
      alert('This product is currently out of stock.');
      return;
    }

    this.dangThemGioNhanh.set(true);
    this.cartService.themSanPham({ variantId: bienThe.variantId, soLuong: 1 }).subscribe({
      next: () => {
        this.dangThemGioNhanh.set(false);
      },
      error: err => {
        this.dangThemGioNhanh.set(false);
        alert(err?.error?.message ?? 'Failed to add to cart.');
      }
    });
  }

  diToiPawVip(event: Event): void {
    event.preventDefault();
    event.stopPropagation();
    this.router.navigate(['/gio-hang/pawvip']);
  }

  private thanhQueryParams(loc: BoLocSanPham): Record<string, string | number> {
    const params: Record<string, string | number> = {};
    if (loc.tuKhoa) params['tuKhoa'] = loc.tuKhoa;
    if (loc.categoryId) params['categoryId'] = loc.categoryId;
    if (loc.brandId) params['brandId'] = loc.brandId;
    if (loc.giaMin) params['giaMin'] = loc.giaMin;
    if (loc.giaMax) params['giaMax'] = loc.giaMax;
    if (loc.danhGiaMin) params['danhGiaMin'] = loc.danhGiaMin;
    if (loc.conditionId) params['conditionId'] = loc.conditionId;
    if (loc.page && loc.page > 1) params['page'] = loc.page;
    return params;
  }
}