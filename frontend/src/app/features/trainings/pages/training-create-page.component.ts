import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Router, RouterLink } from '@angular/router';

import { TrainingsApiService } from '../services/trainings-api.service';

@Component({
  selector: 'app-training-create-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatButtonModule, MatFormFieldModule, MatInputModule, MatSnackBarModule, RouterLink],
  templateUrl: './training-create-page.component.html',
  styleUrl: './training-create-page.component.css'
})
export class TrainingCreatePageComponent {
  readonly form;
  enviando = false;

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly trainingsApiService: TrainingsApiService,
    private readonly snackBar: MatSnackBar,
    private readonly router: Router
  ) {
    this.form = this.formBuilder.group({
      atletaId: ['', [Validators.required]],
      data: ['', [Validators.required]],
      tipo: [0, [Validators.required, Validators.min(0)]],
      duracaoMinutos: [30, [Validators.required, Validators.min(1)]],
      distanciaKm: [0, [Validators.required, Validators.min(0)]],
      rpe: [5, [Validators.required, Validators.min(1), Validators.max(10)]]
    });
  }

  salvar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.enviando = true;
    const valor = this.form.getRawValue();

    this.trainingsApiService
      .cadastrar({
        atletaId: valor.atletaId ?? '',
        data: valor.data ?? '',
        tipo: Number(valor.tipo ?? 0),
        duracaoMinutos: Number(valor.duracaoMinutos ?? 0),
        distanciaKm: Number(valor.distanciaKm ?? 0),
        rpe: Number(valor.rpe ?? 0)
      })
      .subscribe({
        next: () => {
          this.snackBar.open('Treino cadastrado com sucesso.', 'Fechar', { duration: 3000 });
          void this.router.navigateByUrl('/dashboard');
        },
        error: () => {
          this.enviando = false;
          this.snackBar.open('Nao foi possivel cadastrar o treino. Tente novamente.', 'Fechar', { duration: 4000 });
        }
      });
  }
}
