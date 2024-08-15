import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CampaignService } from '@app/_services';
import { Campaign } from '@app/_models';
import { createDateRangeValidator } from '@app/_helpers/date-range.validator';

@Component({
  selector: 'app-campaign-add',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule, CommonModule],
  templateUrl: './campaign-add.component.html',
  styleUrl: './campaign-add.component.css'
})
export class CampaignAddComponent {
  errorMessage = '';
  submitted = false;
  loading = false;

  campaignForm = this.formBuilder.group({
    campaignName: ['', Validators.required],
    campaignStartDate: [null, Validators.required],
    campaignEndDate: [null, Validators.required],
    campaignStartTime: ['00:00', Validators.required],
    campaignEndTime: ['23:59', Validators.required]
  }, {
    validators: [createDateRangeValidator()]
  });

  constructor(private router: Router, private formBuilder: FormBuilder, private campaignService: CampaignService) {}

  // getter to simplify the template validation code
  get cf() { return this.campaignForm.controls; }

  onSubmit() {
    this.errorMessage = '';
    this.submitted = true;

    if (this.campaignForm.valid) {
      this.loading = true;

      let combinedStartDate = new Date(this.campaignForm.value.campaignStartDate! + 'T' 
                                      + this.campaignForm.value.campaignStartTime! + 'Z');
      let combinedEndDate = new Date(this.campaignForm.value.campaignEndDate! + 'T' 
                                      + this.campaignForm.value.campaignEndTime! + 'Z');

      let campaign = new Campaign();
      campaign.name = this.campaignForm.value.campaignName!;
      campaign.startDate = new Date(combinedStartDate);
      campaign.endDate = new Date(combinedEndDate);

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
      this.loading = false;
    }
  }
}
