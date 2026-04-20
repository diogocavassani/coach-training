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

  it('renderiza a nova hierarquia editorial de autenticacao', () => {
    const host: HTMLElement = fixture.nativeElement;
    const shell = host.querySelector<HTMLElement>('.login-shell.page-surface');
    const intro = host.querySelector<HTMLElement>('.login-intro');
    const form = host.querySelector<HTMLElement>('form.login-form');
    const introLabel = intro?.querySelector<HTMLElement>('.section-label');
    const introTitle = intro?.querySelector<HTMLHeadingElement>('h1');
    const introDescription = intro?.querySelector<HTMLElement>('.login-description');
    const registerCta = intro?.querySelector<HTMLAnchorElement>('a.register-link');
    const formHeading = form?.querySelector<HTMLHeadingElement>('.form-header h2');

    expect(shell).not.toBeNull();
    expect(intro).not.toBeNull();
    expect(form).not.toBeNull();
    expect(shell?.contains(intro as Node)).toBeTrue();
    expect(shell?.contains(form as Node)).toBeTrue();
    expect(intro?.contains(form as Node)).toBeFalse();

    expect(introLabel?.textContent?.trim()).toBe('CoachTraining');
    expect(introTitle?.textContent?.trim()).toBe('Acesse o workspace do treinador.');
    expect(introDescription?.textContent).toContain('Leituras de risco, taper e aderencia em um unico lugar');
    expect(registerCta?.textContent?.trim()).toBe('Ainda nao tem conta? Criar acesso');
    expect(formHeading?.textContent?.trim()).toBe('Entrar no workspace');
  });
});
