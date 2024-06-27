import { Routes } from '@angular/router';

export const routes: Routes = [
    { path: '', redirectTo: '/home', pathMatch: 'full' },

    // otherwise redirect to home
    { path: '**', redirectTo: '' }
];