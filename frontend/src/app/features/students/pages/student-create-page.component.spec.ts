import { of, throwError } from 'rxjs';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

import { StudentCreatePageComponent } from './student-create-page.component';
import { StudentsApiService } from '../services/students-api.service';

describe('StudentCreatePageComponent', () => {
  let fixture: ComponentFixture<StudentCreatePageComponent>;
  let component: StudentCreatePageComponent;
  let studentsApiService: jasmine.SpyObj<StudentsApiService>;
  let snackBar: jasmine.SpyObj<MatSnackBar>;
  let router: Router;

  beforeEach(async () => {
    studentsApiService = jasmine.createSpyObj<StudentsApiService>('StudentsApiService', ['cadastrar']);
    snackBar = jasmine.createSpyObj<MatSnackBar>('MatSnackBar', ['open']);
    studentsApiService.cadastrar.and.returnValue(of({ id: '1', nome: 'Aluno Teste' }));

    await TestBed.configureTestingModule({
      imports: [StudentCreatePageComponent],
      providers: [
        { provide: StudentsApiService, useValue: studentsApiService },
        { provide: MatSnackBar, useValue: snackBar },
        provideRouter([])
      ]
    }).compileComponents();

    router = TestBed.inject(Router);
    spyOn(router, 'navigateByUrl').and.resolveTo(true);

    fixture = TestBed.createComponent(StudentCreatePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('nao envia cadastro quando formulario estiver invalido', () => {
    component.form.setValue({
      nome: 'A',
      email: 'email-invalido',
      observacoesClinicas: '',
      nivelEsportivo: ''
    });

    component.salvar();

    expect(studentsApiService.cadastrar).not.toHaveBeenCalled();
  });

  it('envia payload normalizado e redireciona no sucesso', () => {
    component.form.setValue({
      nome: '  Aluno Teste  ',
      email: 'aluno@teste.com',
      observacoesClinicas: '  Sem restricoes  ',
      nivelEsportivo: '  Intermediario  '
    });
    expect(component.form.valid).toBeTrue();

    component.salvar();

    expect(studentsApiService.cadastrar).toHaveBeenCalledWith({
      nome: 'Aluno Teste',
      email: 'aluno@teste.com',
      observacoesClinicas: 'Sem restricoes',
      nivelEsportivo: 'Intermediario'
    });
    expect(router.navigateByUrl).toHaveBeenCalledWith('/dashboard/alunos');
  });

  it('mantem usuario na tela e exibe erro quando cadastro falha', () => {
    studentsApiService.cadastrar.and.returnValue(throwError(() => new Error('Falha no cadastro')));
    component.form.setValue({
      nome: 'Aluno Teste',
      email: 'aluno@teste.com',
      observacoesClinicas: '',
      nivelEsportivo: ''
    });
    expect(component.form.valid).toBeTrue();

    component.salvar();

    expect(studentsApiService.cadastrar).toHaveBeenCalled();
    expect(component.enviando).toBeFalse();
    expect(router.navigateByUrl).not.toHaveBeenCalled();
  });
});
