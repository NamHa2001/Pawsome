import { DecimalPipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { DashboardStats } from './models/dashboard-stats.model';
import { DashboardService } from './services/dashboard.service';

@Component({
  selector: 'app-quan-tri-dashboard',
  standalone: true,
  imports: [DecimalPipe],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard {
  private readonly dashboardService = inject(DashboardService);

  readonly thongKe = signal<DashboardStats | null>(null);
  readonly dangTai = signal(true);
  readonly loiTai = signal<string | null>(null);

  constructor() {
    this.taiThongKe();
  }

  taiThongKe(): void {
    this.dangTai.set(true);
    this.loiTai.set(null);

    this.dashboardService.layThongKe().subscribe({
      next: ket => {
        this.thongKe.set(ket);
        this.dangTai.set(false);
      },
      error: () => {
        this.loiTai.set('Failed to load dashboard statistics.');
        this.dangTai.set(false);
      }
    });
  }
}
