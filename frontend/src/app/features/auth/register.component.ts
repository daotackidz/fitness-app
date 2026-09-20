import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, Signal, computed, signal } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router, RouterLink } from '@angular/router';
import { Select } from 'primeng/select';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { ApiResponse } from '../../core/models/api-response.model';
import { AuthService } from '../../core/services/auth.service';
import { LanguageService } from '../../core/services/language.service';

function passwordsMatchValidator(group: AbstractControl): ValidationErrors | null {
  const password = group.get('password')?.value;
  const confirmPassword = group.get('confirmPassword')?.value;
  return password === confirmPassword ? null : { passwordMismatch: true };
}

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, TranslatePipe, Select],
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  readonly loading = signal(false);
  readonly isSuperAdmin: Signal<boolean>;
  readonly roleOptions: Signal<{ value: string; label: string }[]>;
  readonly form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar,
    protected languageService: LanguageService
  ) {
    this.isSuperAdmin = computed(() => this.authService.hasAnyRole(['super_admin']));
    this.roleOptions = computed(() =>
      this.isSuperAdmin()
        ? [
            { value: 'moderator', label: 'Moderator' },
            { value: 'admin', label: 'Admin' },
            { value: 'super_admin', label: 'Super Admin' }
          ]
        : [{ value: 'moderator', label: 'Moderator' }]
    );
    this.form = this.fb.group(
      {
        fullName: ['', [Validators.required, Validators.maxLength(100)]],
        email: ['', [Validators.required, Validators.email]],
        phone: [''],
        password: ['', [Validators.required, Validators.minLength(8)]],
        confirmPassword: ['', [Validators.required]],
        role: ['moderator', [Validators.required]]
      },
      { validators: passwordsMatchValidator }
    );
  }

  async submit(): Promise<void> {
    if (this.form.invalid) return;

    this.loading.set(true);
    try {
      const { fullName, email, phone, password, role } = this.form.getRawValue();
      await firstValueFrom(
        this.http.post<ApiResponse<unknown>>(`${environment.apiBaseUrl}/admin/users`, {
          fullName,
          email,
          phone: phone || null,
          password,
          role
        })
      );
      this.snackBar.open('Tạo tài khoản thành công', 'Đóng', { duration: 3000 });
      this.router.navigate(['/users']);
    } catch {
      // lỗi đã được hiển thị bởi error interceptor
    } finally {
      this.loading.set(false);
    }
  }
}
