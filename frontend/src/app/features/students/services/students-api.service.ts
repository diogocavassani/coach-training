import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Student } from '../models/student.model';

export interface CreateStudentRequest {
  nome: string;
  email?: string;
}

@Injectable({
  providedIn: 'root'
})
export class StudentsApiService {
  private readonly endpoint = '/api/Atleta';

  constructor(private readonly httpClient: HttpClient) {}

  cadastrar(payload: CreateStudentRequest): Observable<Student> {
    return this.httpClient.post<Student>(this.endpoint, {
      nome: payload.nome,
      observacoesClinicas: payload.email ?? null
    });
  }

  listar(): Observable<Student[]> {
    return this.httpClient.get<Student[]>(this.endpoint);
  }
}
