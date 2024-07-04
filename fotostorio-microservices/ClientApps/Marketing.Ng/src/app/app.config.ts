import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors, withInterceptorsFromDi } from '@angular/common/http';

import { JwtModule } from "@auth0/angular-jwt";
import { routes } from './app.routes';
import { errorInterceptor } from './_helpers/error.interceptor';

export function tokenGetter() {
  let user = JSON.parse(localStorage.getItem("user") || "{}");
  return user.token;
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    importProvidersFrom(
      JwtModule.forRoot({
          config: {
              tokenGetter: tokenGetter,
              allowedDomains: ["localhost:4200", "localhost:8011"],
              disallowedRoutes: ["http://localhost:4200/api/accounts/login"],
          },
      }),
    ),
    provideHttpClient(
      withInterceptors([errorInterceptor]),
      withInterceptorsFromDi()
    )
  ]
};