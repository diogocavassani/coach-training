import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';

import { DashboardApiService } from './dashboard-api.service';

describe('DashboardApiService', () => {
  let service: DashboardApiService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [DashboardApiService, provideHttpClient(), provideHttpClientTesting()]
    });

    service = TestBed.inject(DashboardApiService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('busca dashboard do atleta no endpoint esperado', () => {
    service.obterPorAtletaId('atleta-123').subscribe();

    const req = httpMock.expectOne('/api/dashboard/atleta/atleta-123');
    expect(req.request.method).toBe('GET');
    req.flush({});
  });

  it('busca resumo do dashboard do professor no endpoint esperado', () => {
    service.obterResumoProfessor().subscribe();

    const req = httpMock.expectOne('/api/dashboard/professor/resumo');
    expect(req.request.method).toBe('GET');
    req.flush({});
  });
});
