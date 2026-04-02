import { HttpErrorResponse } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink } from '@angular/router';

import {
  DashboardProfessorAtletaPrioritario,
  DashboardProfessorResumo,
  DashboardProfessorTreinoRecente
} from '../models/dashboard.model';
import { DashboardApiService } from '../services/dashboard-api.service';

@Component({
  selector: 'app-dashboard-page',
  standalone: true,
  imports: [CommonModule, MatButtonModule, RouterLink],
  templateUrl: './dashboard-page.component.html',
  styleUrl: './dashboard-page.component.css'
})
export class DashboardPageComponent implements OnInit {
  resumo: DashboardProfessorResumo | null = null;
  carregando = true;
  mensagemErro = '';

  constructor(private readonly dashboardApiService: DashboardApiService) {}

  ngOnInit(): void {
    this.dashboardApiService.obterResumoProfessor().subscribe({
      next: (resumo) => {
        this.resumo = resumo;
        this.carregando = false;
      },
      error: (error: HttpErrorResponse) => {
        this.resumo = null;
        this.carregando = false;
        this.mensagemErro = error.error?.erro ?? 'Nao foi possivel carregar o painel inicial.';
      }
    });
  }

  formatarData(dataIso: string | null | undefined): string {
    if (!dataIso) {
      return '-';
    }

    const partes = dataIso.split('-');
    if (partes.length !== 3) {
      return '-';
    }

    const [ano, mes, dia] = partes;
    return `${dia.padStart(2, '0')}/${mes.padStart(2, '0')}/${ano}`;
  }

  descricaoStatusRisco(statusRisco: number): string {
    switch (statusRisco) {
      case 0:
        return 'Normal';
      case 1:
        return 'Atencao';
      case 2:
        return 'Risco';
      default:
        return 'Nao definido';
    }
  }

  classeStatusRisco(statusRisco: number): string {
    switch (statusRisco) {
      case 1:
        return 'status-atencao';
      case 2:
        return 'status-risco';
      default:
        return 'status-normal';
    }
  }

  descricaoTipoTreino(tipo: number): string {
    switch (tipo) {
      case 0:
        return 'Leve';
      case 1:
        return 'Ritmo';
      case 2:
        return 'Intervalado';
      case 3:
        return 'Longo';
      default:
        return 'Nao definido';
    }
  }

  resumoAderencia(atleta: DashboardProfessorAtletaPrioritario): string {
    if (atleta.aderenciaPlanejamentoPercentual == null) {
      return 'Planejamento base ainda nao configurado.';
    }

    return `${atleta.aderenciaPlanejamentoPercentual.toFixed(1)}% do planejado`;
  }

  descricaoTreinoRecente(treino: DashboardProfessorTreinoRecente): string {
    return `${this.descricaoTipoTreino(treino.tipo)} · carga ${treino.carga}`;
  }
}
