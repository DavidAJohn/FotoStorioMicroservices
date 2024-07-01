import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-login-display',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './login-display.component.html',
  styleUrl: './login-display.component.css'
})
export class LoginDisplayComponent {
  displayName = 'Marketing Admin';
}
