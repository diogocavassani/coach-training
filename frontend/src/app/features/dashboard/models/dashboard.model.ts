export interface DashboardSerieCargaSemanal {
  semanaInicio: string;
  semanaFim: string;
  valor: number;
}

export interface DashboardSeriePaceSemanal {
  semanaInicio: string;
  semanaFim: string;
  valorMinPorKm: number | null;
}

export interface DashboardTreinoJanela {
  id: string;
  data: string;
  tipo: number;
  duracaoMinutos: number;
  distanciaKm: number;
  rpe: number;
  carga: number;
  paceMinPorKm: number | null;
}

export interface DashboardAtleta {
  atletaId: string;
  nome: string;
  cargaUltimaSessao: number;
  cargaSemanal: number;
  cargaSemanalAnterior: number;
  cargaAguda: number;
  cargaCronica: number;
  acwr: number;
  deltaPercentualSemanal: number;
  faseAtual: number;
  statusRisco: number;
  emJanelaDeTaper: boolean;
  proximaProva: string | null;
  reducaoVolumeTaper: number | null;
  dataUltimaAtualizacao: string;
  observacoesClin: string | null;
  nivelAtleta: string | null;
  insights: string[];
  serieCargaSemanal: DashboardSerieCargaSemanal[];
  seriePaceSemanal: DashboardSeriePaceSemanal[];
  treinosJanela: DashboardTreinoJanela[];
}
