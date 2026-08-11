import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-banner-noi-bat',
  imports: [RouterLink],
  templateUrl: './banner-noi-bat.html',
  styleUrl: './banner-noi-bat.scss'
})
export class BannerNoiBat {
  readonly anhTrungTam = input.required<string>();
  readonly altTrungTam = input.required<string>();
  readonly anhNhoTrai = input.required<string>();
  readonly anhNhoPhai = input.required<string>();
  readonly anhHeroTrai = input.required<string[]>();
  readonly anhHeroPhai = input.required<string[]>();
}
