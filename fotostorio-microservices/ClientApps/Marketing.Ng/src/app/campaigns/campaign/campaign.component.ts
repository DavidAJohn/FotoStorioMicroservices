import { Component, Input } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { ActivatedRoute } from '@angular/router';

import { Campaign, Discount, Product } from '@app/_models';
import { CampaignService, DiscountService, ProductService } from '@app/_services';

@Component({
  selector: 'app-campaign',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './campaign.component.html',
  styleUrl: './campaign.component.css'
})
export class CampaignComponent {
  @Input() campaign?: Campaign;
  errorMessage = '';
  currentDateTime = new Date();
  campaignDiscounts: Discount[] = [];
  discountedProducts: Product[] = [];

  constructor(private campaignService: CampaignService, 
              private discountService: DiscountService, 
              private productService: ProductService, 
              private location: Location,
              private route: ActivatedRoute) {
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
          this.campaignDiscounts = discounts;

          // Get details of the products that are discounted
          this.campaignDiscounts.forEach(discount => {
            this.productService.getProductBySku(discount.sku)
              .subscribe({
                next: product => {
                  product.salePrice = discount.salePrice;
                  this.discountedProducts.push(product);
                },
                error: error => {
                  console.error('Error receiving discounted product: ', error);
                  this.errorMessage = 'Error receiving discounted product: ' + error;
                }
              });
          });
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

  goBack(): void {
    this.location.back();
  }
}
