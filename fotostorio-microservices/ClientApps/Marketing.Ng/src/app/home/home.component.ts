import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  discountsMessage = '2 products discounted';
  campaignsMessage = '1 active campaign';
  ordersMessage = '4 recent orders';
  productCountToDisplay = 24;
}
