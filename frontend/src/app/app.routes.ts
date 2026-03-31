import { Routes } from '@angular/router';

import { authGuard } from './core/guards/auth.guard';
import { LoginPageComponent } from './features/auth/pages/login-page.component';
import { DashboardPageComponent } from './features/dashboard/pages/dashboard-page.component';
import { StudentDashboardPageComponent } from './features/dashboard/pages/student-dashboard-page.component';
import { ProfessorLandingPageComponent } from './features/professor/pages/professor-landing-page.component';
import { AppShellComponent } from './core/layout/app-shell.component';
import { StudentsListPageComponent } from './features/students/pages/students-list-page.component';
import { StudentCreatePageComponent } from './features/students/pages/student-create-page.component';
import { TrainingCreatePageComponent } from './features/trainings/pages/training-create-page.component';

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
      },
      {
        path: 'alunos',
        component: StudentsListPageComponent
      },
      {
        path: 'alunos/novo',
        component: StudentCreatePageComponent
      },
      {
        path: 'alunos/:id',
        component: StudentDashboardPageComponent
      },
      {
        path: 'treinos/novo',
        component: TrainingCreatePageComponent
      }
    ]
  },
  {
    path: '**',
    redirectTo: ''
  }
];
