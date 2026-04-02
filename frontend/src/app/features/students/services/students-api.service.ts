import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { SaveStudentTargetRequest, Student, StudentTarget } from '../models/student.model';

export interface CreateStudentRequest {
  nome: string;
  email?: string;
  observacoesClinicas?: string;
  nivelEsportivo?: string;
}

@Injectable({
  providedIn: 'root'
})
export class StudentsApiService {
  private readonly endpoint = '/api/Atleta';

  constructor(private readonly httpClient: HttpClient) {}

  cadastrar(payload: CreateStudentRequest): Observable<Student> {
    return this.httpClient.post<Student>(this.endpoint, payload);
  }

  listar(): Observable<Student[]> {
    return this.httpClient.get<Student[]>(this.endpoint);
  }

  obterProvaAlvo(atletaId: string): Observable<StudentTarget> {
    return this.httpClient.get<StudentTarget>(`${this.endpoint}/${atletaId}/prova-alvo`);
  }

  salvarProvaAlvo(atletaId: string, payload: SaveStudentTargetRequest): Observable<StudentTarget> {
    return this.httpClient.put<StudentTarget>(`${this.endpoint}/${atletaId}/prova-alvo`, payload);
  }
}
