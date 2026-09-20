import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { ToggleSwitch } from 'primeng/toggleswitch';
import { TranslatePipe } from '../core/i18n/translate.pipe';
import { AuthService } from '../core/services/auth.service';
import { LanguageService } from '../core/services/language.service';
import { SidenavService } from '../core/services/sidenav.service';
import { ThemeService } from '../core/services/theme.service';

interface NavItem {
  labelKey: string;
  path: string;
  icon: string;
  roles: string[];
}

const NAV_ITEMS: NavItem[] = [
  { labelKey: 'nav.dashboard', path: '/dashboard', icon: 'dashboard', roles: ['admin', 'super_admin'] },
  { labelKey: 'nav.users', path: '/users', icon: 'people', roles: ['admin', 'super_admin'] },
  { labelKey: 'nav.exercises', path: '/exercises', icon: 'fitness_center', roles: ['moderator', 'admin', 'super_admin'] },
  { labelKey: 'nav.routines', path: '/routines', icon: 'list_alt', roles: ['moderator', 'admin', 'super_admin'] },
  { labelKey: 'nav.mealPlans', path: '/meal-plans', icon: 'restaurant', roles: ['moderator', 'admin', 'super_admin'] },
  { labelKey: 'nav.articles', path: '/articles', icon: 'article', roles: ['moderator', 'admin', 'super_admin'] },
  { labelKey: 'nav.videos', path: '/videos', icon: 'ondemand_video', roles: ['moderator', 'admin', 'super_admin'] },
  { labelKey: 'nav.faqs', path: '/faqs', icon: 'help', roles: ['moderator', 'admin', 'super_admin'] },
  { labelKey: 'nav.challenges', path: '/challenges', icon: 'emoji_events', roles: ['moderator', 'admin', 'super_admin'] },
  { labelKey: 'nav.categories', path: '/categories', icon: 'category', roles: ['moderator', 'admin', 'super_admin'] },
  { labelKey: 'nav.reportedContents', path: '/reported-contents', icon: 'flag', roles: ['moderator', 'admin', 'super_admin'] },
  { labelKey: 'nav.support', path: '/support', icon: 'support_agent', roles: ['moderator', 'admin', 'super_admin'] }
];

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatSidenavModule,
    MatToolbarModule,
    MatListModule,
    MatIconModule,
    TranslatePipe,
    ToggleSwitch
  ],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.scss'
})
export class MainLayoutComponent {
  readonly user;

  constructor(
    private authService: AuthService,
    private router: Router,
    protected themeService: ThemeService,
    protected languageService: LanguageService,
    protected sidenavService: SidenavService
  ) {
    this.user = this.authService.user;
  }

  get visibleNavItems(): NavItem[] {
    return NAV_ITEMS.filter((item) => this.authService.hasAnyRole(item.roles));
  }

  get userInitial(): string {
    const name = this.user()?.fullName?.trim();
    return name ? name.charAt(0).toUpperCase() : '?';
  }

  async logout(): Promise<void> {
    await this.authService.logout();
    this.router.navigate(['/login']);
  }
}
