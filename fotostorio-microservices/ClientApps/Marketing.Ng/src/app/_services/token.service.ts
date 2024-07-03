import { Injectable } from '@angular/core';

import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class TokenService {

  constructor(public jwtHelper: JwtHelperService) { }

  private decodeToken(token: string) {
    return this.jwtHelper.decodeToken(token);
  }

  private getTokenExpirationDate(token: string) {
    const decodedToken = this.decodeToken(token);

    if (decodedToken.exp === undefined) {
      return null;
    }

    const date = new Date(0);
    date.setUTCSeconds(decodedToken.exp);

    return date;
  }

  public isTokenExpired(token: string, offsetSeconds?: number) {
    const date = this.getTokenExpirationDate(token);
    offsetSeconds = offsetSeconds || 0;

    if (date === null) {
      return false;
    }

    return !(date.valueOf() > new Date().valueOf() + offsetSeconds * 1000);
  }

  public getDisplayName(token: string) {
    const decodedToken = this.decodeToken(token);

    return decodedToken.given_name;
  }

  public getRole(token: string) {
    const decodedToken = this.decodeToken(token);

    return decodedToken.role;
  }
}
