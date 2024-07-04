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
}
