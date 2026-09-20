import { Injectable, signal } from '@angular/core';

const THEME_KEY = 'fitbody_admin_theme';

export type ThemeMode = 'light' | 'dark';

/**
 * Controls the app shell's light/dark mode by toggling a `.dark` class on
 * <html> (see styles.scss / tailwind.css `@custom-variant dark`, and
 * app.config.ts's `providePrimeNG` darkModeSelector: '.dark' - all three
 * are wired to this same class). Defaults to light regardless of the
 * OS/browser preference and only switches when the admin explicitly picks
 * dark - deliberately not tied to `prefers-color-scheme`, since Material's
 * theme mixin following it silently is what made text unreadable before.
 */
@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly modeSignal = signal<ThemeMode>(this.readInitialMode());
  readonly mode = this.modeSignal.asReadonly();

  constructor() {
    this.apply(this.modeSignal());
  }

  toggle(): void {
    this.setMode(this.modeSignal() === 'dark' ? 'light' : 'dark');
  }

  setMode(mode: ThemeMode): void {
    this.modeSignal.set(mode);
    localStorage.setItem(THEME_KEY, mode);
    this.apply(mode);
  }

  private apply(mode: ThemeMode): void {
    document.documentElement.classList.toggle('dark', mode === 'dark');
  }

  private readInitialMode(): ThemeMode {
    return localStorage.getItem(THEME_KEY) === 'dark' ? 'dark' : 'light';
  }
}
