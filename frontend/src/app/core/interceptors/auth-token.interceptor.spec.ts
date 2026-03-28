import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';

import { AuthService } from '../../services/auth/auth.service';
import { authTokenInterceptor } from './auth-token.interceptor';

describe('authTokenInterceptor', () => {
  let httpClient: HttpClient;
  let httpMock: HttpTestingController;
  let authService: jasmine.SpyObj<AuthService>;

  beforeEach(() => {
    authService = jasmine.createSpyObj<AuthService>('AuthService', ['getToken']);

    TestBed.configureTestingModule({
      providers: [
        { provide: AuthService, useValue: authService },
        provideHttpClient(withInterceptors([authTokenInterceptor])),
        provideHttpClientTesting()
      ]
    });

    httpClient = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('adiciona bearer token quando sessao existe', () => {
    authService.getToken.and.returnValue('token-teste');

    httpClient.get('/api/atleta/123').subscribe();

    const req = httpMock.expectOne('/api/atleta/123');
    expect(req.request.headers.get('Authorization')).toBe('Bearer token-teste');
    req.flush({});
  });

  it('mantem request sem header quando nao existe token', () => {
    authService.getToken.and.returnValue(null);

    httpClient.get('/api/atleta/123').subscribe();

    const req = httpMock.expectOne('/api/atleta/123');
    expect(req.request.headers.has('Authorization')).toBeFalse();
    req.flush({});
  });
});
