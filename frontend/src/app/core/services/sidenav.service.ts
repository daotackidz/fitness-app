import { Injectable, signal } from '@angular/core';

const COLLAPSED_KEY = 'fitbody_admin_sidenav_collapsed';

/** Persists whether the admin sidebar is shown collapsed (icons only) or expanded. */
@Injectable({ providedIn: 'root' })
export class SidenavService {
  private readonly collapsedSignal = signal<boolean>(localStorage.getItem(COLLAPSED_KEY) === 'true');
  readonly collapsed = this.collapsedSignal.asReadonly();

  toggle(): void {
    this.setCollapsed(!this.collapsedSignal());
  }

  setCollapsed(value: boolean): void {
    this.collapsedSignal.set(value);
    localStorage.setItem(COLLAPSED_KEY, String(value));
  }
}
