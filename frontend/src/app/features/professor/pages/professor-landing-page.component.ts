import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-professor-landing-page',
  standalone: true,
  imports: [RouterLink, MatButtonModule],
  templateUrl: './professor-landing-page.component.html',
  styleUrl: './professor-landing-page.component.css'
})
export class ProfessorLandingPageComponent {}
