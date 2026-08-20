import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ChatAi } from '../../../shared/components/chat-ai/chat-ai';
import { Footer } from '../../../shared/components/footer/footer';
import { Header } from '../../../shared/components/header/header';
import { orderStatusLabel } from '../lich-su-don-hang/models/lich-su-don-hang.model';
import { Order } from '../thanh-toan/models/thanh-toan.model';
import { OrderService } from '../thanh-toan/services/order.service';

type TrackingStep = 'cho_xu_ly' | 'dang_xu_ly' | 'da_giao_van' | 'da_giao';

const TRACKING_STEPS: { value: TrackingStep; label: string }[] = [
  { value: 'cho_xu_ly', label: 'Order Placed' },
  { value: 'dang_xu_ly', label: 'Processing' },
  { value: 'da_giao_van', label: 'Shipped' },
  { value: 'da_giao', label: 'Delivered' }
];

const OFF_TRACK_STATUSES = new Set(['da_huy', 'cho_tra_hang', 'da_tra_hang']);

@Component({
  selector: 'app-theo-doi-don-hang',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, Header, Footer, ChatAi],
  templateUrl: './theo-doi-don-hang.html',
  styleUrl: './theo-doi-don-hang.css'
})
export class TheoDoiDonHangComponent {
  private readonly orderService = inject(OrderService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  readonly trackingSteps = TRACKING_STEPS;
  readonly orderStatusLabel = orderStatusLabel;

  readonly order = signal<Order | null>(null);
  readonly loading = signal(false);
  readonly errorMessage = signal<string | null>(null);

  orderIdInput = '';

  readonly isOffTrack = computed(() => {
    const o = this.order();
    return !!o && OFF_TRACK_STATUSES.has(o.trangThai);
  });

  readonly currentStepIndex = computed(() => {
    const o = this.order();
    if (!o) return -1;
    return this.trackingSteps.findIndex(s => s.value === o.trangThai);
  });

  constructor() {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.orderIdInput = id;
        this.loadOrder(Number(id));
      }
    });
  }

  private loadOrder(orderId: number): void {
    if (!orderId || isNaN(orderId)) {
      this.errorMessage.set('Please enter a valid order number.');
      return;
    }

    this.loading.set(true);
    this.errorMessage.set(null);
    this.order.set(null);

    this.orderService.getById(orderId).subscribe({
      next: order => {
        this.order.set(order);
        this.loading.set(false);
      },
      error: err => {
        this.errorMessage.set(err?.error?.message ?? 'Order not found. Please check the order number and try again.');
        this.loading.set(false);
      }
    });
  }

  trackOrder(): void {
    const id = Number(this.orderIdInput.trim());
    this.router.navigate(['/don-hang/theo-doi', id]);
    this.loadOrder(id);
  }

  isStepDone(index: number): boolean {
    return index <= this.currentStepIndex();
  }
}