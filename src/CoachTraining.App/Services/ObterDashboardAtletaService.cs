using CoachTraining.App.DTOs;
using CoachTraining.Domain.Entities;
using CoachTraining.Domain.Enums;
using CoachTraining.Domain.Services;
using CoachTraining.Domain.ValueObjects;

namespace CoachTraining.App.Services;

/// <summary>
/// Servico de aplicacao responsavel por consolidar metricas e montar o dashboard do atleta.
/// Orquestra os Domain Services para apresentar uma visao holistica da saude do treinamento.
/// </summary>
public class ObterDashboardAtletaService
{
    private const int NumeroSemanasJanelaDashboard = 12;

    private readonly record struct IntervaloSemanal(DateOnly Inicio, DateOnly Fim);

    // Utiliza metodos estaticos dos Domain Services (CalculadoraDeCarga, AvaliadorDeRisco, ClassificadorDeFase)
    public ObterDashboardAtletaService() { }

    /// <summary>
    /// Constroi o DTO completo de dashboard para um atleta a partir de suas sessoes e dados.
    /// </summary>
    /// <param name="atleta">Entidade do atleta</param>
    /// <param name="sessoes">Historico de sessoes de treino do atleta</param>
    /// <param name="prova">Prova alvo agendada (se houver)</param>
    /// <returns>DTO com todas as metricas consolidadas</returns>
    public DashboardAtletaDto ObterDashboard(Atleta atleta, IEnumerable<SessaoDeTreino> sessoes, ProvaAlvo? prova = null)
    {
        if (atleta == null) throw new ArgumentNullException(nameof(atleta));
        if (sessoes == null) throw new ArgumentNullException(nameof(sessoes));

        var agora = DateTime.UtcNow;
        var hoje = DateOnly.FromDateTime(agora);
        var sessoesListEntrada = sessoes.ToList();
        ValidarIntegridadeDasSessoes(sessoesListEntrada);
        var sessoesList = FiltrarOrdenarSessoes(sessoesListEntrada, hoje);
        var janelaSemanal = ObterJanelaSemanal(hoje, NumeroSemanasJanelaDashboard);
        var sessoesJanela = ObterSessoesDaJanela(sessoesList, janelaSemanal);

        // Se nao houver sessoes validas, retornar dashboard vazio
        if (sessoesList.Count == 0)
        {
            return CriarDashboardVazio(atleta, prova, hoje, agora, janelaSemanal);
        }

        var cargas = sessoesList.Select(s => s.CalcularCarga()).ToList();
        var cargaDiaria = CalculadoraDeCarga.AgregarCargaDiaria(sessoesList);

        // Calculos de carga
        var cargaUltimaSessao = cargas[^1].Valor;
        var cargaSemanal = ObterCargaNoPeriodo(cargaDiaria, hoje.AddDays(-6), hoje);
        var cargaSemanaAnterior = ObterCargaNoPeriodo(cargaDiaria, hoje.AddDays(-13), hoje.AddDays(-7));
        var (cargaAguda, cargaCronica) = CalcularCargaAgudaECronicaPorJanela(cargaDiaria, hoje);

        // Calculos de risco
        var acwrCalculado = AvaliadorDeRisco.CalcularAcwr(cargaAguda, cargaCronica);
        var deltaPercentual = AvaliadorDeRisco.CalcularDeltaPercentual(cargaSemanal, cargaSemanaAnterior);
        var statusRisco = AvaliadorDeRisco.AvaliarRiscoCombinado(acwrCalculado, deltaPercentual);
        var acwr = NormalizarNumeroParaJson(acwrCalculado);

        // Classificacao de fase
        var fase = ClassificadorDeFase.ClassificarFase(cargas, hoje, prova);
        var emTaper = prova != null && ClassificadorDeFase.IsInTaperWindow(hoje, prova.DataProva);

        double? reducaoTaper = null;
        if (emTaper)
        {
            // Somar explicitamente janelas de 7 dias: [ref .. ref+6] ou [ref-6 .. ref-1] conforme intencao.
            // Aqui escolhemos comparar a semana que comeca em hoje.AddDays(-21) com a semana que comeca em hoje.AddDays(-7)
            var cargaAntesDeTaper = ObterCargaNoPeriodo(cargaDiaria, hoje.AddDays(-21), hoje.AddDays(-15)); // 7 dias: -21..-15
            var cargaDuranteTaper = ObterCargaNoPeriodo(cargaDiaria, hoje.AddDays(-7), hoje.AddDays(-1));   // 7 dias: -7..-1
            reducaoTaper = ClassificadorDeFase.CalcularReducaoVolumeTaper(cargaAntesDeTaper, cargaDuranteTaper);
        }

        // Montar DTO
        var dto = new DashboardAtletaDto
        {
            AtletaId = atleta.Id,
            Nome = atleta.Nome,
            CargaUltimaSessao = cargaUltimaSessao,
            CargaSemanal = cargaSemanal.Valor,
            CargaSemanalAnterior = cargaSemanaAnterior.Valor,
            CargaAguda = cargaAguda.Valor,
            CargaCronica = cargaCronica.Valor,
            ACWR = acwr,
            DeltaPercentualSemanal = deltaPercentual,
            FaseAtual = fase,
            StatusRisco = statusRisco,
            EmJanelaDeTaper = emTaper,
            ProximaProva = prova?.DataProva,
            ReducaoVolumeTaper = reducaoTaper,
            DataUltimaAtualizacao = agora,
            ObservacoesClin = atleta.ObservacoesClinicas,
            NivelAtleta = atleta.NivelEsportivo,
            SerieCargaSemanal = MontarSerieCargaSemanal(janelaSemanal, sessoesJanela),
            SeriePaceSemanal = MontarSeriePaceSemanal(janelaSemanal, sessoesJanela),
            TreinosJanela = MontarTreinosDaJanela(sessoesJanela)
        };

        return PreencherInsights(dto);
    }

    private static List<SessaoDeTreino> FiltrarOrdenarSessoes(IEnumerable<SessaoDeTreino> sessoes, DateOnly referencia)
    {
        return sessoes
            .Where(s => s != null && s.Data <= referencia)
            .OrderBy(s => s.Data)
            .ToList();
    }

    private static void ValidarIntegridadeDasSessoes(IEnumerable<SessaoDeTreino> sessoes)
    {
        var sessoesInvalidas = sessoes
            .Where(s => s == null || s.DuracaoMinutos <= 0)
            .ToList();

        if (sessoesInvalidas.Count == 0)
        {
            return;
        }

        var exemplos = string.Join(
            ", ",
            sessoesInvalidas
                .Take(3)
                .Select(s => s == null
                    ? "{Id: null, DuracaoMinutos: null}"
                    : $"{{Id: {s.Id}, DuracaoMinutos: {s.DuracaoMinutos}}}"));

        throw new InvalidOperationException(
            $"Erro de integridade: sessoes com DuracaoMinutos <= 0 foram encontradas ({sessoesInvalidas.Count}). Exemplos: {exemplos}.");
    }

    private CargaTreino ObterCargaNoPeriodo(IDictionary<DateOnly, CargaTreino> cargaDiaria, DateOnly inicio, DateOnly fim)
    {
        // Soma cargas de dias entre inicio e fim (inclusive)
        var total = cargaDiaria
            .Where(kvp => kvp.Key >= inicio && kvp.Key <= fim)
            .Sum(kvp => kvp.Value.Valor);

        return new CargaTreino(total);
    }

    /// <summary>
    /// Calcula carga aguda (ultima janela de 7 dias) e cronica (media das ultimas 4 janelas de 7 dias).
    /// </summary>
    private (CargaTreino Aguda, CargaTreino Cronica) CalcularCargaAgudaECronicaPorJanela(
        IDictionary<DateOnly, CargaTreino> cargaDiaria,
        DateOnly referencia)
    {
        var semanas = new List<CargaTreino>(capacity: 4);
        for (int i = 0; i < 4; i++)
        {
            var fim = referencia.AddDays(-7 * i);
            var inicio = fim.AddDays(-6);
            semanas.Add(ObterCargaNoPeriodo(cargaDiaria, inicio, fim));
        }

        var aguda = semanas[0];
        var cronicaValor = semanas.Any(s => s.Valor == 0)
            ? 0
            : (int)Math.Round(semanas.Average(s => s.Valor));
        var cronica = new CargaTreino(cronicaValor);

        return (aguda, cronica);
    }

    private static IReadOnlyList<IntervaloSemanal> ObterJanelaSemanal(DateOnly referencia, int quantidadeSemanas)
    {
        var inicioSemanaAtual = ObterInicioDaSemana(referencia);
        var intervalos = new List<IntervaloSemanal>(quantidadeSemanas);

        for (var i = quantidadeSemanas - 1; i >= 0; i--)
        {
            var inicio = inicioSemanaAtual.AddDays(-7 * i);
            intervalos.Add(new IntervaloSemanal(inicio, inicio.AddDays(6)));
        }

        return intervalos;
    }

    private static DateOnly ObterInicioDaSemana(DateOnly data)
    {
        var deslocamento = ((int)data.DayOfWeek + 6) % 7; // Semana iniciando na segunda-feira.
        return data.AddDays(-deslocamento);
    }

    private static IReadOnlyList<SessaoDeTreino> ObterSessoesDaJanela(
        IReadOnlyList<SessaoDeTreino> sessoes,
        IReadOnlyList<IntervaloSemanal> janelaSemanal)
    {
        if (janelaSemanal.Count == 0)
        {
            return [];
        }

        var inicioJanela = janelaSemanal[0].Inicio;
        var fimJanela = janelaSemanal[^1].Fim;

        return sessoes
            .Where(s => s.Data >= inicioJanela && s.Data <= fimJanela)
            .OrderBy(s => s.Data)
            .ToList();
    }

    private static IList<SerieCargaSemanalDto> MontarSerieCargaSemanal(
        IReadOnlyList<IntervaloSemanal> janelaSemanal,
        IReadOnlyList<SessaoDeTreino> sessoesJanela)
    {
        var cargasPorDia = CalculadoraDeCarga.AgregarCargaDiaria(sessoesJanela);

        return janelaSemanal
            .Select(intervalo => new SerieCargaSemanalDto
            {
                SemanaInicio = intervalo.Inicio,
                SemanaFim = intervalo.Fim,
                Valor = cargasPorDia
                    .Where(kvp => kvp.Key >= intervalo.Inicio && kvp.Key <= intervalo.Fim)
                    .Sum(kvp => kvp.Value.Valor)
            })
            .ToList();
    }

    private static IList<SeriePaceSemanalDto> MontarSeriePaceSemanal(
        IReadOnlyList<IntervaloSemanal> janelaSemanal,
        IReadOnlyList<SessaoDeTreino> sessoesJanela)
    {
        return janelaSemanal
            .Select(intervalo =>
            {
                var sessoesComDistancia = sessoesJanela
                    .Where(s => s.Data >= intervalo.Inicio && s.Data <= intervalo.Fim && s.DistanciaKm > 0)
                    .ToList();

                double? pace = null;
                if (sessoesComDistancia.Count > 0)
                {
                    var distanciaTotal = sessoesComDistancia.Sum(s => s.DistanciaKm);
                    var duracaoTotal = sessoesComDistancia.Sum(s => s.DuracaoMinutos);
                    if (distanciaTotal > 0)
                    {
                        pace = Math.Round(duracaoTotal / distanciaTotal, 2);
                    }
                }

                return new SeriePaceSemanalDto
                {
                    SemanaInicio = intervalo.Inicio,
                    SemanaFim = intervalo.Fim,
                    ValorMinPorKm = pace
                };
            })
            .ToList();
    }

    private static IList<TreinoJanelaDto> MontarTreinosDaJanela(IReadOnlyList<SessaoDeTreino> sessoesJanela)
    {
        return sessoesJanela
            .OrderBy(s => s.Data)
            .Select(sessao =>
            {
                double? pace = null;
                if (sessao.DistanciaKm > 0)
                {
                    pace = Math.Round(sessao.DuracaoMinutos / sessao.DistanciaKm, 2);
                }

                return new TreinoJanelaDto
                {
                    Id = sessao.Id,
                    Data = sessao.Data,
                    Tipo = sessao.Tipo,
                    DuracaoMinutos = sessao.DuracaoMinutos,
                    DistanciaKm = sessao.DistanciaKm,
                    Rpe = sessao.Rpe.Valor,
                    Carga = sessao.CalcularCarga().Valor,
                    PaceMinPorKm = pace
                };
            })
            .ToList();
    }

    /// <summary>
    /// Cria um dashboard vazio para atleta sem sessoes registradas.
    /// </summary>
    private DashboardAtletaDto CriarDashboardVazio(
        Atleta atleta,
        ProvaAlvo? prova,
        DateOnly hoje,
        DateTime dataAtualizacao,
        IReadOnlyList<IntervaloSemanal> janelaSemanal)
    {
        var emTaper = prova != null && ClassificadorDeFase.IsInTaperWindow(hoje, prova.DataProva);

        return new DashboardAtletaDto
        {
            AtletaId = atleta.Id,
            Nome = atleta.Nome,
            CargaUltimaSessao = 0,
            CargaSemanal = 0,
            CargaSemanalAnterior = 0,
            CargaAguda = 0,
            CargaCronica = 0,
            ACWR = 0.0,
            DeltaPercentualSemanal = 0.0,
            FaseAtual = FaseDoCiclo.Base,
            StatusRisco = StatusDeRisco.Normal,
            EmJanelaDeTaper = emTaper,
            ProximaProva = prova?.DataProva,
            ReducaoVolumeTaper = null,
            DataUltimaAtualizacao = dataAtualizacao,
            ObservacoesClin = atleta.ObservacoesClinicas,
            NivelAtleta = atleta.NivelEsportivo,
            SerieCargaSemanal = MontarSerieCargaSemanal(janelaSemanal, []),
            SeriePaceSemanal = MontarSeriePaceSemanal(janelaSemanal, []),
            TreinosJanela = []
        };
    }

    private DashboardAtletaDto PreencherInsights(DashboardAtletaDto dto)
    {
        dto.Insights = GeradorDeInsights.GerarInsights(dto);
        return dto;
    }

    private static double NormalizarNumeroParaJson(double valor)
    {
        if (double.IsNaN(valor))
        {
            return 0;
        }

        if (double.IsPositiveInfinity(valor))
        {
            return 999.99;
        }

        if (double.IsNegativeInfinity(valor))
        {
            return -999.99;
        }

        return valor;
    }
}
