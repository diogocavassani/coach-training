export interface CreateProfessorRequest {
  nome: string;
  email: string;
  senha: string;
}

export interface ProfessorResponse {
  id: string;
  nome: string;
  email: string;
  dataCriacao: string;
}
