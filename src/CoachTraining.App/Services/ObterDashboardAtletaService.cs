using CoachTraining.App.DTOs;
using CoachTraining.Domain.Entities;
using CoachTraining.Domain.Enums;
using CoachTraining.Domain.Services;
using CoachTraining.Domain.ValueObjects;

namespace CoachTraining.App.Services;

/// <summary>
/// Serviço de aplicação responsável por consolidar métricas e montar o dashboard do atleta.
/// Orquestra os Domain Services para apresentar uma visão holística da saúde do treinamento.
/// </summary>
public class ObterDashboardAtletaService
{
    // Utiliza métodos estáticos dos Domain Services (CalculadoraDeCarga, AvaliadorDeRisco, ClassificadorDeFase)
    public ObterDashboardAtletaService() { }

    /// <summary>
    /// Constrói o DTO completo de dashboard para um atleta a partir de suas sessões e dados.
    /// </summary>
    /// <param name="atleta">Entidade do atleta</param>
    /// <param name="sessoes">Histórico de sessões de treino do atleta</param>
    /// <param name="prova">Prova alvo agendada (se houver)</param>
    /// <returns>DTO com todas as métricas consolidadas</returns>
    public DashboardAtletaDto ObterDashboard(Atleta atleta, IEnumerable<SessaoDeTreino> sessoes, ProvaAlvo? prova = null)
    {
        if (atleta == null) throw new ArgumentNullException(nameof(atleta));
        if (sessoes == null) throw new ArgumentNullException(nameof(sessoes));

        var sessoesList = sessoes.ToList();
        
        // Se não houver sessões, retornar dashboard vazio
        if (!sessoesList.Any())
        {
            return CriarDashboardVazio(atleta, prova);
        }

        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var cargas = sessoesList.Select(s => s.CalcularCarga()).ToList();

        // Cálculos de carga
        var cargaUltimaSessao = cargas.Last().Valor;
        var cargaDiaria = CalculadoraDeCarga.AgregarCargaDiaria(sessoesList);
        var cargaSemanal = ObterCargaSemanalAtual(cargaDiaria, hoje);
        var (cargaAguda, cargaCronica) = CalculadoraDeCarga.CalcularCargaAgudaECronica(sessoesList, hoje);

        // Cálculos de risco
        var acwr = AvaliadorDeRisco.CalcularAcwr(cargaAguda, cargaCronica);
        var cargaSemanaAnterior = ObterCargaSemanalAnterior(cargaDiaria, hoje);
        var deltaPercentual = AvaliadorDeRisco.CalcularDeltaPercentual(cargaSemanal, cargaSemanaAnterior);
        var statusRisco = AvaliadorDeRisco.AvaliarRiscoCombinado(acwr, deltaPercentual);

        // Classificação de fase
        var fase = ClassificadorDeFase.ClassificarFase(cargas, hoje, prova);
        var emTaper = prova != null && ClassificadorDeFase.IsInTaperWindow(hoje, prova.DataProva);
        
        double? reducaoTaper = null;
        if (emTaper && prova != null)
        {
            // Calcular redução entre última semana completa e semana anterior
            var cargaSemanaAnteriorCompleta = ObterCargaSemanalAnterior(cargaDiaria, hoje);
            reducaoTaper = ClassificadorDeFase.CalcularReducaoVolumeTaper(cargaSemanaAnteriorCompleta, cargaSemanal);
        }

        // Montar DTO
        var dto = new DashboardAtletaDto
        {
            AtletaId = atleta.Id,
            Nome = atleta.Nome,
            CargaUltimaSessao = cargaUltimaSessao,
            CargaSemanal = cargaSemanal.Valor,
            CargaAguda = cargaAguda.Valor,
            CargaCronica = cargaCronica.Valor,
            ACWR = acwr,
            DeltaPercentualSemanal = deltaPercentual,
            FaseAtual = fase,
            StatusRisco = statusRisco,
            EmJanelaDeTaper = emTaper,
            ProximaProva = prova?.DataProva,
            ReducaoVolumeTaper = reducaoTaper,
            DataUltimaAtualizacao = DateTime.UtcNow,
            ObservacoesClin = atleta.ObservacoesClinicas,
            NivelAtleta = atleta.NivelEsportivo
        };

        return PreencherInsights(dto);
    }

    /// <summary>
    /// Extrai a carga total da semana atual (segunda-domingo ISO).
    /// </summary>
    private CargaTreino ObterCargaSemanalAtual(IDictionary<DateOnly, CargaTreino> cargaDiaria, DateOnly referencia)
    {
        var (anoAtual, semanaAtual) = ObterAnoSemana(referencia);
        var cargaSemanal = cargaDiaria
            .Where(kvp => EstaEmSemanaISO(kvp.Key, anoAtual, semanaAtual))
            .Aggregate(new CargaTreino(0), (acc, kvp) => new CargaTreino(acc.Valor + kvp.Value.Valor));
        
        return cargaSemanal;
    }

    /// <summary>
    /// Extrai a carga total da semana anterior.
    /// </summary>
    private CargaTreino ObterCargaSemanalAnterior(IDictionary<DateOnly, CargaTreino> cargaDiaria, DateOnly referencia)
    {
        var (anoAtual, semanaAtual) = ObterAnoSemana(referencia);
        var (anoAnterior, semanaAnterior) = semanaAtual > 1 
            ? (anoAtual, semanaAtual - 1)
            : (anoAtual - 1, 52);

        var cargaSemanal = cargaDiaria
            .Where(kvp => EstaEmSemanaISO(kvp.Key, anoAnterior, semanaAnterior))
            .Aggregate(new CargaTreino(0), (acc, kvp) => new CargaTreino(acc.Valor + kvp.Value.Valor));
        
        return cargaSemanal;
    }

    /// <summary>
    /// Obtém o ano e semana ISO de uma data.
    /// </summary>
    private (int Ano, int Semana) ObterAnoSemana(DateOnly data)
    {
        var regras = System.Globalization.DateTimeFormatInfo.CurrentInfo;
        var cal = System.Globalization.CultureInfo.CurrentCulture.Calendar;
        var numSemana = cal.GetWeekOfYear(data.ToDateTime(TimeOnly.MinValue), 
            System.Globalization.CalendarWeekRule.FirstFourDayWeek, 
            DayOfWeek.Monday);
        
        return (data.Year, numSemana);
    }

    /// <summary>
    /// Verifica se uma data pertence a uma semana ISO específica.
    /// </summary>
    private bool EstaEmSemanaISO(DateOnly data, int ano, int semana)
    {
        var (dataAno, dataSemana) = ObterAnoSemana(data);
        return dataAno == ano && dataSemana == semana;
    }

    /// <summary>
    /// Cria um dashboard vazio para atleta sem sessões registradas.
    /// </summary>
    private DashboardAtletaDto CriarDashboardVazio(Atleta atleta, ProvaAlvo? prova)
    {
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var emTaper = prova != null && ClassificadorDeFase.IsInTaperWindow(hoje, prova.DataProva);

        return new DashboardAtletaDto
        {
            AtletaId = atleta.Id,
            Nome = atleta.Nome,
            CargaUltimaSessao = 0,
            CargaSemanal = 0,
            CargaAguda = 0,
            CargaCronica = 0,
            ACWR = 0.0,
            DeltaPercentualSemanal = 0.0,
            FaseAtual = FaseDoCiclo.Base,
            StatusRisco = StatusDeRisco.Normal,
            EmJanelaDeTaper = emTaper,
            ProximaProva = prova?.DataProva,
            ReducaoVolumeTaper = null,
            DataUltimaAtualizacao = DateTime.UtcNow,
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
