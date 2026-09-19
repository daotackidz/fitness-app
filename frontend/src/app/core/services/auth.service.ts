import { HttpClient } from '@angular/common/http';
import { Injectable, computed, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { AuthResult, AuthUser } from '../models/user.model';

const REFRESH_TOKEN_KEY = 'fitbody_admin_refresh_token';
const ADMIN_ROLES = ['moderator', 'admin', 'super_admin'];

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly accessTokenSignal = signal<string | null>(null);
  private readonly userSignal = signal<AuthUser | null>(null);

  readonly accessToken = this.accessTokenSignal.asReadonly();
  readonly user = this.userSignal.asReadonly();
  readonly isAuthenticated = computed(() => this.accessTokenSignal() !== null);

  constructor(private http: HttpClient) {}

  async login(email: string, password: string): Promise<void> {
    const response = await firstValueFrom(
      this.http.post<ApiResponse<AuthResult>>(`${environment.apiBaseUrl}/auth/login`, { email, password })
    );

    const result = response.data!;
    const role = result.user.role.toLowerCase();
    if (!ADMIN_ROLES.includes(role)) {
      throw new Error('Tai khoan nay khong co quyen truy cap trang quan tri');
    }

    this.setSession(result);
  }

  async refreshAccessToken(): Promise<boolean> {
    const refreshToken = localStorage.getItem(REFRESH_TOKEN_KEY);
    if (!refreshToken) return false;

    try {
      const response = await firstValueFrom(
        this.http.post<ApiResponse<AuthResult>>(`${environment.apiBaseUrl}/auth/refresh-token`, { refreshToken })
      );
      this.setSession(response.data!);
      return true;
    } catch {
      this.clearSession();
      return false;
    }
  }

  async logout(): Promise<void> {
    const refreshToken = localStorage.getItem(REFRESH_TOKEN_KEY);
    try {
      if (refreshToken) {
        await firstValueFrom(this.http.post(`${environment.apiBaseUrl}/auth/logout`, { refreshToken }));
      }
    } finally {
      this.clearSession();
    }
  }

  hasAnyRole(roles: string[]): boolean {
    const role = this.userSignal()?.role?.toLowerCase();
    return !!role && roles.map((r) => r.toLowerCase()).includes(role);
  }

  private setSession(result: AuthResult): void {
    this.accessTokenSignal.set(result.accessToken);
    this.userSignal.set(result.user);
    localStorage.setItem(REFRESH_TOKEN_KEY, result.refreshToken);
  }

  private clearSession(): void {
    this.accessTokenSignal.set(null);
    this.userSignal.set(null);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
  }
}
