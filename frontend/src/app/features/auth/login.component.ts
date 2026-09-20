import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { Checkbox } from 'primeng/checkbox';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { AuthService } from '../../core/services/auth.service';
import { LanguageService } from '../../core/services/language.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, TranslatePipe, Checkbox],
  templateUrl: './login.component.html'
})
export class LoginComponent {
  readonly loading = signal(false);
  readonly form: ReturnType<FormBuilder['group']>;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar,
    protected languageService: LanguageService
  ) {
    const rememberedEmail = this.authService.getRememberedEmail();
    this.form = this.fb.group({
      email: [rememberedEmail ?? '', [Validators.required, Validators.email]],
      password: ['', [Validators.required]],
      rememberMe: [!!rememberedEmail]
    });
  }

  async submit(): Promise<void> {
    if (this.form.invalid) return;

    this.loading.set(true);
    try {
      const { email, password, rememberMe } = this.form.getRawValue();
      await this.authService.login(email!, password!, !!rememberMe);
      this.router.navigate(['/dashboard']);
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Đăng nhập thất bại';
      this.snackBar.open(message, 'Dong', { duration: 4000 });
    } finally {
      this.loading.set(false);
    }
  }
}
