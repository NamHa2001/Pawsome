import { Component } from '@angular/core';
import { Header } from '../../../shared/components/header/header';
import { Footer } from '../../../shared/components/footer/footer';

@Component({
  selector: 'app-wishlist',
  imports: [Header, Footer],
  templateUrl: './wishlist.html',
  styleUrl: './wishlist.scss'
})
export class Wishlist {
}
