import { of, throwError } from 'rxjs';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { DashboardPageComponent } from './dashboard-page.component';
import { DashboardApiService } from '../services/dashboard-api.service';
import { DashboardProfessorResumo } from '../models/dashboard.model';

describe('DashboardPageComponent', () => {
  let fixture: ComponentFixture<DashboardPageComponent>;
  let component: DashboardPageComponent;
  let dashboardApiService: jasmine.SpyObj<DashboardApiService>;

  const resumoMock: DashboardProfessorResumo = {
    totalAtletas: 3,
    atletasEmAtencao: 1,
    atletasEmRisco: 1,
    atletasEmTaper: 1,
    treinosRegistradosNaSemana: 5,
    atletasComPlanejamentoConfigurado: 2,
    dataUltimaAtualizacao: '2026-04-01T12:00:00Z',
    atletasPrioritarios: [
      {
        atletaId: 'atleta-risco',
        nome: 'Atleta Risco',
        statusRisco: 2,
        emJanelaDeTaper: false,
        proximaProva: null,
        cargaSemanal: 1380,
        aderenciaPlanejamentoPercentual: 50
      }
    ],
    treinosRecentes: [
      {
        atletaId: 'atleta-risco',
        nomeAtleta: 'Atleta Risco',
        data: '2026-04-01',
        tipo: 2,
        carga: 810
      }
    ]
  };

  beforeEach(async () => {
    dashboardApiService = jasmine.createSpyObj<DashboardApiService>('DashboardApiService', ['obterResumoProfessor']);
    dashboardApiService.obterResumoProfessor.and.returnValue(of(resumoMock));

    await TestBed.configureTestingModule({
      imports: [DashboardPageComponent],
      providers: [{ provide: DashboardApiService, useValue: dashboardApiService }, provideRouter([])]
    }).compileComponents();

    fixture = TestBed.createComponent(DashboardPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('carrega o resumo operacional do professor', () => {
    expect(dashboardApiService.obterResumoProfessor).toHaveBeenCalled();
    expect(component.resumo?.totalAtletas).toBe(3);
    expect(component.carregando).toBeFalse();
    expect(fixture.nativeElement.textContent).toContain('Atletas prioritarios');
    expect(fixture.nativeElement.textContent).toContain('Atleta Risco');
  });

  it('mostra estado vazio quando nao houver atletas cadastrados', () => {
    dashboardApiService.obterResumoProfessor.and.returnValue(
      of({
        ...resumoMock,
        totalAtletas: 0,
        atletasEmAtencao: 0,
        atletasEmRisco: 0,
        atletasEmTaper: 0,
        treinosRegistradosNaSemana: 0,
        atletasComPlanejamentoConfigurado: 0,
        atletasPrioritarios: [],
        treinosRecentes: []
      })
    );

    fixture = TestBed.createComponent(DashboardPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Seu painel ainda esta vazio');
  });

  it('exibe erro quando a API falha', () => {
    dashboardApiService.obterResumoProfessor.and.returnValue(
      throwError(() => ({ error: { erro: 'Falha ao carregar home' } }))
    );

    fixture = TestBed.createComponent(DashboardPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();

    expect(component.resumo).toBeNull();
    expect(component.mensagemErro).toBe('Falha ao carregar home');
  });
});
