import { Component } from '@angular/core';
import { Header } from '../../../shared/components/header/header';
import { Footer } from '../../../shared/components/footer/footer';
import { ChatAi } from '../../../shared/components/chat-ai/chat-ai';

@Component({
  selector: 'app-blog',
  imports: [Header, Footer, ChatAi],
  templateUrl: './blog.html',
  styleUrl: './blog.scss'
})
export class Blog {
}
