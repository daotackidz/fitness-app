import { Pipe, PipeTransform, inject } from '@angular/core';
import { LanguageService } from '../services/language.service';
import { TRANSLATIONS } from './translations';

/**
 * `pure: false` so the text updates immediately when LanguageService's
 * signal flips, without needing the surrounding key to change identity.
 */
@Pipe({ name: 'translate', standalone: true, pure: false })
export class TranslatePipe implements PipeTransform {
  private readonly languageService = inject(LanguageService);

  transform(key: string): string {
    const entry = TRANSLATIONS[key];
    if (!entry) return key;
    return entry[this.languageService.lang()];
  }
}
