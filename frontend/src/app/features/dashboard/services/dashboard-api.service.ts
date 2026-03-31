import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { DashboardAtleta } from '../models/dashboard.model';

@Injectable({
  providedIn: 'root'
})
export class DashboardApiService {
  private readonly endpoint = '/api/dashboard/atleta';

  constructor(private readonly httpClient: HttpClient) {}

  obterPorAtletaId(atletaId: string): Observable<DashboardAtleta> {
    return this.httpClient.get<DashboardAtleta>(`${this.endpoint}/${atletaId}`);
  }
}
