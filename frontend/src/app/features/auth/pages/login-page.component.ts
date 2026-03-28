import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule, MatButtonModule, MatFormFieldModule, MatInputModule],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.css'
})
export class LoginPageComponent {
  readonly loginForm;
  enviouFormulario = false;

  constructor(private readonly formBuilder: FormBuilder) {
    this.loginForm = this.formBuilder.nonNullable.group({
      email: ['', [Validators.required, Validators.email]],
      senha: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit(): void {
    this.enviouFormulario = true;
    this.loginForm.markAllAsTouched();
  }
}
