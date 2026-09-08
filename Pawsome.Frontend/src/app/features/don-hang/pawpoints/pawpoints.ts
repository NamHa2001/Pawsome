import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import {
  PAWPOINTS_TYPE_FILTERS,
  PawPointsTransaction,
  VND_PER_PAWPOINT,
  pawPointsTypeLabel
} from './models/pawpoints.model';
import { PawPointsService } from './services/pawpoints.service';

@Component({
  selector: 'app-pawpoints',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './pawpoints.html',
  styleUrl: './pawpoints.css'
})
export class PawPointsComponent {
  private readonly pawPointsService = inject(PawPointsService);

  readonly typeFilters = PAWPOINTS_TYPE_FILTERS;
  readonly pawPointsTypeLabel = pawPointsTypeLabel;
  readonly vndPerPoint = VND_PER_PAWPOINT;

  readonly balance = signal<number | null>(null);
  readonly loadingBalance = signal(true);

  readonly transactions = signal<PawPointsTransaction[]>([]);
  readonly loadingHistory = signal(true);
  readonly errorMessage = signal<string | null>(null);

  readonly selectedType = signal('');
  readonly page = signal(1);
  readonly pageSize = 10;
  readonly totalCount = signal(0);
  readonly totalPages = signal(1);

  readonly balanceInVnd = computed(() => (this.balance() ?? 0) * this.vndPerPoint);
  readonly hasTransactions = computed(() => this.transactions().length > 0);

  constructor() {
    this.loadBalance();
    this.loadHistory();
  }

  private loadBalance(): void {
    this.loadingBalance.set(true);
    this.pawPointsService.getBalance().subscribe({
      next: res => {
        this.balance.set(res.soDuHienTai);
        this.loadingBalance.set(false);
      },
      error: () => this.loadingBalance.set(false)
    });
  }

  private loadHistory(): void {
    this.loadingHistory.set(true);
    this.errorMessage.set(null);

    this.pawPointsService
      .getHistory({ loai: this.selectedType() || undefined, page: this.page(), pageSize: this.pageSize })
      .subscribe({
        next: result => {
          this.transactions.set(result.items);
          this.totalCount.set(result.totalCount);
          this.totalPages.set(Math.max(1, result.totalPages));
          this.loadingHistory.set(false);
        },
        error: () => {
          this.errorMessage.set('Could not load your PawPoints history. Please try again.');
          this.loadingHistory.set(false);
        }
      });
  }

  selectType(type: string): void {
    if (this.selectedType() === type) return;
    this.selectedType.set(type);
    this.page.set(1);
    this.loadHistory();
  }

  goToPage(p: number): void {
    if (p < 1 || p > this.totalPages() || p === this.page()) return;
    this.page.set(p);
    this.loadHistory();
  }
}