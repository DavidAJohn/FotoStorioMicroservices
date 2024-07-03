import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';

import { JwtModule } from "@auth0/angular-jwt";
import { routes } from './app.routes';

export function tokenGetter() {
  return localStorage.getItem("user");
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    importProvidersFrom(
      JwtModule.forRoot({
          config: {
              tokenGetter: tokenGetter,
              allowedDomains: ["localhost:4200"],
              disallowedRoutes: ["http://localhost:4200/api/accounts/login"],
          },
      }),
    ),
    provideHttpClient(
      withInterceptorsFromDi()
    )
  ]
};