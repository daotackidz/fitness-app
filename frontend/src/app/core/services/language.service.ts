import { Injectable, signal } from '@angular/core';

const LANG_KEY = 'fitbody_admin_lang';

export type Lang = 'vi' | 'en';

@Injectable({ providedIn: 'root' })
export class LanguageService {
  private readonly langSignal = signal<Lang>(this.readInitial());
  readonly lang = this.langSignal.asReadonly();

  toggle(): void {
    this.setLang(this.langSignal() === 'vi' ? 'en' : 'vi');
  }

  setLang(lang: Lang): void {
    this.langSignal.set(lang);
    localStorage.setItem(LANG_KEY, lang);
  }

  private readInitial(): Lang {
    return localStorage.getItem(LANG_KEY) === 'en' ? 'en' : 'vi';
  }
}
