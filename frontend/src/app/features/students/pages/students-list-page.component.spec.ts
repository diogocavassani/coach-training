import { of, throwError } from 'rxjs';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { By } from '@angular/platform-browser';

import { StudentsListPageComponent } from './students-list-page.component';
import { StudentsApiService } from '../services/students-api.service';

describe('StudentsListPageComponent', () => {
  let fixture: ComponentFixture<StudentsListPageComponent>;
  let component: StudentsListPageComponent;
  let studentsApiService: jasmine.SpyObj<StudentsApiService>;

  beforeEach(async () => {
    studentsApiService = jasmine.createSpyObj<StudentsApiService>('StudentsApiService', ['listar']);
    studentsApiService.listar.and.returnValue(
      of([
        {
          id: '1',
          nome: 'Aluno A',
          email: 'aluno.a@teste.com',
          nivelEsportivo: 'Intermediario',
          observacoesClinicas: 'Sem restricoes',
          dataCriacao: '2026-03-28T12:00:00Z'
        },
        { id: '2', nome: 'Aluno B' }
      ])
    );

    await TestBed.configureTestingModule({
      imports: [StudentsListPageComponent],
      providers: [{ provide: StudentsApiService, useValue: studentsApiService }, provideRouter([])]
    }).compileComponents();

    fixture = TestBed.createComponent(StudentsListPageComponent);
    component = fixture.componentInstance;
  });

  it('carrega alunos no init e finaliza estado de carregamento', () => {
    fixture.detectChanges();

    expect(studentsApiService.listar).toHaveBeenCalled();
    expect(component.carregando).toBeFalse();
    expect(component.alunos.length).toBe(2);
    expect(component.alunos[0].nome).toBe('Aluno A');
  });

  it('finaliza carregamento quando a listagem falha', () => {
    studentsApiService.listar.and.returnValue(throwError(() => new Error('Falha na listagem')));

    fixture.detectChanges();

    expect(component.carregando).toBeFalse();
    expect(component.alunos).toEqual([]);
  });

  it('formata data para pt-BR e retorna placeholder em entradas invalidas', () => {
    expect(component.formatarData()).toBe('-');
    expect(component.formatarData('valor-invalido')).toBe('-');
    expect(component.formatarData('2026-03-28T12:00:00Z')).toMatch(/^\d{2}\/\d{2}\/\d{4}$/);
  });

  it('exibe acao de ver dashboard para os alunos listados', () => {
    fixture.detectChanges();

    const botoesDashboard = fixture.debugElement.queryAll(By.css('a[mat-button]'));
    expect(botoesDashboard.length).toBe(2);
    expect(botoesDashboard[0].nativeElement.textContent).toContain('Abrir dashboard');
  });

  it('renderiza a lista como workspace de atletas', () => {
    fixture.detectChanges();

    const text = fixture.nativeElement.textContent;
    expect(text).toContain('Atletas do workspace');
    expect(text).toContain('Lista operacional');
  });
});
