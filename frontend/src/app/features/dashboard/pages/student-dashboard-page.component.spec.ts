import { of, throwError } from 'rxjs';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap, provideRouter } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

import { StudentDashboardPageComponent } from './student-dashboard-page.component';
import { DashboardApiService } from '../services/dashboard-api.service';
import { DashboardAtleta } from '../models/dashboard.model';

describe('StudentDashboardPageComponent', () => {
  let fixture: ComponentFixture<StudentDashboardPageComponent>;
  let component: StudentDashboardPageComponent;
  let dashboardApiService: jasmine.SpyObj<DashboardApiService>;
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

    snackBar = jasmine.createSpyObj<MatSnackBar>('MatSnackBar', ['open']);

    await TestBed.configureTestingModule({
      imports: [StudentDashboardPageComponent],
      providers: [
        { provide: DashboardApiService, useValue: dashboardApiService },
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
    expect(component.dashboard?.atletaId).toBe('atleta-1');
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
