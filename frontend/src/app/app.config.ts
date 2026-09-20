import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { ApplicationConfig, inject, provideAppInitializer, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideRouter } from '@angular/router';
import Aura from '@primeuix/themes/aura';
import { providePrimeNG } from 'primeng/config';

import { authInterceptor } from './core/interceptors/auth.interceptor';
import { errorInterceptor } from './core/interceptors/error.interceptor';
import { AuthService } from './core/services/auth.service';
import { ThemeService } from './core/services/theme.service';
import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideAnimationsAsync(),
    provideHttpClient(withInterceptors([authInterceptor, errorInterceptor])),
    // PrimeNG components (Select, Checkbox, ToggleSwitch, ...) used
    // alongside Angular Material. darkModeSelector is the same `.dark`
    // class ThemeService toggles, so PrimeNG follows the app's own toggle
    // instead of the OS preference.
    providePrimeNG({
      theme: {
        preset: Aura,
        options: { darkModeSelector: '.dark' }
      }
    }),
    // Applying the saved light/dark class must happen before first render,
    // otherwise the page briefly flashes the wrong theme.
    provideAppInitializer(() => {
      inject(ThemeService);
    }),
    // Restore a logged-in session from storage before the first route
    // resolves, so reloading the page (F5) doesn't bounce back to /login.
    provideAppInitializer(() => inject(AuthService).restoreSession())
  ]
};
