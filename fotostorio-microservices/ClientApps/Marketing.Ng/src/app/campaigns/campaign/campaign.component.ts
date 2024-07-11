import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, ActivatedRoute } from '@angular/router';

import { Campaign, Discount } from '@app/_models';
import { CampaignService, DiscountService } from '@app/_services';

@Component({
  selector: 'app-campaign',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './campaign.component.html',
  styleUrl: './campaign.component.css'
})
export class CampaignComponent {
  @Input() campaign?: Campaign;
  errorMessage = '';
  currentDateTime = new Date();
  campaignDiscounts: Discount[] = [];

  constructor(private campaignService: CampaignService, private discountService: DiscountService, private route: ActivatedRoute) {
  }

  ngOnInit(): void {
    this.getCampaign();
  }

  getCampaign(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.campaignService.getCampaignById(id)
      .subscribe(campaign => {
        this.campaign = campaign;
        this.getCampaignDiscounts(campaign.id);
      });
  }

  getCampaignDiscounts(id: number): void {
    this.discountService.getDiscountsByCampaignId(id)
      .subscribe({
        next: discounts => {
          console.log('Campaign Discounts: ', discounts);
          this.campaignDiscounts = discounts;
        },
        error: error => {
          console.error('Error receiving campaign discounts: ', error);
          this.errorMessage = 'Error receiving campaign discounts: ' + error;
        }
      });
  }

  returnDateAsDate(date: Date) {
    return new Date(date);
  }
}
