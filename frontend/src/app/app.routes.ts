import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';
import {
  ARTICLES_CONFIG,
  CHALLENGES_CONFIG,
  EXERCISES_CONFIG,
  FAQS_CONFIG,
  MEAL_PLANS_CONFIG,
  ROUTINES_CONFIG,
  VIDEOS_CONFIG
} from './shared/simple-crud/resource-configs';
import { SimpleCrudPageComponent } from './shared/simple-crud/simple-crud-page.component';

const CONTENT_ROLES = ['moderator', 'admin', 'super_admin'];
const ADMIN_ROLES = ['admin', 'super_admin'];

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login.component').then((m) => m.LoginComponent)
  },
  {
    path: 'register',
    canActivate: [authGuard, roleGuard(ADMIN_ROLES)],
    loadComponent: () => import('./features/auth/register.component').then((m) => m.RegisterComponent)
  },
  {
    path: '',
    loadComponent: () => import('./layout/main-layout.component').then((m) => m.MainLayoutComponent),
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        canActivate: [roleGuard(ADMIN_ROLES)],
        loadComponent: () => import('./features/dashboard/dashboard.component').then((m) => m.DashboardComponent)
      },
      {
        path: 'users',
        canActivate: [roleGuard(ADMIN_ROLES)],
        loadComponent: () => import('./features/users/users.component').then((m) => m.UsersComponent)
      },
      {
        path: 'exercises',
        canActivate: [roleGuard(CONTENT_ROLES)],
        component: SimpleCrudPageComponent,
        data: { resourceConfig: EXERCISES_CONFIG }
      },
      {
        path: 'routines',
        canActivate: [roleGuard(CONTENT_ROLES)],
        component: SimpleCrudPageComponent,
        data: { resourceConfig: ROUTINES_CONFIG }
      },
      {
        path: 'meal-plans',
        canActivate: [roleGuard(CONTENT_ROLES)],
        component: SimpleCrudPageComponent,
        data: { resourceConfig: MEAL_PLANS_CONFIG }
      },
      {
        path: 'articles',
        canActivate: [roleGuard(CONTENT_ROLES)],
        component: SimpleCrudPageComponent,
        data: { resourceConfig: ARTICLES_CONFIG }
      },
      {
        path: 'videos',
        canActivate: [roleGuard(CONTENT_ROLES)],
        component: SimpleCrudPageComponent,
        data: { resourceConfig: VIDEOS_CONFIG }
      },
      {
        path: 'faqs',
        canActivate: [roleGuard(CONTENT_ROLES)],
        component: SimpleCrudPageComponent,
        data: { resourceConfig: FAQS_CONFIG }
      },
      {
        path: 'challenges',
        canActivate: [roleGuard(CONTENT_ROLES)],
        component: SimpleCrudPageComponent,
        data: { resourceConfig: CHALLENGES_CONFIG }
      },
      {
        path: 'categories',
        canActivate: [roleGuard(CONTENT_ROLES)],
        loadComponent: () => import('./features/categories/categories.component').then((m) => m.CategoriesComponent)
      },
      {
        path: 'reported-contents',
        canActivate: [roleGuard(CONTENT_ROLES)],
        loadComponent: () =>
          import('./features/reported-contents/reported-contents.component').then((m) => m.ReportedContentsComponent)
      },
      {
        path: 'support',
        canActivate: [roleGuard(CONTENT_ROLES)],
        loadComponent: () => import('./features/support/support.component').then((m) => m.SupportComponent)
      }
    ]
  },
  { path: '**', redirectTo: 'dashboard' }
];
