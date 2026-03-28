import { of } from 'rxjs';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';

import { LoginPageComponent } from './login-page.component';
import { AuthService } from '../../../services/auth/auth.service';

describe('LoginPageComponent', () => {
  let fixture: ComponentFixture<LoginPageComponent>;
  let component: LoginPageComponent;
  let authService: jasmine.SpyObj<AuthService>;
  let router: Router;

  beforeEach(async () => {
    authService = jasmine.createSpyObj<AuthService>('AuthService', ['login']);
    authService.login.and.returnValue(of({ token: 'abc', expiraEmUtc: '2099-01-01T00:00:00.000Z' }));

    await TestBed.configureTestingModule({
      imports: [LoginPageComponent],
      providers: [{ provide: AuthService, useValue: authService }, provideRouter([])]
    }).compileComponents();

    router = TestBed.inject(Router);
    spyOn(router, 'navigateByUrl').and.resolveTo(true);

    fixture = TestBed.createComponent(LoginPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('inicia com formulario invalido', () => {
    expect(component.loginForm.valid).toBeFalse();
  });

  it('valida email e senha no submit', () => {
    component.loginForm.setValue({ email: 'email-invalido', senha: '123' });

    component.onSubmit();

    expect(component.loginForm.controls.email.hasError('email')).toBeTrue();
    expect(component.loginForm.controls.senha.hasError('minlength')).toBeTrue();
  });
});
