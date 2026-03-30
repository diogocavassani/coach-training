import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';

import { TrainingsApiService } from './trainings-api.service';

describe('TrainingsApiService', () => {
  let service: TrainingsApiService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [TrainingsApiService, provideHttpClient(), provideHttpClientTesting()]
    });

    service = TestBed.inject(TrainingsApiService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('envia cadastro para o endpoint /api/treinos', () => {
    service
      .cadastrar({
        atletaId: '8c83f976-6ab9-46f4-b1e1-15a77d9d1f83',
        data: '2026-03-29',
        tipo: 2,
        duracaoMinutos: 50,
        distanciaKm: 10,
        rpe: 7
      })
      .subscribe();

    const req = httpMock.expectOne('/api/treinos');
    expect(req.request.method).toBe('POST');
    expect(req.request.body.tipo).toBe(2);
    req.flush({ id: '1' });
  });
});
