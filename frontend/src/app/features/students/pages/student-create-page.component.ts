import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Router, RouterLink } from '@angular/router';

import { StudentsApiService } from '../services/students-api.service';

@Component({
  selector: 'app-student-create-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatSnackBarModule, RouterLink],
  templateUrl: './student-create-page.component.html',
  styleUrl: './student-create-page.component.css'
})
export class StudentCreatePageComponent {
  readonly form;

  enviando = false;

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly studentsApiService: StudentsApiService,
    private readonly snackBar: MatSnackBar,
    private readonly router: Router
  ) {
    this.form = this.formBuilder.group({
      nome: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.email]]
    });
  }

  salvar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.enviando = true;
    const valor = this.form.getRawValue();

    this.studentsApiService
      .cadastrar({
        nome: valor.nome?.trim() ?? '',
        email: valor.email?.trim() || undefined
      })
      .subscribe({
        next: () => {
          this.snackBar.open('Aluno cadastrado com sucesso.', 'Fechar', { duration: 3000 });
          void this.router.navigateByUrl('/dashboard/alunos');
        },
        error: () => {
          this.enviando = false;
          this.snackBar.open('Nao foi possivel cadastrar o aluno. Tente novamente.', 'Fechar', { duration: 4000 });
        }
      });
  }
}
