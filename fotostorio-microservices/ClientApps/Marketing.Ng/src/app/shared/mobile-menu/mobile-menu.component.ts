import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

import { NavMenuItem } from '@models/NavMenuItem';

@Component({
  selector: 'app-mobile-menu',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './mobile-menu.component.html',
  styleUrl: './mobile-menu.component.css'
})
export class MobileMenuComponent {
  navMenuItems: NavMenuItem[] = [
    { id: 1, displayName: "Home", url: "/" },
    { id: 2, displayName: "Test", url: "/test" },
    { id: 3, displayName: "Additional Link", url: "/link" },
  ];
}
