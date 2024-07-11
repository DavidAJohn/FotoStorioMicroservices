import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '@environments/environment';
import { Product } from '@app/_models';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  constructor(private http: HttpClient) { }

  getProductCount() {
    return this.http.get<Product[]>(`${environment.marketingGatewayUrl}/marketing/catalog`, {observe: 'response'})
  }

  getProductBySku(sku: string) {
    return this.http.get<Product>(`${environment.marketingGatewayUrl}/marketing/catalog/sku/${sku}`)
  }
}
