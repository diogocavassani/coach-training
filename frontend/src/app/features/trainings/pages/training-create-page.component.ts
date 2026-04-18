import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatAutocompleteModule, MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatSelectModule } from '@angular/material/select';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

import { Student } from '../../students/models/student.model';
import { StudentsApiService } from '../../students/services/students-api.service';
import { TrainingsApiService } from '../services/trainings-api.service';

interface TipoDeTreinoOpcao {
  readonly valor: number;
  readonly nome: string;
  readonly descricao: string;
}

@Component({
  selector: 'app-training-create-page',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatAutocompleteModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSnackBarModule,
    MatSelectModule,
    RouterLink
  ],
  templateUrl: './training-create-page.component.html',
  styleUrl: './training-create-page.component.css'
})
export class TrainingCreatePageComponent implements OnInit {
  readonly form;

  readonly tiposDeTreino: ReadonlyArray<TipoDeTreinoOpcao> = [
    {
      valor: 0,
      nome: 'Leve',
      descricao: 'Treino de baixa intensidade para recuperacao ou manutencao de volume.'
    },
    {
      valor: 1,
      nome: 'Ritmo',
      descricao: 'Treino em ritmo sustentado, proximo da intensidade-alvo de prova.'
    },
    {
      valor: 2,
      nome: 'Intervalado',
      descricao: 'Treino com repeticoes mais intensas e pausas de recuperacao.'
    },
    {
      valor: 3,
      nome: 'Longo',
      descricao: 'Treino de maior duracao para desenvolver resistencia aerobica.'
    }
  ];

  atletas: Student[] = [];
  atletasFiltrados: Student[] = [];
  carregandoAtletas = true;
  enviando = false;
  mensagemErroAtletas = '';

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly trainingsApiService: TrainingsApiService,
    private readonly studentsApiService: StudentsApiService,
    private readonly snackBar: MatSnackBar,
    private readonly router: Router
  ) {
    this.form = this.formBuilder.nonNullable.group({
      buscaAtleta: ['', [Validators.required]],
      atletaId: ['', [Validators.required]],
      data: [this.obterDataAtual(), [Validators.required]],
      tipo: [0, [Validators.required]],
      duracaoMinutos: [30, [Validators.required, Validators.min(1)]],
      distanciaKm: [0, [Validators.required, Validators.min(0)]],
      rpe: [5, [Validators.required, Validators.min(1), Validators.max(10)]]
    });

    this.form.controls.buscaAtleta.valueChanges.subscribe((valorBusca) => {
      this.filtrarAtletas(valorBusca);

      if (typeof valorBusca === 'string') {
        const atletaSelecionado = this.atletas.find((atleta) => this.obterNomeAtleta(atleta.id) === valorBusca);
        this.form.controls.atletaId.setValue(atletaSelecionado?.id ?? '');
      }
    });
  }

  ngOnInit(): void {
    this.carregarAtletas();
  }

  get descricaoTipoSelecionado(): string {
    const tipoSelecionado = this.tiposDeTreino.find((tipo) => tipo.valor === this.form.controls.tipo.value);
    return tipoSelecionado?.descricao ?? '';
  }

  get podeSalvar(): boolean {
    return !this.enviando && !this.carregandoAtletas && this.atletas.length > 0;
  }

  selecionarAtleta(event: MatAutocompleteSelectedEvent): void {
    const atletaId = event.option.value as string;
    this.form.controls.atletaId.setValue(atletaId);
    this.form.controls.buscaAtleta.setValue(this.obterNomeAtleta(atletaId), { emitEvent: false });
  }

  obterNomeAtleta(atletaId: string): string {
    const atleta = this.atletas.find((item) => item.id === atletaId);
    if (!atleta) {
      return atletaId;
    }

    return atleta.email ? `${atleta.nome} (${atleta.email})` : atleta.nome;
  }

  salvar(): void {
    if (this.form.invalid || this.atletas.length === 0) {
      this.form.markAllAsTouched();
      return;
    }

    const valor = this.form.getRawValue();
    this.enviando = true;

    this.trainingsApiService
      .cadastrar({
        atletaId: valor.atletaId,
        data: valor.data,
        tipo: Number(valor.tipo),
        duracaoMinutos: Number(valor.duracaoMinutos),
        distanciaKm: Number(valor.distanciaKm),
        rpe: Number(valor.rpe)
      })
      .pipe(finalize(() => (this.enviando = false)))
      .subscribe({
        next: () => {
          this.snackBar.open('Treino cadastrado com sucesso.', 'Fechar', { duration: 3000 });
          void this.router.navigateByUrl('/dashboard');
        },
        error: (error: HttpErrorResponse) => {
          const mensagem = error.error?.erro ?? 'Nao foi possivel cadastrar o treino. Tente novamente.';
          this.snackBar.open(mensagem, 'Fechar', { duration: 4000 });
        }
      });
  }

  private carregarAtletas(): void {
    this.carregandoAtletas = true;
    this.mensagemErroAtletas = '';

    this.studentsApiService
      .listar()
      .pipe(finalize(() => (this.carregandoAtletas = false)))
      .subscribe({
        next: (atletas) => {
          this.atletas = [...atletas].sort((a, b) => a.nome.localeCompare(b.nome));
          this.filtrarAtletas(this.form.controls.buscaAtleta.value);
        },
        error: () => {
          this.atletas = [];
          this.atletasFiltrados = [];
          this.mensagemErroAtletas = 'Nao foi possivel carregar os alunos para selecao.';
        }
      });
  }

  private filtrarAtletas(valorBusca: string): void {
    const termo = valorBusca.trim().toLowerCase();
    if (!termo) {
      this.atletasFiltrados = [...this.atletas];
      return;
    }

    this.atletasFiltrados = this.atletas.filter((atleta) => {
      const nome = atleta.nome.toLowerCase();
      const email = (atleta.email ?? '').toLowerCase();
      return nome.includes(termo) || email.includes(termo);
    });
  }

  private obterDataAtual(): string {
    return new Date().toISOString().slice(0, 10);
  }
}
