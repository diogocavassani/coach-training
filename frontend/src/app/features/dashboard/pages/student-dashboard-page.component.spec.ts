import { of, throwError } from 'rxjs';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap, provideRouter } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HttpErrorResponse } from '@angular/common/http';

import { StudentDashboardPageComponent } from './student-dashboard-page.component';
import { DashboardApiService } from '../services/dashboard-api.service';
import { DashboardAtleta } from '../models/dashboard.model';
import { StudentsApiService } from '../../students/services/students-api.service';

describe('StudentDashboardPageComponent', () => {
  let fixture: ComponentFixture<StudentDashboardPageComponent>;
  let component: StudentDashboardPageComponent;
  let dashboardApiService: jasmine.SpyObj<DashboardApiService>;
  let studentsApiService: jasmine.SpyObj<StudentsApiService>;
  let snackBar: jasmine.SpyObj<MatSnackBar>;

  const mockDashboard: DashboardAtleta = {
    atletaId: 'atleta-1',
    nome: 'Aluno Dashboard',
    cargaUltimaSessao: 320,
    cargaSemanal: 1100,
    cargaSemanalAnterior: 980,
    cargaAguda: 1100,
    cargaCronica: 1020,
    acwr: 1.08,
    deltaPercentualSemanal: 12.2,
    faseAtual: 1,
    statusRisco: 0,
    emJanelaDeTaper: false,
    proximaProva: null,
    reducaoVolumeTaper: null,
    dataUltimaAtualizacao: '2026-03-30T10:00:00Z',
    observacoesClin: null,
    nivelAtleta: 'Intermediario',
    insights: ['Carga sob controle.'],
    serieCargaSemanal: [
      { semanaInicio: '2026-03-16', semanaFim: '2026-03-22', valor: 980 },
      { semanaInicio: '2026-03-23', semanaFim: '2026-03-29', valor: 1100 }
    ],
    seriePaceSemanal: [
      { semanaInicio: '2026-03-16', semanaFim: '2026-03-22', valorMinPorKm: 5.1 },
      { semanaInicio: '2026-03-23', semanaFim: '2026-03-29', valorMinPorKm: 4.95 }
    ],
    treinosJanela: [
      {
        id: 'treino-1',
        data: '2026-03-28',
        tipo: 2,
        duracaoMinutos: 50,
        distanciaKm: 10,
        rpe: 7,
        carga: 350,
        paceMinPorKm: 5
      }
    ]
  };

  beforeEach(async () => {
    dashboardApiService = jasmine.createSpyObj<DashboardApiService>('DashboardApiService', ['obterPorAtletaId']);
    dashboardApiService.obterPorAtletaId.and.returnValue(of(mockDashboard));
    studentsApiService = jasmine.createSpyObj<StudentsApiService>('StudentsApiService', [
      'obterProvaAlvo',
      'salvarProvaAlvo',
      'obterPlanejamentoBase',
      'salvarPlanejamentoBase'
    ]);
    studentsApiService.obterProvaAlvo.and.returnValue(
      of({
        id: 'prova-1',
        atletaId: 'atleta-1',
        dataProva: '2026-05-01',
        distanciaKm: 21.1,
        objetivo: 'Completar forte'
      })
    );
    studentsApiService.obterPlanejamentoBase.and.returnValue(
      of({
        atletaId: 'atleta-1',
        treinosPlanejadosPorSemana: 5
      })
    );
    studentsApiService.salvarPlanejamentoBase.and.returnValue(
      of({
        atletaId: 'atleta-1',
        treinosPlanejadosPorSemana: 5
      })
    );
    studentsApiService.salvarProvaAlvo.and.returnValue(
      of({
        id: 'prova-1',
        atletaId: 'atleta-1',
        dataProva: '2026-05-01',
        distanciaKm: 21.1,
        objetivo: 'Completar forte'
      })
    );

    snackBar = jasmine.createSpyObj<MatSnackBar>('MatSnackBar', ['open']);

    await TestBed.configureTestingModule({
      imports: [StudentDashboardPageComponent],
      providers: [
        { provide: DashboardApiService, useValue: dashboardApiService },
        { provide: StudentsApiService, useValue: studentsApiService },
        { provide: MatSnackBar, useValue: snackBar },
        provideRouter([]),
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: convertToParamMap({ id: 'atleta-1' })
            }
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(StudentDashboardPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('carrega dashboard pelo id da rota', () => {
    expect(dashboardApiService.obterPorAtletaId).toHaveBeenCalledWith('atleta-1');
    expect(studentsApiService.obterProvaAlvo).toHaveBeenCalledWith('atleta-1');
    expect(studentsApiService.obterPlanejamentoBase).toHaveBeenCalledWith('atleta-1');
    expect(component.dashboard?.atletaId).toBe('atleta-1');
    expect(component.provaAlvoForm.getRawValue()).toEqual({
      dataProva: '2026-05-01',
      distanciaKm: 21.1,
      objetivo: 'Completar forte'
    });
    expect(component.planejamentoBaseForm.getRawValue()).toEqual({
      treinosPlanejadosPorSemana: 5
    });
    expect(component.carregando).toBeFalse();
  });

  it('exibe erro quando API falha', async () => {
    dashboardApiService.obterPorAtletaId.and.returnValue(
      throwError(() => ({ error: { erro: 'Falha ao carregar dashboard' } }))
    );

    fixture = TestBed.createComponent(StudentDashboardPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();

    expect(component.dashboard).toBeNull();
    expect(component.mensagemErro).toBe('Falha ao carregar dashboard');
    expect(component.carregando).toBeFalse();
  });

  it('mantem formulario vazio quando prova-alvo ainda nao foi cadastrada', () => {
    studentsApiService.obterProvaAlvo.and.returnValue(
      throwError(() => new HttpErrorResponse({ status: 404, error: { erro: 'Nao encontrada' } }))
    );

    fixture = TestBed.createComponent(StudentDashboardPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();

    expect(component.provaAlvoForm.getRawValue()).toEqual({
      dataProva: '',
      distanciaKm: null,
      objetivo: ''
    });
    expect(component.mensagemErroProvaAlvo).toBe('');
  });

  it('mantem formulario de planejamento vazio quando o atleta ainda nao possui base planejada', () => {
    studentsApiService.obterPlanejamentoBase.and.returnValue(
      throwError(() => new HttpErrorResponse({ status: 404, error: { erro: 'Nao encontrado' } }))
    );

    fixture = TestBed.createComponent(StudentDashboardPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();

    expect(component.planejamentoBaseForm.getRawValue()).toEqual({
      treinosPlanejadosPorSemana: null
    });
    expect(component.mensagemErroPlanejamentoBase).toBe('');
  });

  it('salva prova-alvo e recarrega o dashboard', () => {
    dashboardApiService.obterPorAtletaId.calls.reset();
    component.provaAlvoForm.setValue({
      dataProva: '2026-05-01',
      distanciaKm: 21.1,
      objetivo: '  Completar forte  '
    });

    component.salvarProvaAlvo();

    expect(studentsApiService.salvarProvaAlvo).toHaveBeenCalledWith('atleta-1', {
      dataProva: '2026-05-01',
      distanciaKm: 21.1,
      objetivo: 'Completar forte'
    });
    expect(dashboardApiService.obterPorAtletaId).toHaveBeenCalledWith('atleta-1');
    expect(component.mensagemErroProvaAlvo).toBe('');
  });

  it('salva planejamento base e recarrega o dashboard', () => {
    dashboardApiService.obterPorAtletaId.calls.reset();
    component.planejamentoBaseForm.setValue({
      treinosPlanejadosPorSemana: 6
    });

    component.salvarPlanejamentoBase();

    expect(studentsApiService.salvarPlanejamentoBase).toHaveBeenCalledWith('atleta-1', {
      treinosPlanejadosPorSemana: 6
    });
    expect(dashboardApiService.obterPorAtletaId).toHaveBeenCalledWith('atleta-1');
    expect(component.mensagemErroPlanejamentoBase).toBe('');
  });

  it('exporta excel com os treinos carregados', () => {
    const salvarWorkbookSpy = spyOn<any>(component, 'salvarWorkbook').and.callFake(() => {});

    component.exportarExcel();

    expect(salvarWorkbookSpy).toHaveBeenCalled();
  });

  it('exporta pdf com os dados do dashboard', () => {
    const salvarPdfSpy = spyOn<any>(component, 'salvarDocumentoPdf').and.callFake(() => {});

    component.exportarPdf();

    expect(salvarPdfSpy).toHaveBeenCalled();
  });
});
