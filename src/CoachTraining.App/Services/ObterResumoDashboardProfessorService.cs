using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.App.DTOs;
using CoachTraining.Domain.Entities;
using CoachTraining.Domain.Enums;

namespace CoachTraining.App.Services;

public class ObterResumoDashboardProfessorService
{
    private readonly IAtletaRepository _atletaRepository;
    private readonly ISessaoDeTreinoRepository _sessaoDeTreinoRepository;
    private readonly IProvaAlvoRepository _provaAlvoRepository;
    private readonly ObterDashboardAtletaService _obterDashboardAtletaService;

    public ObterResumoDashboardProfessorService(
        IAtletaRepository atletaRepository,
        ISessaoDeTreinoRepository sessaoDeTreinoRepository,
        IProvaAlvoRepository provaAlvoRepository,
        ObterDashboardAtletaService obterDashboardAtletaService)
    {
        _atletaRepository = atletaRepository ?? throw new ArgumentNullException(nameof(atletaRepository));
        _sessaoDeTreinoRepository = sessaoDeTreinoRepository ?? throw new ArgumentNullException(nameof(sessaoDeTreinoRepository));
        _provaAlvoRepository = provaAlvoRepository ?? throw new ArgumentNullException(nameof(provaAlvoRepository));
        _obterDashboardAtletaService = obterDashboardAtletaService ?? throw new ArgumentNullException(nameof(obterDashboardAtletaService));
    }

    public DashboardProfessorResumoDto ObterResumo(Guid professorId)
    {
        if (professorId == Guid.Empty)
        {
            throw new ArgumentException("ProfessorId obrigatorio.", nameof(professorId));
        }

        var dataAtualizacao = DateTime.UtcNow;
        var atletas = _atletaRepository.ListarPorProfessor(professorId);
        var dashboards = new List<DashboardAtletaDto>(atletas.Count);
        var treinosRecentes = new List<DashboardProfessorTreinoRecenteDto>();

        foreach (var atleta in atletas)
        {
            var sessoes = _sessaoDeTreinoRepository.ObterPorAtletaId(atleta.Id, professorId);
            var provaAlvo = _provaAlvoRepository.ObterPorAtletaId(atleta.Id, professorId);
            var dashboard = _obterDashboardAtletaService.ObterDashboard(atleta, sessoes, provaAlvo);

            dashboards.Add(dashboard);
            treinosRecentes.AddRange(MontarTreinosRecentes(atleta, sessoes));
        }

        return new DashboardProfessorResumoDto
        {
            TotalAtletas = dashboards.Count,
            AtletasEmAtencao = dashboards.Count(dashboard => dashboard.StatusRisco != StatusDeRisco.Normal),
            AtletasEmRisco = dashboards.Count(dashboard => dashboard.StatusRisco == StatusDeRisco.Risco),
            AtletasEmTaper = dashboards.Count(dashboard => dashboard.EmJanelaDeTaper),
            TreinosRegistradosNaSemana = dashboards.Sum(dashboard => dashboard.TreinosRealizadosNaSemana),
            AtletasComPlanejamentoConfigurado = dashboards.Count(dashboard => dashboard.TreinosPlanejadosPorSemana.HasValue),
            DataUltimaAtualizacao = dataAtualizacao,
            AtletasPrioritarios = dashboards
                .Where(EhAtletaPrioritario)
                .OrderByDescending(CalcularPrioridade)
                .ThenBy(dashboard => dashboard.ProximaProva ?? DateOnly.MaxValue)
                .ThenBy(dashboard => dashboard.Nome)
                .Take(5)
                .Select(dashboard => new DashboardProfessorAtletaPrioritarioDto
                {
                    AtletaId = dashboard.AtletaId,
                    Nome = dashboard.Nome,
                    StatusRisco = dashboard.StatusRisco,
                    EmJanelaDeTaper = dashboard.EmJanelaDeTaper,
                    ProximaProva = dashboard.ProximaProva,
                    CargaSemanal = dashboard.CargaSemanal,
                    AderenciaPlanejamentoPercentual = dashboard.AderenciaPlanejamentoPercentual
                })
                .ToList(),
            TreinosRecentes = treinosRecentes
                .OrderByDescending(treino => treino.Data)
                .ThenBy(treino => treino.NomeAtleta)
                .Take(6)
                .ToList()
        };
    }

    private static IEnumerable<DashboardProfessorTreinoRecenteDto> MontarTreinosRecentes(
        Atleta atleta,
        IEnumerable<SessaoDeTreino> sessoes)
    {
        return sessoes.Select(sessao => new DashboardProfessorTreinoRecenteDto
        {
            AtletaId = atleta.Id,
            NomeAtleta = atleta.Nome,
            Data = sessao.Data,
            Tipo = sessao.Tipo,
            Carga = sessao.CalcularCarga().Valor
        });
    }

    private static bool EhAtletaPrioritario(DashboardAtletaDto dashboard)
    {
        return dashboard.StatusRisco != StatusDeRisco.Normal
            || dashboard.EmJanelaDeTaper
            || dashboard.AderenciaPlanejamentoPercentual is < 80
            || dashboard.AderenciaPlanejamentoPercentual is > 120;
    }

    private static int CalcularPrioridade(DashboardAtletaDto dashboard)
    {
        var prioridade = dashboard.StatusRisco switch
        {
            StatusDeRisco.Risco => 300,
            StatusDeRisco.Atencao => 200,
            _ => 0
        };

        if (dashboard.EmJanelaDeTaper)
        {
            prioridade += 100;
        }

        if (dashboard.AderenciaPlanejamentoPercentual is double aderencia)
        {
            if (aderencia < 80)
            {
                prioridade += 80;
            }
            else if (aderencia > 120)
            {
                prioridade += 40;
            }
        }

        return prioridade;
    }
}
