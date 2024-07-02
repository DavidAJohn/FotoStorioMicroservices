import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { User, LoginModel, LoginResult } from '@app/_models';
import { environment } from '@environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  private userSubject: BehaviorSubject<User | null>;
  public user: Observable<User | null>;

  constructor(private router: Router, private http: HttpClient) {
    this.userSubject = new BehaviorSubject(JSON.parse(localStorage.getItem('user')!));
    this.user = this.userSubject.asObservable();
  }

  public get userValue() {
    return this.userSubject.value;
  }

  login(loginModel: LoginModel) {
    return this.http.post<LoginResult>(`${environment.identityApiUrl}/api/accounts/login`, { email: loginModel.emailAddress, password: loginModel.password})
        .pipe(map(loginResult => {
            // store basic user details and jwt token in local storage to keep user logged in between page refreshes
            let loginUser = new User();
            loginUser.emailAddress = loginModel.emailAddress;
            loginUser.userName = loginModel.emailAddress;
            loginUser.token = loginResult.token;

            localStorage.setItem('user', JSON.stringify(loginUser));
            this.userSubject.next(loginUser);
            return loginUser;
        }));
  }

  logout() {
    // remove user from local storage and set current user to null
    localStorage.removeItem('user');
    this.userSubject.next(null);
    this.router.navigate(['/account/login']);
  }
}
