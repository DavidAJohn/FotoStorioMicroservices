import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '@environments/environment';
import { Discount } from '@models/Discount';

@Injectable({
  providedIn: 'root'
})
export class DiscountService {

  constructor(private http: HttpClient) { }

  getCurrentDiscounts() {
    return this.http.get<Discount[]>(`${environment.marketingGatewayUrl}/marketing/discounts/current`)
  }

  getDiscountsForSkuByDate(sku: string, date: Date) {
    let encodedDate = encodeURIComponent(date.toString());
    return this.http.get<Discount[]>(`${environment.marketingGatewayUrl}/marketing/discounts/sku/${sku}/date/${encodedDate}`)
  }

  getDiscountsByCampaignId(campaignId: number) {
    return this.http.get<Discount[]>(`${environment.marketingGatewayUrl}/marketing/discounts/campaign/${campaignId}`)
  }
}
