import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../core/services/auth.service';

interface NavItem {
  label: string;
  path: string;
  icon: string;
  roles: string[];
}

const NAV_ITEMS: NavItem[] = [
  { label: 'Dashboard', path: '/dashboard', icon: 'dashboard', roles: ['admin', 'super_admin'] },
  { label: 'Nguoi dung', path: '/users', icon: 'people', roles: ['admin', 'super_admin'] },
  { label: 'Bai tap', path: '/exercises', icon: 'fitness_center', roles: ['moderator', 'admin', 'super_admin'] },
  { label: 'Routine', path: '/routines', icon: 'list_alt', roles: ['moderator', 'admin', 'super_admin'] },
  { label: 'Meal Plans', path: '/meal-plans', icon: 'restaurant', roles: ['moderator', 'admin', 'super_admin'] },
  { label: 'Articles', path: '/articles', icon: 'article', roles: ['moderator', 'admin', 'super_admin'] },
  { label: 'Videos', path: '/videos', icon: 'ondemand_video', roles: ['moderator', 'admin', 'super_admin'] },
  { label: 'FAQs', path: '/faqs', icon: 'help', roles: ['moderator', 'admin', 'super_admin'] },
  { label: 'Challenges', path: '/challenges', icon: 'emoji_events', roles: ['moderator', 'admin', 'super_admin'] },
  { label: 'Kiem duyet', path: '/reported-contents', icon: 'flag', roles: ['moderator', 'admin', 'super_admin'] },
  { label: 'Ho tro', path: '/support', icon: 'support_agent', roles: ['moderator', 'admin', 'super_admin'] }
];

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive, MatSidenavModule, MatToolbarModule, MatListModule, MatIconModule],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.scss'
})
export class MainLayoutComponent {
  readonly user;

  constructor(private authService: AuthService, private router: Router) {
    this.user = this.authService.user;
  }

  get visibleNavItems(): NavItem[] {
    return NAV_ITEMS.filter((item) => this.authService.hasAnyRole(item.roles));
  }

  async logout(): Promise<void> {
    await this.authService.logout();
    this.router.navigate(['/login']);
  }
}
