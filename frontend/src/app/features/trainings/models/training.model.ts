export interface Training {
  id: string;
  atletaId: string;
  data: string;
  tipo: number;
  duracaoMinutos: number;
  distanciaKm: number;
  rpe: number;
}

export interface CreateTrainingRequest {
  atletaId: string;
  data: string;
  tipo: number;
  duracaoMinutos: number;
  distanciaKm: number;
  rpe: number;
}
