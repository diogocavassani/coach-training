import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { AppShellComponent } from './app-shell.component';
import { AuthService } from '../../services/auth/auth.service';

@Component({
  standalone: true,
  template: '<p>Conteudo</p>'
})
class DummyPageComponent {}

describe('AppShellComponent', () => {
  let fixture: ComponentFixture<AppShellComponent>;
  let authService: jasmine.SpyObj<AuthService>;

  beforeEach(async () => {
    authService = jasmine.createSpyObj<AuthService>('AuthService', ['logout']);

    await TestBed.configureTestingModule({
      imports: [AppShellComponent],
      providers: [
        { provide: AuthService, useValue: authService },
        provideRouter([
          {
            path: '',
            component: DummyPageComponent
          }
        ])
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AppShellComponent);
    fixture.detectChanges();
  });

  it('renderiza a navegacao principal do workspace do professor', () => {
    const text = fixture.nativeElement.textContent;

    expect(text).toContain('CoachTraining');
    expect(text).toContain('Workspace do professor');
    expect(text).toContain('Leituras');
    expect(text).toContain('Novo treino');
  });

  it('mantem o shell generico sem reutilizar o titulo da home do dashboard', () => {
    const headerTitle = fixture.nativeElement.querySelector('.header-copy h1') as HTMLHeadingElement | null;

    expect(headerTitle?.textContent?.trim()).toBe('Workspace do professor');
    expect(fixture.nativeElement.textContent).not.toContain('Leituras do workspace');
  });
});
