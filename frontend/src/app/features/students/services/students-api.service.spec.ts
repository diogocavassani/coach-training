import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';

import { StudentsApiService } from './students-api.service';

describe('StudentsApiService', () => {
  let service: StudentsApiService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [StudentsApiService, provideHttpClient(), provideHttpClientTesting()]
    });

    service = TestBed.inject(StudentsApiService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('envia payload completo de cadastro sem mapeamentos provisorios', () => {
    service
      .cadastrar({
        nome: 'Aluno Teste',
        email: 'aluno@teste.com',
        observacoesClinicas: 'Sem restricoes',
        nivelEsportivo: 'Intermediario'
      })
      .subscribe();

    const req = httpMock.expectOne('/api/Atleta');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({
      nome: 'Aluno Teste',
      email: 'aluno@teste.com',
      observacoesClinicas: 'Sem restricoes',
      nivelEsportivo: 'Intermediario'
    });
    req.flush({ id: '1', nome: 'Aluno Teste' });
  });

  it('envia apenas nome quando campos opcionais nao informados', () => {
    service.cadastrar({ nome: 'Aluno Sem Email' }).subscribe();

    const req = httpMock.expectOne('/api/Atleta');
    expect(req.request.body).toEqual({
      nome: 'Aluno Sem Email'
    });
    req.flush({ id: '2', nome: 'Aluno Sem Email' });
  });

  it('lista alunos com GET no endpoint de atleta', () => {
    service.listar().subscribe();

    const req = httpMock.expectOne('/api/Atleta');
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });
});
