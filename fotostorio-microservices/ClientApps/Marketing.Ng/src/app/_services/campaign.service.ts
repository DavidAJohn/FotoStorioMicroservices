import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '@environments/environment';
import { Campaign } from '@app/_models';

@Injectable({
  providedIn: 'root'
})
export class CampaignService {

  constructor(private http: HttpClient) { }

  getCurrentCampaigns() {
    return this.http.get<Campaign[]>(`${environment.marketingGatewayUrl}/marketing/campaigns/current`)
  }
}
