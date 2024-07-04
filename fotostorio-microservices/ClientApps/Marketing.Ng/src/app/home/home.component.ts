import { Component } from '@angular/core';

import { Discount, Campaign } from '@app/_models';
import { DiscountService, CampaignService } from '@app/_services/';

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
  productCountToDisplay = 24;

  currentDiscounts: Discount[] = [];
  currentCampaigns: Campaign[] = [];

  constructor(private discountService: DiscountService, private campaignService: CampaignService) { 
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
  }

  setDiscountsMessage() {
    this.discountsMessage = `${this.currentDiscounts.length} current discounts`;
  }

  setCampaignsMessage() {
    this.campaignsMessage = `${this.currentCampaigns.length} active ${this.currentCampaigns.length == 1 ? 'campaign' : 'campaigns'}`;
  }
}
