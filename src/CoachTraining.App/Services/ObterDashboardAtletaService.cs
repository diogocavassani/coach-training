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
        var sessoesList = FiltrarOrdenarSessoes(sessoes, hoje);

        // Se nao houver sessoes validas, retornar dashboard vazio
        if (sessoesList.Count == 0)
        {
            return CriarDashboardVazio(atleta, prova, hoje, agora);
        }

        var cargas = sessoesList.Select(s => s.CalcularCarga()).ToList();
        var cargaDiaria = CalculadoraDeCarga.AgregarCargaDiaria(sessoesList);

        // Calculos de carga
        var cargaUltimaSessao = cargas[^1].Valor;
        var cargaSemanal = ObterCargaNoPeriodo(cargaDiaria, hoje.AddDays(-6), hoje);
        var cargaSemanaAnterior = ObterCargaNoPeriodo(cargaDiaria, hoje.AddDays(-13), hoje.AddDays(-7));
        var (cargaAguda, cargaCronica) = CalcularCargaAgudaECronicaPorJanela(cargaDiaria, hoje);

        // Calculos de risco
        var acwr = AvaliadorDeRisco.CalcularAcwr(cargaAguda, cargaCronica);
        var deltaPercentual = AvaliadorDeRisco.CalcularDeltaPercentual(cargaSemanal, cargaSemanaAnterior);
        var statusRisco = AvaliadorDeRisco.AvaliarRiscoCombinado(acwr, deltaPercentual);

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
            NivelAtleta = atleta.NivelEsportivo
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

    /// <summary>
    /// Cria um dashboard vazio para atleta sem sessoes registradas.
    /// </summary>
    private DashboardAtletaDto CriarDashboardVazio(Atleta atleta, ProvaAlvo? prova, DateOnly hoje, DateTime dataAtualizacao)
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
            NivelAtleta = atleta.NivelEsportivo
        };
    }

    private DashboardAtletaDto PreencherInsights(DashboardAtletaDto dto)
    {
        dto.Insights = GeradorDeInsights.GerarInsights(dto);
        return dto;
    }
}
