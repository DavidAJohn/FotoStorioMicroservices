import { Component } from '@angular/core';

import { Discount, Campaign } from '@app/_models';
import { DiscountService, CampaignService, ProductService } from '@app/_services/';

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
  ordersMessage = '4 recent orders';
  productCountToDisplay = -1;

  currentDiscounts: Discount[] = [];
  currentCampaigns: Campaign[] = [];

  constructor(private discountService: DiscountService, private campaignService: CampaignService, private productService: ProductService) { 
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
  }

  setDiscountsMessage() {
    this.discountsMessage = `${this.currentDiscounts.length} current discounts`;
  }

  setCampaignsMessage() {
    this.campaignsMessage = `${this.currentCampaigns.length} active ${this.currentCampaigns.length == 1 ? 'campaign' : 'campaigns'}`;
  }
}
