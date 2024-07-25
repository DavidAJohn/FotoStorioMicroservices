import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CampaignAddComponent } from './campaign-add.component';

describe('CampaignAddComponent', () => {
  let component: CampaignAddComponent;
  let fixture: ComponentFixture<CampaignAddComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CampaignAddComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CampaignAddComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
