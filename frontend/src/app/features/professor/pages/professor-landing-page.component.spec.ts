import { of } from 'rxjs';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { ProfessorLandingPageComponent } from './professor-landing-page.component';
import { ProfessorApiService } from '../../../services/api/professor-api.service';

describe('ProfessorLandingPageComponent', () => {
  let fixture: ComponentFixture<ProfessorLandingPageComponent>;
  let component: ProfessorLandingPageComponent;
  let professorApiService: jasmine.SpyObj<ProfessorApiService>;

  beforeEach(async () => {
    professorApiService = jasmine.createSpyObj<ProfessorApiService>('ProfessorApiService', ['cadastrar']);
    professorApiService.cadastrar.and.returnValue(
      of({
        id: '00000000-0000-0000-0000-000000000001',
        nome: 'Professor Teste',
        email: 'professor@coach.com',
        dataCriacao: '2026-03-28T00:00:00.000Z'
      })
    );

    await TestBed.configureTestingModule({
      imports: [ProfessorLandingPageComponent],
      providers: [{ provide: ProfessorApiService, useValue: professorApiService }, provideRouter([])]
    }).compileComponents();

    fixture = TestBed.createComponent(ProfessorLandingPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
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
});
