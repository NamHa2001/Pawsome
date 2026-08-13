import { Component, signal } from '@angular/core';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { filter } from 'rxjs';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  readonly laKhuVucQuanTri = signal(false);

  constructor(router: Router) {
    this.laKhuVucQuanTri.set(router.url.startsWith('/quan-tri'));

    router.events
      .pipe(filter((e): e is NavigationEnd => e instanceof NavigationEnd))
      .subscribe(e => this.laKhuVucQuanTri.set(e.urlAfterRedirects.startsWith('/quan-tri')));
  }
}
