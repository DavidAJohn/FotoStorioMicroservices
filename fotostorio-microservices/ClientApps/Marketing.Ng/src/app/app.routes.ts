import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './account/login.component';
import { AuthGuard } from './_helpers/auth.guard';
import { UnauthorisedComponent } from './account/unauthorised.component';
import { OrderListComponent } from './orders/order-list/order-list.component';
import { OrderComponent } from './orders/order/order.component';
import { CampaignListComponent } from './campaigns/campaign-list/campaign-list.component';
import { CampaignComponent } from './campaigns/campaign/campaign.component';

export const routes: Routes = [
    { path: '', redirectTo: '/home', pathMatch: 'full' },
    { path: 'home', component: HomeComponent, canActivate: [AuthGuard]  },
    { path: 'account/login', component: LoginComponent },
    { path: 'unauthorised', component: UnauthorisedComponent },
    { path: 'orders', component: OrderListComponent, canActivate: [AuthGuard]},
    { path: 'order/:id', component: OrderComponent, canActivate: [AuthGuard]},
    { path: 'campaigns', component: CampaignListComponent, canActivate: [AuthGuard]},
    { path: 'campaign/:id', component: CampaignComponent, canActivate: [AuthGuard]},
    
    // otherwise redirect to home
    { path: '**', redirectTo: '' }
];