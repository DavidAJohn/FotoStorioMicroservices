import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

import { FooterComponent } from "@shared/footer/footer.component";

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrl: './app.component.css',
    standalone: true,
    imports: [RouterOutlet, FooterComponent]
})
export class AppComponent {
  title = 'FotoStorio Marketing';
}
