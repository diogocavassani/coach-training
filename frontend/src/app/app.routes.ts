import { Routes } from '@angular/router';

import { authGuard } from './core/guards/auth.guard';
import { LoginPageComponent } from './features/auth/pages/login-page.component';
import { DashboardPageComponent } from './features/dashboard/pages/dashboard-page.component';
import { ProfessorLandingPageComponent } from './features/professor/pages/professor-landing-page.component';
import { AppShellComponent } from './core/layout/app-shell.component';

export const routes: Routes = [
  {
    path: '',
    component: ProfessorLandingPageComponent
  },
  {
    path: 'login',
    component: LoginPageComponent
  },
  {
    path: 'dashboard',
    component: AppShellComponent,
    canActivate: [authGuard],
    children: [
      {
        path: '',
        component: DashboardPageComponent
      }
    ]
  },
  {
    path: '**',
    redirectTo: ''
  }
];
