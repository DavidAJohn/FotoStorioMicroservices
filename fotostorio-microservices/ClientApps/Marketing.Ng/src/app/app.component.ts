import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

import { FooterComponent } from "@shared/footer/footer.component";
import { NavMenuComponent } from "@shared/nav-menu/nav-menu.component";
import { MobileMenuComponent } from '@shared/mobile-menu/mobile-menu.component';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrl: './app.component.css',
    standalone: true,
    imports: [RouterOutlet, FooterComponent, NavMenuComponent, MobileMenuComponent]
})
export class AppComponent {
  title = 'FotoStorio Marketing';
}
