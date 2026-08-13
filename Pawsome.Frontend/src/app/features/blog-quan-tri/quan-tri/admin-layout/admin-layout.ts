import { Component } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { TokenService } from '../../../../core/models/token.service';

@Component({
  selector: 'app-admin-layout',
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './admin-layout.html',
  styleUrl: './admin-layout.scss'
})
export class AdminLayout {
  constructor(
    private readonly tokenService: TokenService,
    private readonly router: Router
  ) {}

  dangXuat(): void {
    this.tokenService.clear();
    this.router.navigate(['/tai-khoan/dang-nhap']);
  }
}
