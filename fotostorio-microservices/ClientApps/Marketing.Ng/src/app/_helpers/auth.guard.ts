import { Injectable } from '@angular/core';
import { Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';

import { AuthenticationService, TokenService } from '@app/_services';
import { environment } from '@environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthGuard {
    constructor(
        private router: Router,
        private authenticationService: AuthenticationService,
        private tokenService: TokenService
    ) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        const user = this.authenticationService.userValue;
        if (user) {
            // check if token is expired
            if (user.token && this.tokenService.isTokenExpired(user.token)) {
                this.authenticationService.logout();
                this.router.navigate(['/account/login'], { queryParams: { returnUrl: state.url } });
                return false;
            }
            if (user.token) {
                // check if route is restricted by role
                if (environment.allowedRoles && environment.allowedRoles.indexOf(this.tokenService.getRole(user.token)) === -1) {
                    // role not authorised, so redirect to advisory page
                    this.router.navigate(['/unauthorised']);
                    return false;
                }

                // authenticated and role is allowed, so return true
                return true;
            }
        }

        // not logged in so redirect to login page with the return url
        this.router.navigate(['/account/login'], { queryParams: { returnUrl: state.url } });
        return false;
    }
}
