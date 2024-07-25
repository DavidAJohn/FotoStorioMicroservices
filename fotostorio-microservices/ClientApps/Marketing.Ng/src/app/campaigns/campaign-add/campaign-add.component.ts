import { Component } from '@angular/core';
import { RouterLink, Router } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CampaignService } from '@app/_services';
import { Campaign } from '@app/_models';

@Component({
  selector: 'app-campaign-add',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule],
  templateUrl: './campaign-add.component.html',
  styleUrl: './campaign-add.component.css'
})
export class CampaignAddComponent {
  errorMessage = '';

  campaignForm = this.formBuilder.group({
    campaignName: ['', Validators.required],
    campaignStartDate: ['', Validators.required],
    campaignEndDate: ['', Validators.required]
  });

  constructor(private router: Router, private formBuilder: FormBuilder, private campaignService: CampaignService) {}

  onSubmit() {
    this.errorMessage = '';

    if (this.campaignForm.valid) {
      console.log('Form values submitted', this.campaignForm.value);

      let campaign = new Campaign();
      campaign.name = this.campaignForm.value.campaignName!;
      campaign.startDate = new Date(this.campaignForm.value.campaignStartDate!);
      campaign.endDate = new Date(this.campaignForm.value.campaignEndDate!);

      this.campaignService.createCampaign(campaign)
        .subscribe({
          next: response => {
            console.log('Campaign created successfully!');
            console.log('Server response: ', response);
            this.router.navigate(['/campaigns']);
          },
          error: error => {
            console.error('Error creating campaign: ', error);
            this.errorMessage = 'Error creating campaign: ' + error;
          }
        });
    } else {
      this.errorMessage = 'Please fill in all fields';
    }
  }
}
