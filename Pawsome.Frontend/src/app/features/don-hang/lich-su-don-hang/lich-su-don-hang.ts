import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ChatAi } from '../../../shared/components/chat-ai/chat-ai';
import { Footer } from '../../../shared/components/footer/footer';
import { Header } from '../../../shared/components/header/header';
import { Order } from '../thanh-toan/models/thanh-toan.model';
import { OrderService } from '../thanh-toan/services/order.service';
import {
  ORDER_STATUS_FILTERS, OrderStatus, canCancelOrder, canRequestReturn,
  orderStatusCssClass, orderStatusLabel
} from './models/lich-su-don-hang.model';

type ActionType = 'cancel' | 'return';

@Component({
  selector: 'app-lich-su-don-hang',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, Header, Footer, ChatAi],
  templateUrl: './lich-su-don-hang.html',
  styleUrl: './lich-su-don-hang.css'
})
export class LichSuDonHangComponent {
  private readonly orderService = inject(OrderService);

  readonly statusFilters = ORDER_STATUS_FILTERS;
  readonly orderStatusLabel = orderStatusLabel;
  readonly orderStatusCssClass = orderStatusCssClass;
  readonly canCancelOrder = canCancelOrder;
  readonly canRequestReturn = canRequestReturn;

  readonly orders = signal<Order[]>([]);
  readonly loading = signal(true);
  readonly errorMessage = signal<string | null>(null);

  readonly selectedStatus = signal<OrderStatus | ''>('');
  readonly page = signal(1);
  readonly pageSize = 5;
  readonly totalCount = signal(0);
  readonly totalPages = signal(1);

  readonly expandedOrderId = signal<number | null>(null);

  readonly activeAction = signal<{ orderId: number; type: ActionType } | null>(null);
  reasonText = '';
  readonly submittingAction = signal(false);
  readonly actionError = signal<string | null>(null);

  readonly hasOrders = computed(() => this.orders().length > 0);

  constructor() {
    this.loadOrders();
  }

  private loadOrders(): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    this.orderService
      .getMyOrders({ trangThai: this.selectedStatus() || undefined, page: this.page(), pageSize: this.pageSize })
      .subscribe({
        next: result => {
          this.orders.set(result.items);
          this.totalCount.set(result.totalCount);
          this.totalPages.set(Math.max(1, result.totalPages));
          this.loading.set(false);
        },
        error: () => {
          this.errorMessage.set('Could not load your orders. Please try again.');
          this.loading.set(false);
        }
      });
  }

  selectStatus(status: OrderStatus | ''): void {
    if (this.selectedStatus() === status) return;
    this.selectedStatus.set(status);
    this.page.set(1);
    this.loadOrders();
  }

  goToPage(p: number): void {
    if (p < 1 || p > this.totalPages() || p === this.page()) return;
    this.page.set(p);
    this.loadOrders();
  }

  toggleExpand(orderId: number): void {
    this.expandedOrderId.set(this.expandedOrderId() === orderId ? null : orderId);
  }

  openAction(orderId: number, type: ActionType): void {
    this.activeAction.set({ orderId, type });
    this.reasonText = '';
    this.actionError.set(null);
  }

  closeAction(): void {
    this.activeAction.set(null);
    this.reasonText = '';
    this.actionError.set(null);
  }

  confirmAction(): void {
    const action = this.activeAction();
    if (!action) return;

    if (action.type === 'return' && !this.reasonText.trim()) {
      this.actionError.set('Please tell us why you want to return this order.');
      return;
    }

    this.submittingAction.set(true);
    this.actionError.set(null);

    const request$ = action.type === 'cancel'
      ? this.orderService.cancel(action.orderId, { lyDo: this.reasonText.trim() || undefined })
      : this.orderService.requestReturn(action.orderId, { lyDo: this.reasonText.trim() });

    request$.subscribe({
      next: updated => {
        this.submittingAction.set(false);
        this.orders.set(this.orders().map(o => (o.orderId === updated.orderId ? updated : o)));
        this.closeAction();
      },
      error: err => {
        this.submittingAction.set(false);
        this.actionError.set(err?.error?.message ?? 'Something went wrong. Please try again.');
      }
    });
  }
}