import { Component, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-footer',
  imports: [RouterLink],
  templateUrl: './footer.html',
  styleUrl: './footer.scss'
})
export class Footer {
  readonly tabDangChon = signal<'about-us' | 'products' | 'info' | 'categories'>('about-us');

  chonTab(tab: 'about-us' | 'products' | 'info' | 'categories'): void {
    this.tabDangChon.set(tab);
  }

  lenDauTrang(): void {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
}
