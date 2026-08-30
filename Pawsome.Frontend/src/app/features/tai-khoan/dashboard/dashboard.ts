import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Header } from '../../../shared/components/header/header';
import { Footer } from '../../../shared/components/footer/footer';
import { ChatAi } from '../../../shared/components/chat-ai/chat-ai';
import { TaiKhoanSidebarComponent } from '../tai-khoan-sidebar/tai-khoan-sidebar';
import { UserProfile, UserService } from '../user.service';
import { OrderService } from '../../don-hang/thanh-toan/services/order.service';
import { Order } from '../../don-hang/thanh-toan/models/thanh-toan.model';
import { orderStatusCssClass, orderStatusLabel } from '../../don-hang/lich-su-don-hang/models/lich-su-don-hang.model';
import { WishlistService } from '../../blog-quan-tri/wishlist/services/wishlist.service';
import { DANH_SACH_GOI_PAWVIP } from '../../gio-hang/pawvip/models/pawvip-goi.model';

const SO_DON_GAN_DAY = 3;

// Trang "Dashboard" khách hàng - trước đây chỉ là mục sidebar bị disable ghi "Coming soon",
// không có trang thật. Gộp lại các số liệu đã có sẵn API (không thêm bảng/logic nghiệp vụ mới,
// vì tính năng này không được đặc tả trong SRS - chỉ gộp hiển thị dữ liệu đã tồn tại).
@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, Header, Footer, ChatAi, TaiKhoanSidebarComponent],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class DashboardComponent {
  private readonly userService = inject(UserService);
  private readonly orderService = inject(OrderService);
  private readonly wishlistService = inject(WishlistService);

  readonly hoSo = signal<UserProfile | null>(null);
  readonly donHangGanDay = signal<Order[]>([]);
  readonly tongSoDon = signal(0);
  readonly soLuongWishlist = signal(0);
  readonly tongTietKiem = signal(0);
  readonly dangTai = signal(true);

  orderStatusLabel = orderStatusLabel;
  orderStatusCssClass = orderStatusCssClass;

  readonly goiPawVipHienTai = computed(() => {
    const tier = this.hoSo()?.pawVipTier;
    return tier ? DANH_SACH_GOI_PAWVIP.find(g => g.id === tier) : undefined;
  });

  constructor() {
    this.taiTatCa();
  }

  private taiTatCa(): void {
    this.dangTai.set(true);

    this.userService.getProfile().subscribe({
      next: res => this.hoSo.set(res.data ?? null),
      error: () => {}
    });

    this.orderService.getMyOrders({ page: 1, pageSize: SO_DON_GAN_DAY }).subscribe({
      next: ket => {
        this.donHangGanDay.set(ket.items);
        this.tongSoDon.set(ket.totalCount);
        this.dangTai.set(false);
      },
      error: () => this.dangTai.set(false)
    });

    this.wishlistService.layDanhSach(1, 1).subscribe({
      next: ket => this.soLuongWishlist.set(ket.totalCount),
      error: () => {}
    });

    this.orderService.tongTietKiem().subscribe({
      next: soTien => this.tongTietKiem.set(soTien),
      error: () => {}
    });
  }
}
