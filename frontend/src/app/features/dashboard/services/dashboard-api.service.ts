import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { DashboardAtleta, DashboardProfessorResumo } from '../models/dashboard.model';

@Injectable({
  providedIn: 'root'
})
export class DashboardApiService {
  private readonly dashboardAtletaEndpoint = '/api/dashboard/atleta';
  private readonly dashboardProfessorEndpoint = '/api/dashboard/professor/resumo';

  constructor(private readonly httpClient: HttpClient) {}

  obterPorAtletaId(atletaId: string): Observable<DashboardAtleta> {
    return this.httpClient.get<DashboardAtleta>(`${this.dashboardAtletaEndpoint}/${atletaId}`);
  }

  obterResumoProfessor(): Observable<DashboardProfessorResumo> {
    return this.httpClient.get<DashboardProfessorResumo>(this.dashboardProfessorEndpoint);
  }
}
