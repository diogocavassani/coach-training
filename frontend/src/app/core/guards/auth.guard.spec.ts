import { TestBed } from '@angular/core/testing';
import { Router, UrlTree } from '@angular/router';

import { AuthService } from '../../services/auth/auth.service';
import { authGuard } from './auth.guard';

describe('authGuard', () => {
  let authService: jasmine.SpyObj<AuthService>;
  let router: jasmine.SpyObj<Router>;

  beforeEach(() => {
    authService = jasmine.createSpyObj<AuthService>('AuthService', ['isAuthenticated', 'logout']);
    router = jasmine.createSpyObj<Router>('Router', ['createUrlTree']);

    TestBed.configureTestingModule({
      providers: [
        { provide: AuthService, useValue: authService },
        { provide: Router, useValue: router }
      ]
    });
  });

  it('permite acesso quando autenticado', () => {
    authService.isAuthenticated.and.returnValue(true);

    const result = TestBed.runInInjectionContext(() => authGuard({} as never, {} as never));

    expect(result).toBeTrue();
    expect(authService.logout).not.toHaveBeenCalled();
  });

  it('redireciona para login quando nao autenticado', () => {
    const loginTree = {} as UrlTree;
    authService.isAuthenticated.and.returnValue(false);
    router.createUrlTree.and.returnValue(loginTree);

    const result = TestBed.runInInjectionContext(() => authGuard({} as never, {} as never));

    expect(authService.logout).toHaveBeenCalled();
    expect(router.createUrlTree).toHaveBeenCalledWith(['/login']);
    expect(result).toBe(loginTree);
  });
});
