import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '@environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  constructor(private http: HttpClient) { }

  getProductCount() {
    return this.http.get<any>(`${environment.marketingGatewayUrl}/marketing/catalog`, {observe: 'response'})
  }
}
