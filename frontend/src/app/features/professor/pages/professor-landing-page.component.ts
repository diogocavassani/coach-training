import { HttpErrorResponse } from '@angular/common/http';
import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

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
  cadastroConcluido = false;

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly professorApiService: ProfessorApiService
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
    this.cadastroConcluido = false;

    this.professorApiService
      .cadastrar(this.cadastroForm.getRawValue())
      .pipe(finalize(() => (this.carregandoCadastro = false)))
      .subscribe({
        next: () => {
          this.cadastroConcluido = true;
          this.cadastroForm.reset({ nome: '', email: '', senha: '' });
          this.enviouFormulario = false;
        },
        error: (error: HttpErrorResponse) => {
          this.mensagemErro = error.error?.erro ?? 'Nao foi possivel concluir o cadastro.';
        }
      });
  }
}
