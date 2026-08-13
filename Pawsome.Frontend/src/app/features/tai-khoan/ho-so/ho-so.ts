import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Header } from '../../../shared/components/header/header';
import { Footer } from '../../../shared/components/footer/footer';
import { ChatAi } from '../../../shared/components/chat-ai/chat-ai';

@Component({
  selector: 'app-ho-so',
  standalone: true,
  imports: [CommonModule,RouterLink,Header,Footer,ChatAi],
  templateUrl: './ho-so.html',
  styleUrl: './ho-so.scss'
})
export class HoSoComponent {

}