import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

import { AuthenticationService, TokenService } from '@app/_services'
import { User } from '@app/_models';

@Component({
  selector: 'app-login-display',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './login-display.component.html',
  styleUrl: './login-display.component.css'
})
export class LoginDisplayComponent {
  user?: User | null;

  constructor(private authenticationService: AuthenticationService, private tokenService: TokenService) {
    this.authenticationService.user.subscribe(x => {
      this.user = x;
      if (this.user && this.user.token) {
        this.user.displayName = this.tokenService.getDisplayName(this.user.token);
      }
    });
  }

  logout() {
    this.authenticationService.logout();
  }
}
