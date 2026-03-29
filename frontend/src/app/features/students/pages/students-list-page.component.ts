import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink } from '@angular/router';

import { Student } from '../models/student.model';
import { StudentsApiService } from '../services/students-api.service';

@Component({
  selector: 'app-students-list-page',
  standalone: true,
  imports: [CommonModule, MatButtonModule, RouterLink],
  templateUrl: './students-list-page.component.html',
  styleUrl: './students-list-page.component.css'
})
export class StudentsListPageComponent implements OnInit {
  alunos: Student[] = [];
  carregando = true;

  constructor(private readonly studentsApiService: StudentsApiService) {}

  ngOnInit(): void {
    this.studentsApiService.listar().subscribe({
      next: (response) => {
        this.alunos = response;
        this.carregando = false;
      },
      error: () => {
        this.carregando = false;
      }
    });
  }

  formatarData(dataCriacao?: string): string {
    if (!dataCriacao) {
      return '-';
    }

    const data = new Date(dataCriacao);
    if (Number.isNaN(data.getTime())) {
      return '-';
    }

    return data.toLocaleDateString('pt-BR');
  }
}
