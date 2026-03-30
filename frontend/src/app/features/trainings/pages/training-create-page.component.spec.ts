import { of, throwError } from 'rxjs';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

import { TrainingCreatePageComponent } from './training-create-page.component';
import { TrainingsApiService } from '../services/trainings-api.service';
import { StudentsApiService } from '../../students/services/students-api.service';

describe('TrainingCreatePageComponent', () => {
  let fixture: ComponentFixture<TrainingCreatePageComponent>;
  let component: TrainingCreatePageComponent;
  let trainingsApiService: jasmine.SpyObj<TrainingsApiService>;
  let studentsApiService: jasmine.SpyObj<StudentsApiService>;
  let snackBar: jasmine.SpyObj<MatSnackBar>;
  let router: Router;

  beforeEach(async () => {
    trainingsApiService = jasmine.createSpyObj<TrainingsApiService>('TrainingsApiService', ['cadastrar']);
    studentsApiService = jasmine.createSpyObj<StudentsApiService>('StudentsApiService', ['listar']);
    snackBar = jasmine.createSpyObj<MatSnackBar>('MatSnackBar', ['open']);

    studentsApiService.listar.and.returnValue(
      of([
        { id: 'atleta-1', nome: 'Aluno Um', email: 'um@teste.com' },
        { id: 'atleta-2', nome: 'Aluno Dois' }
      ])
    );
    trainingsApiService.cadastrar.and.returnValue(
      of({
        id: 'treino-1',
        atletaId: 'atleta-1',
        data: '2026-03-29',
        tipo: 1,
        duracaoMinutos: 45,
        distanciaKm: 8,
        rpe: 6
      })
    );

    await TestBed.configureTestingModule({
      imports: [TrainingCreatePageComponent],
      providers: [
        { provide: TrainingsApiService, useValue: trainingsApiService },
        { provide: StudentsApiService, useValue: studentsApiService },
        { provide: MatSnackBar, useValue: snackBar },
        provideRouter([])
      ]
    }).compileComponents();

    router = TestBed.inject(Router);
    spyOn(router, 'navigateByUrl').and.resolveTo(true);

    fixture = TestBed.createComponent(TrainingCreatePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('carrega alunos no init para selecao', () => {
    expect(studentsApiService.listar).toHaveBeenCalled();
    expect(component.atletas.length).toBe(2);
    expect(component.atletasFiltrados.length).toBe(2);
  });

  it('nao envia cadastro quando formulario for invalido', () => {
    component.form.patchValue({
      atletaId: '',
      data: '',
      tipo: 0
    });

    component.salvar();

    expect(trainingsApiService.cadastrar).not.toHaveBeenCalled();
  });

  it('envia treino quando formulario estiver valido', () => {
    component.form.setValue({
      buscaAtleta: '',
      atletaId: 'atleta-1',
      data: '2026-03-29',
      tipo: 2,
      duracaoMinutos: 40,
      distanciaKm: 7.5,
      rpe: 6
    });

    component.salvar();

    expect(trainingsApiService.cadastrar).toHaveBeenCalledWith({
      atletaId: 'atleta-1',
      data: '2026-03-29',
      tipo: 2,
      duracaoMinutos: 40,
      distanciaKm: 7.5,
      rpe: 6
    });
    expect(router.navigateByUrl).toHaveBeenCalledWith('/dashboard');
  });

  it('exibe erro e nao redireciona quando API falha', () => {
    trainingsApiService.cadastrar.and.returnValue(throwError(() => new Error('Falha ao cadastrar treino')));
    component.form.setValue({
      buscaAtleta: '',
      atletaId: 'atleta-1',
      data: '2026-03-29',
      tipo: 1,
      duracaoMinutos: 30,
      distanciaKm: 5,
      rpe: 5
    });

    component.salvar();

    expect(trainingsApiService.cadastrar).toHaveBeenCalled();
    expect(component.enviando).toBeFalse();
    expect(router.navigateByUrl).not.toHaveBeenCalled();
  });
});
