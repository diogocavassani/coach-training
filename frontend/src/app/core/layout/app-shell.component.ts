import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { AuthService } from '../../services/auth/auth.service';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, MatButtonModule],
  templateUrl: './app-shell.component.html',
  styleUrl: './app-shell.component.css'
})
export class AppShellComponent {
  sidebarAberta = false;
  readonly navItems = [
    { label: 'Leituras', routerLink: '/dashboard', exact: true },
    { label: 'Alunos', routerLink: '/dashboard/alunos', exact: true },
    { label: 'Novo aluno', routerLink: '/dashboard/alunos/novo', exact: true },
    { label: 'Novo treino', routerLink: '/dashboard/treinos/novo', exact: true }
  ] as const;

  constructor(
    private readonly authService: AuthService,
    private readonly router: Router
  ) {}

  toggleSidebar(): void {
    this.sidebarAberta = !this.sidebarAberta;
  }

  fecharSidebar(): void {
    this.sidebarAberta = false;
  }

  logout(): void {
    this.authService.logout();
    void this.router.navigateByUrl('/login');
  }
}
