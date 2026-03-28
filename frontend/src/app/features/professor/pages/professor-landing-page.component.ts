import { HttpErrorResponse } from '@angular/common/http';
import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize, switchMap, tap } from 'rxjs';

import { AuthService } from '../../../services/auth/auth.service';
import { ProfessorApiService } from '../../../services/api/professor-api.service';

@Component({
  selector: 'app-professor-landing-page',
  standalone: true,
  imports: [RouterLink, MatButtonModule, ReactiveFormsModule, MatFormFieldModule, MatInputModule],
  templateUrl: './professor-landing-page.component.html',
  styleUrl: './professor-landing-page.component.css'
})
export class ProfessorLandingPageComponent {
  readonly cadastroForm;

  enviouFormulario = false;
  carregandoCadastro = false;
  mensagemErro = '';

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly professorApiService: ProfessorApiService,
    private readonly authService: AuthService,
    private readonly router: Router
  ) {
    this.cadastroForm = this.formBuilder.nonNullable.group({
      nome: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      senha: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit(): void {
    this.enviouFormulario = true;
    this.cadastroForm.markAllAsTouched();

    if (this.cadastroForm.invalid) {
      return;
    }

    this.carregandoCadastro = true;
    this.mensagemErro = '';

    const payload = this.cadastroForm.getRawValue();
    let cadastroEfetuado = false;

    this.professorApiService
      .cadastrar(payload)
      .pipe(
        tap(() => {
          cadastroEfetuado = true;
        }),
        switchMap(() =>
          this.authService.login({
            email: payload.email,
            senha: payload.senha
          })
        ),
        finalize(() => (this.carregandoCadastro = false))
      )
      .subscribe({
        next: () => {
          void this.router.navigateByUrl('/dashboard');
        },
        error: (error: unknown) => {
          if (cadastroEfetuado) {
            this.mensagemErro =
              'Cadastro concluido, mas nao foi possivel entrar automaticamente. Tente fazer login.';
            return;
          }

          const httpError = error as HttpErrorResponse;
          this.mensagemErro = httpError.error?.erro ?? 'Nao foi possivel concluir o cadastro.';
        }
      });
  }
}
