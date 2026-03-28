import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';

import { AuthService } from './auth.service';
import { LoginRequest, LoginResponse } from './auth.models';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    localStorage.clear();

    TestBed.configureTestingModule({
      providers: [AuthService, provideHttpClient(), provideHttpClientTesting()]
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('persiste token e expiracao ao realizar login', () => {
    const payload: LoginRequest = { email: 'professor@coach.com', senha: '123456' };
    const response: LoginResponse = {
      token: 'token.jwt.teste',
      expiraEmUtc: '2099-01-01T00:00:00.000Z'
    };

    service.login(payload).subscribe((result) => {
      expect(result).toEqual(response);
    });

    const req = httpMock.expectOne('/auth/login');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(payload);
    req.flush(response);

    expect(service.getToken()).toBe(response.token);
    expect(service.getExpiration()?.toISOString()).toBe(response.expiraEmUtc);
    expect(service.isAuthenticated()).toBeTrue();
  });

  it('remove dados de sessao no logout', () => {
    localStorage.setItem('coachtraining.auth.token', 'abc');
    localStorage.setItem('coachtraining.auth.expiration', '2099-01-01T00:00:00.000Z');

    service.logout();

    expect(service.getToken()).toBeNull();
    expect(service.getExpiration()).toBeNull();
    expect(service.isAuthenticated()).toBeFalse();
  });

  it('retorna nao autenticado quando expirado', () => {
    localStorage.setItem('coachtraining.auth.token', 'abc');
    localStorage.setItem('coachtraining.auth.expiration', '2000-01-01T00:00:00.000Z');

    expect(service.isAuthenticated()).toBeFalse();
  });
});
