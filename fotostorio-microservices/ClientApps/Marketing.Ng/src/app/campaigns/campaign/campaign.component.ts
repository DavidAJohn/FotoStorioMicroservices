import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, ActivatedRoute } from '@angular/router';

import { Campaign } from '@app/_models';
import { CampaignService } from '@app/_services';

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

  constructor(private campaignService: CampaignService, private route: ActivatedRoute) {
  }

  ngOnInit(): void {
    this.getCampaign();
  }

  getCampaign(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.campaignService.getCampaignById(id)
      .subscribe(campaign => {
        this.campaign = campaign;
      });
  }

  returnDateAsDate(date: Date) {
    return new Date(date);
  }
}
