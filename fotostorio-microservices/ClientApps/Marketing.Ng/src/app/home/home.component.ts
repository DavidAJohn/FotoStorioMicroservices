import { Component } from '@angular/core';

import { Discount, Campaign } from '@app/_models';
import { DiscountService, CampaignService, ProductService, OrderService } from '@app/_services/';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  discountsMessage = 'Unknown';
  campaignsMessage = 'Unknown';
  ordersMessage = 'Unknown';
  productCountToDisplay = -1;

  currentDiscounts: Discount[] = [];
  currentCampaigns: Campaign[] = [];

  constructor(private discountService: DiscountService, 
    private campaignService: CampaignService, 
    private productService: ProductService,
    private orderService: OrderService) {

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

    this.campaignService.getCurrentCampaigns()
      .subscribe({
        next: campaigns => {
          this.currentCampaigns = campaigns;
          this.setCampaignsMessage();
        },
        error: error => {
          console.error('Error receiving campaigns: ', error);
        }
      });

    this.productService.getProductCount()
      .subscribe({
        next: response => {
          let paginationHeaders = response.headers.get('Pagination');

          if (paginationHeaders) {
            let pagination = JSON.parse(paginationHeaders);
            this.productCountToDisplay = pagination.TotalCount ?? -1;
          } else {
            console.log('No pagination headers found.');
          }
        },
        error: error => {
          console.error('Error receiving product count: ', error);
        }
      });

    this.orderService.getOrderCount()
      .subscribe({
        next: orders => {
          if (orders.body) {
            //console.log('Received orders: ', orders);
            this.setOrdersMessage(orders.body.length);
          } else {
            //console.log('No orders found');
            this.ordersMessage = '0 recent orders';
          }
        },
        error: error => {
          this.ordersMessage = 'Error retrieving orders';
          console.error('Error receiving orders: ', error);
        }
      });
  }

  setDiscountsMessage() {
    this.discountsMessage = `${this.currentDiscounts.length} current discounts`;
  }

  setCampaignsMessage() {
    this.campaignsMessage = `${this.currentCampaigns.length} active ${this.currentCampaigns.length == 1 ? 'campaign' : 'campaigns'}`;
  }

  setOrdersMessage(orderCount: number) {
    this.ordersMessage = `${orderCount} recent order${orderCount == 1 ? '' : 's'}`;
  }
}
