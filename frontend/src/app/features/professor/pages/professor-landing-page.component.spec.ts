import { of, throwError } from 'rxjs';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';

import { ProfessorLandingPageComponent } from './professor-landing-page.component';
import { ProfessorApiService } from '../../../services/api/professor-api.service';
import { AuthService } from '../../../services/auth/auth.service';

describe('ProfessorLandingPageComponent', () => {
  let fixture: ComponentFixture<ProfessorLandingPageComponent>;
  let component: ProfessorLandingPageComponent;
  let professorApiService: jasmine.SpyObj<ProfessorApiService>;
  let authService: jasmine.SpyObj<AuthService>;
  let router: Router;

  beforeEach(async () => {
    professorApiService = jasmine.createSpyObj<ProfessorApiService>('ProfessorApiService', ['cadastrar']);
    authService = jasmine.createSpyObj<AuthService>('AuthService', ['login']);

    professorApiService.cadastrar.and.returnValue(
      of({
        id: '00000000-0000-0000-0000-000000000001',
        nome: 'Professor Teste',
        email: 'professor@coach.com',
        dataCriacao: '2026-03-28T00:00:00.000Z'
      })
    );
    authService.login.and.returnValue(of({ token: 'abc', expiraEmUtc: '2099-01-01T00:00:00.000Z' }));

    await TestBed.configureTestingModule({
      imports: [ProfessorLandingPageComponent],
      providers: [
        { provide: ProfessorApiService, useValue: professorApiService },
        { provide: AuthService, useValue: authService },
        provideRouter([])
      ]
    }).compileComponents();

    router = TestBed.inject(Router);
    spyOn(router, 'navigateByUrl').and.resolveTo(true);

    fixture = TestBed.createComponent(ProfessorLandingPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('renderiza a arquitetura de informacao Executive Signal com as secoes esperadas', () => {
    const page = fixture.nativeElement as HTMLElement;
    const hero = page.querySelector<HTMLElement>('section.hero');
    const landingContent = page.querySelector<HTMLElement>('main.landing-content');
    const topLevelSections = Array.from(landingContent?.querySelectorAll(':scope > section') ?? []);
    const [signals, trainerFlow, signup] = topLevelSections as HTMLElement[];
    const heroSection = hero as HTMLElement;
    const landingMain = landingContent as HTMLElement;

    expect(hero).withContext('hero section').not.toBeNull();
    expect(landingContent).withContext('landing content main').not.toBeNull();
    expect(heroSection.compareDocumentPosition(landingMain) & Node.DOCUMENT_POSITION_FOLLOWING).not.toBe(0);
    expect(hero?.querySelector('.section-label')?.textContent).toContain('Workspace premium para treinadores');
    expect(hero?.querySelector('h1')?.textContent).toContain(
      'Controle o ciclo do atleta com leitura precisa e decisoes mais confiantes.'
    );

    expect(topLevelSections.map((section) => section.id)).toEqual(['sinais', 'fluxo', 'cadastro']);

    expect(signals).withContext('signals section').not.toBeNull();
    expect(signals.id).toBe('sinais');
    expect(signals?.querySelector('h2')?.textContent).toContain('Sinais que importam');

    expect(trainerFlow).withContext('trainer flow section').not.toBeNull();
    expect(trainerFlow.id).toBe('fluxo');
    expect(trainerFlow?.querySelector('h2')?.textContent).toContain('Fluxo do treinador');

    expect(signup).withContext('signup section').not.toBeNull();
    expect(signup.id).toBe('cadastro');
    expect(signup?.querySelector('.section-label')?.textContent).toContain('Criar conta de professor');
    expect(signup?.querySelector('form')).withContext('signup form').not.toBeNull();
  });

  it('reserva espaco de scroll para os links internos sob o header fixo', () => {
    const page = fixture.nativeElement as HTMLElement;
    const signals = page.querySelector<HTMLElement>('#sinais') as HTMLElement;
    const signup = page.querySelector<HTMLElement>('#cadastro') as HTMLElement;

    expect(getComputedStyle(signals).scrollMarginTop).not.toBe('0px');
    expect(getComputedStyle(signup).scrollMarginTop).not.toBe('0px');
  });

  it('inicia com formulario invalido', () => {
    expect(component.cadastroForm.valid).toBeFalse();
  });

  it('valida campos obrigatorios e limites minimos', () => {
    component.cadastroForm.setValue({ nome: 'A', email: 'invalido', senha: '123' });

    component.onSubmit();

    expect(component.cadastroForm.controls.nome.hasError('minlength')).toBeTrue();
    expect(component.cadastroForm.controls.email.hasError('email')).toBeTrue();
    expect(component.cadastroForm.controls.senha.hasError('minlength')).toBeTrue();
    expect(professorApiService.cadastrar).not.toHaveBeenCalled();
  });

  it('realiza cadastro, autentica e redireciona para dashboard', () => {
    component.cadastroForm.setValue({
      nome: 'Professor Teste',
      email: 'professor@coach.com',
      senha: '123456'
    });

    component.onSubmit();

    expect(professorApiService.cadastrar).toHaveBeenCalledWith({
      nome: 'Professor Teste',
      email: 'professor@coach.com',
      senha: '123456'
    });
    expect(authService.login).toHaveBeenCalledWith({
      email: 'professor@coach.com',
      senha: '123456'
    });
    expect(router.navigateByUrl).toHaveBeenCalledWith('/dashboard');
  });

  it('exibe mensagem quando cadastro funciona mas auto-login falha', () => {
    authService.login.and.returnValue(throwError(() => new Error('Falha no login')));
    component.cadastroForm.setValue({
      nome: 'Professor Teste',
      email: 'professor@coach.com',
      senha: '123456'
    });

    component.onSubmit();

    expect(component.mensagemErro).toContain('Cadastro concluido, mas nao foi possivel entrar automaticamente');
  });
});
