import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, ActivatedRoute } from '@angular/router';

import { Order, OrderItem } from '@app/_models';
import { OrderService } from '@app/_services';

@Component({
  selector: 'app-order',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './order.component.html',
  styleUrl: './order.component.css'
})
export class OrderComponent {
  @Input() order?: Order;
  orderItems: OrderItem[] = [];
  errorMessage = '';

  constructor(private orderService: OrderService, private route: ActivatedRoute) {
  }

  ngOnInit(): void {
    this.getOrder();
  }

  getOrder(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.orderService.getOrderById(id)
      .subscribe(order => {
        this.order = order;
        this.orderItems = order.items;
      });
  }
}
