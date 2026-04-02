export interface Student {
  id: string;
  nome: string;
  email?: string;
  observacoesClinicas?: string;
  nivelEsportivo?: string;
  dataCriacao?: string;
}

export interface StudentTarget {
  id: string;
  atletaId: string;
  dataProva: string;
  distanciaKm: number;
  objetivo?: string;
}

export interface SaveStudentTargetRequest {
  dataProva: string;
  distanciaKm: number;
  objetivo?: string;
}
