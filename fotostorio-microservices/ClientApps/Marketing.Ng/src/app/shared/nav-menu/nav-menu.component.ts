import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

import { NavMenuItem } from '@models/NavMenuItem';

@Component({
  selector: 'app-nav-menu',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './nav-menu.component.html',
  styleUrl: './nav-menu.component.css'
})
export class NavMenuComponent {
  navMenuItems: NavMenuItem[] = [
    { id: 1, displayName: "Discounts", url: "/discounts" },
    { id: 2, displayName: "Campaigns", url: "/campaigns" },
    { id: 3, displayName: "Orders", url: "/orders" },
  ];
}
