import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, ActivatedRoute } from '@angular/router';

import { Order, OrderItem } from '@app/_models';
import { OrderService, DiscountService } from '@app/_services';

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
  itemDiscounts: string[] = [];

  constructor(private orderService: OrderService, private discountService: DiscountService, private route: ActivatedRoute) {
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
        
        // Get discounts for each item
        this.orderItems.forEach(item => {
          this.getDiscountsForSkuByDate(item.product.sku, order.orderDate);
        });
      });
  }

  getDiscountsForSkuByDate(sku: string, orderDate: Date): void {
    this.discountService.getDiscountsForSkuByDate(sku, orderDate)
      .subscribe(discounts => {
        // Add discounts to itemDiscounts array
        discounts.forEach(discount => {
          this.itemDiscounts.push(discount.sku);
        });
      });
  }
}
