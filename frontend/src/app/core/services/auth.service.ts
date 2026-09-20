import { HttpClient } from '@angular/common/http';
import { Injectable, computed, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { AuthResult, AuthUser } from '../models/user.model';

const REFRESH_TOKEN_KEY = 'fitbody_admin_refresh_token';
const SESSION_KEY = 'fitbody_admin_session';
const REMEMBERED_EMAIL_KEY = 'fitbody_admin_remembered_email';
const ADMIN_ROLES = ['moderator', 'admin', 'super_admin'];

interface StoredSession {
  accessToken: string;
  user: AuthUser;
  expiresAt: number;
}

/**
 * Backend sends roles as PascalCase enum names (e.g. "SuperAdmin"), but the
 * app's role checks use snake_case (e.g. "super_admin"). Converts between
 * the two so a real SuperAdmin isn't rejected as an unrecognized role.
 */
export function normalizeRole(role: string): string {
  return role.replace(/([a-z0-9])([A-Z])/g, '$1_$2').toLowerCase();
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly accessTokenSignal = signal<string | null>(null);
  private readonly userSignal = signal<AuthUser | null>(null);

  readonly accessToken = this.accessTokenSignal.asReadonly();
  readonly user = this.userSignal.asReadonly();
  readonly isAuthenticated = computed(() => this.accessTokenSignal() !== null);

  constructor(private http: HttpClient) {}

  async login(email: string, password: string, rememberEmail = false): Promise<void> {
    const response = await firstValueFrom(
      this.http.post<ApiResponse<AuthResult>>(`${environment.apiBaseUrl}/auth/login`, { email, password })
    );

    const result = response.data!;
    const role = normalizeRole(result.user.role);
    if (!ADMIN_ROLES.includes(role)) {
      throw new Error('Tai khoan nay khong co quyen truy cap trang quan tri');
    }

    this.setSession(result);

    if (rememberEmail) {
      localStorage.setItem(REMEMBERED_EMAIL_KEY, email);
    } else {
      localStorage.removeItem(REMEMBERED_EMAIL_KEY);
    }
  }

  getRememberedEmail(): string | null {
    return localStorage.getItem(REMEMBERED_EMAIL_KEY);
  }

  /**
   * Runs once at app bootstrap (see app.config.ts) so a page reload doesn't
   * bounce an already-logged-in admin back to /login. Restores the session
   * instantly from sessionStorage while it's still valid, and otherwise
   * falls back to exchanging the persisted refresh token for a new one.
   */
  async restoreSession(): Promise<boolean> {
    const stored = this.readStoredSession();
    if (stored && stored.expiresAt > Date.now()) {
      this.accessTokenSignal.set(stored.accessToken);
      this.userSignal.set(stored.user);
      return true;
    }

    return this.refreshAccessToken();
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
    const rawRole = this.userSignal()?.role;
    if (!rawRole) return false;
    const role = normalizeRole(rawRole);
    return roles.map((r) => normalizeRole(r)).includes(role);
  }

  private setSession(result: AuthResult): void {
    this.accessTokenSignal.set(result.accessToken);
    this.userSignal.set(result.user);
    localStorage.setItem(REFRESH_TOKEN_KEY, result.refreshToken);

    const stored: StoredSession = {
      accessToken: result.accessToken,
      user: result.user,
      expiresAt: Date.now() + result.expiresInSeconds * 1000
    };
    sessionStorage.setItem(SESSION_KEY, JSON.stringify(stored));
  }

  private clearSession(): void {
    this.accessTokenSignal.set(null);
    this.userSignal.set(null);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    sessionStorage.removeItem(SESSION_KEY);
  }

  private readStoredSession(): StoredSession | null {
    const raw = sessionStorage.getItem(SESSION_KEY);
    if (!raw) return null;
    try {
      return JSON.parse(raw) as StoredSession;
    } catch {
      return null;
    }
  }
}
