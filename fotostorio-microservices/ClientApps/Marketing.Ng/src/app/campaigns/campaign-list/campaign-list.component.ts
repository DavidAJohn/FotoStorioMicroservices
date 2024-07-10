import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

import { Campaign } from '@app/_models';
import { CampaignService } from '@app/_services';

@Component({
  selector: 'app-campaign-list',
  standalone: true,
  imports: [RouterLink, CommonModule],
  templateUrl: './campaign-list.component.html',
  styleUrl: './campaign-list.component.css'
})
export class CampaignListComponent {
  errorMessage = '';
  campaigns: Campaign[] = [];
  currentDateTime = new Date();

  constructor(private campaignService: CampaignService) {
    this.campaignService.getAllCampaigns()
      .subscribe({
        next: campaigns => {
          this.campaigns = campaigns;

          // Sort campaigns by start date (desc)
          this.campaigns.sort((a, b) => {
            return new Date(b.startDate).getTime() - new Date(a.startDate).getTime();
          });

        },
        error: error => {
          console.error('Error receiving campaigns: ', error);
          this.errorMessage = 'Error receiving campaigns: ' + error;
        }
      });
  }

  returnDateAsDate(date: Date) {
    return new Date(date);
  }
}
