import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { CreateTrainingRequest, Training } from '../models/training.model';

@Injectable({
  providedIn: 'root'
})
export class TrainingsApiService {
  private readonly endpoint = '/api/treinos';

  constructor(private readonly httpClient: HttpClient) {}

  cadastrar(payload: CreateTrainingRequest): Observable<Training> {
    return this.httpClient.post<Training>(this.endpoint, payload);
  }
}
