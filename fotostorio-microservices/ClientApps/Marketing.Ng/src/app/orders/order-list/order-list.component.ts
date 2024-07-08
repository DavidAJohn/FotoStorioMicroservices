import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

import { Order } from '@app/_models/Order';
import { OrderService } from '@app/_services';

@Component({
  selector: 'app-order-list',
  standalone: true,
  imports: [RouterLink, CommonModule],
  templateUrl: './order-list.component.html',
  styleUrl: './order-list.component.css'
})
export class OrderListComponent {
  errorMessage = '';
  orders: Order[] = [];

  constructor(private orderService: OrderService) {
    this.orderService.getRecentOrders()
      .subscribe({
        next: orders => {
          this.orders = orders;
        },
        error: error => {
          console.error('Error receiving orders: ', error);
          this.errorMessage = 'Error receiving orders: ' + error;
        }
      });
  }
}
