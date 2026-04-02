import { Routes } from '@angular/router';

import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./features/professor/pages/professor-landing-page.component').then(
        (module) => module.ProfessorLandingPageComponent
      )
  },
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/pages/login-page.component').then((module) => module.LoginPageComponent)
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./core/layout/app-shell.component').then((module) => module.AppShellComponent),
    canActivate: [authGuard],
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./features/dashboard/pages/dashboard-page.component').then((module) => module.DashboardPageComponent)
      },
      {
        path: 'alunos',
        loadComponent: () =>
          import('./features/students/pages/students-list-page.component').then(
            (module) => module.StudentsListPageComponent
          )
      },
      {
        path: 'alunos/novo',
        loadComponent: () =>
          import('./features/students/pages/student-create-page.component').then(
            (module) => module.StudentCreatePageComponent
          )
      },
      {
        path: 'alunos/:id',
        loadComponent: () =>
          import('./features/dashboard/pages/student-dashboard-page.component').then(
            (module) => module.StudentDashboardPageComponent
          )
      },
      {
        path: 'treinos/novo',
        loadComponent: () =>
          import('./features/trainings/pages/training-create-page.component').then(
            (module) => module.TrainingCreatePageComponent
          )
      }
    ]
  },
  {
    path: '**',
    redirectTo: ''
  }
];
