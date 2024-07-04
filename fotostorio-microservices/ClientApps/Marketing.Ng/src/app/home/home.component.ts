import { Component } from '@angular/core';

import { Discount } from '@app/_models';
import { DiscountService } from '@app/_services/discount.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  discountsMessage = 'Unknown';
  campaignsMessage = '1 active campaign';
  ordersMessage = '4 recent orders';
  productCountToDisplay = 24;

  currentDiscounts: Discount[] = [];

  constructor(private discountService: DiscountService) {
    this.discountService.getCurrentDiscounts()
      .subscribe({
        next: discounts => {
          this.currentDiscounts = discounts;
          this.setDiscountsMessage();
        },
        error: error => {
          console.error('Error receiving discounts: ', error);
        }
      });
  }

  setDiscountsMessage() {
    this.discountsMessage = `${this.currentDiscounts.length} current discounts`;
  }
}
