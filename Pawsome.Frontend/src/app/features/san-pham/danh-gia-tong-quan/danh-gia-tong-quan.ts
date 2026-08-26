import { DecimalPipe } from '@angular/common';
import { Component, inject, input, output } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TokenService } from '../../../core/models/token.service';

@Component({
  selector: 'app-danh-gia-tong-quan',
  imports: [RouterLink, DecimalPipe],
  templateUrl: './danh-gia-tong-quan.html',
  styleUrl: './danh-gia-tong-quan.scss'
})
export class DanhGiaTongQuan {
  readonly tokenService = inject(TokenService);

  readonly soLuongDanhGia = input(0);
  readonly diemDanhGiaTb = input(0);
  readonly tongPhanTramSao = input.required<Record<number, number>>();

  readonly bamWriteReview = output<void>();
}
