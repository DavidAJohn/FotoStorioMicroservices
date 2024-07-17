import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

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

  getAllCampaigns() {
    return this.http.get<Campaign[]>(`${environment.marketingGatewayUrl}/marketing/campaigns`)
  }

  getCampaignById(id: number) {
    return this.http.get<Campaign>(`${environment.marketingGatewayUrl}/marketing/campaigns/${id}`)
  }

  createCampaign(campaign: Campaign) {
    let jsonCampaign = JSON.stringify(campaign);

    const httpOptions = {
      headers: new HttpHeaders({'Content-Type': 'application/json'}),
      observe: 'response' as 'response'
    }

    return this.http.post(`${environment.marketingGatewayUrl}/marketing/campaigns`, jsonCampaign, httpOptions);
  }
}
