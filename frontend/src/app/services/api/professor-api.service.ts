import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { CreateProfessorRequest, ProfessorResponse } from './professor-api.models';

@Injectable({
  providedIn: 'root'
})
export class ProfessorApiService {
  private readonly endpoint = '/professores';

  constructor(private readonly httpClient: HttpClient) {}

  cadastrar(payload: CreateProfessorRequest): Observable<ProfessorResponse> {
    return this.httpClient.post<ProfessorResponse>(this.endpoint, payload);
  }
}
