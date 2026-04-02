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
  treinosPlanejadosPorSemana: number | null;
  treinosRealizadosNaSemana: number;
  aderenciaPlanejamentoPercentual: number | null;
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

export interface DashboardProfessorAtletaPrioritario {
  atletaId: string;
  nome: string;
  statusRisco: number;
  emJanelaDeTaper: boolean;
  proximaProva: string | null;
  cargaSemanal: number;
  aderenciaPlanejamentoPercentual: number | null;
}

export interface DashboardProfessorTreinoRecente {
  atletaId: string;
  nomeAtleta: string;
  data: string;
  tipo: number;
  carga: number;
}

export interface DashboardProfessorResumo {
  totalAtletas: number;
  atletasEmAtencao: number;
  atletasEmRisco: number;
  atletasEmTaper: number;
  treinosRegistradosNaSemana: number;
  atletasComPlanejamentoConfigurado: number;
  dataUltimaAtualizacao: string;
  atletasPrioritarios: DashboardProfessorAtletaPrioritario[];
  treinosRecentes: DashboardProfessorTreinoRecente[];
}
