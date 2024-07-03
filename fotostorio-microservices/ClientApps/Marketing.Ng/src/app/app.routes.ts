import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './account/login.component';
import { AuthGuard } from './_helpers/auth.guard';
import { UnauthorisedComponent } from './account/unauthorised.component';

export const routes: Routes = [
    { path: '', redirectTo: '/home', pathMatch: 'full' },
    { path: 'home', component: HomeComponent, canActivate: [AuthGuard]  },
    { path: 'account/login', component: LoginComponent },
    { path: 'unauthorised', component: UnauthorisedComponent },
    
    // otherwise redirect to home
    { path: '**', redirectTo: '' }
];