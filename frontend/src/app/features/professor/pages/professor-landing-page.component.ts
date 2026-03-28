import { Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-professor-landing-page',
  standalone: true,
  imports: [RouterLink, MatButtonModule, ReactiveFormsModule, MatFormFieldModule, MatInputModule],
  templateUrl: './professor-landing-page.component.html',
  styleUrl: './professor-landing-page.component.css'
})
export class ProfessorLandingPageComponent {
  readonly cadastroForm;

  enviouFormulario = false;

  constructor(private readonly formBuilder: FormBuilder) {
    this.cadastroForm = this.formBuilder.group({
      nome: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      senha: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit(): void {
    this.enviouFormulario = true;
    this.cadastroForm.markAllAsTouched();
  }
}
