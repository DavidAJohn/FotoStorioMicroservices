import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '@environments/environment';
import { Order } from '@app/_models';

@Injectable({
  providedIn: 'root'
})
export class OrderService {

  constructor(private http: HttpClient) { }

  getRecentOrders() {
    let days = environment.latestOrdersThreshold ?? 60;

    return this.http.get<any>(`${environment.marketingGatewayUrl}/marketing/orders/latest/${days}`)
  }

  getOrderById(id: number) {
    return this.http.get<Order>(`${environment.marketingGatewayUrl}/marketing/orders/${id}`)
  }
}
