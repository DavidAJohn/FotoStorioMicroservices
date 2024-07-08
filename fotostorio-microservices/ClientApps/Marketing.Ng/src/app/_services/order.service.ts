import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '@environments/environment';

@Injectable({
  providedIn: 'root'
})
export class OrderService {

  constructor(private http: HttpClient) { }

  getRecentOrders() {
    let days = environment.latestOrdersThreshold ?? 60;

    return this.http.get<any>(`${environment.marketingGatewayUrl}/marketing/orders/latest/${days}`)
  }
}
