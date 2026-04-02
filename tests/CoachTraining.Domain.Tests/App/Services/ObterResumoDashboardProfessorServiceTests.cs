using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.App.Services;
using CoachTraining.Domain.Entities;
using CoachTraining.Domain.Enums;
using CoachTraining.Domain.ValueObjects;

namespace CoachTraining.Tests.App.Services;

public class ObterResumoDashboardProfessorServiceTests
{
    private readonly Guid _professorId = Guid.NewGuid();

    [Fact]
    public void ObterResumo_ComIndicadoresOperacionais_DeveConsolidarKPIsEListas()
    {
        var atletaRisco = new Atleta("Atleta Risco", _professorId, treinosPlanejadosPorSemana: 4);
        var atletaTaper = new Atleta("Atleta Taper", _professorId, treinosPlanejadosPorSemana: 5);
        var atletaEstavel = new Atleta("Atleta Estavel", _professorId);

        var atletaRepository = new FakeAtletaRepository([atletaRisco, atletaTaper, atletaEstavel]);
        var sessaoRepository = new FakeSessaoDeTreinoRepository();
        var provaRepository = new FakeProvaAlvoRepository();
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);

        sessaoRepository.Seed(atletaRisco.Id, _professorId, [
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-30), TipoDeTreino.Leve, 45, 6.0, new RPE(3)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-23), TipoDeTreino.Leve, 45, 6.0, new RPE(3)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-16), TipoDeTreino.Leve, 45, 6.0, new RPE(3)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-9), TipoDeTreino.Leve, 45, 6.0, new RPE(3)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-2), TipoDeTreino.Intervalado, 90, 10.0, new RPE(9)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-1), TipoDeTreino.Longo, 120, 16.0, new RPE(8))
        ]);

        sessaoRepository.Seed(atletaTaper.Id, _professorId, [
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-3), TipoDeTreino.Ritmo, 50, 8.0, new RPE(6)),
            new SessaoDeTreino(Guid.NewGuid(), hoje.AddDays(-1), TipoDeTreino.Leve, 40, 6.0, new RPE(4))
        ]);

        sessaoRepository.Seed(atletaEstavel.Id, _professorId, [
            new SessaoDeTreino(Guid.NewGuid(), hoje, TipoDeTreino.Leve, 35, 5.0, new RPE(4))
        ]);

        provaRepository.Seed(atletaTaper.Id, _professorId, new ProvaAlvo(hoje.AddDays(10), 21.1, "Meia maratona"));

        var service = new ObterResumoDashboardProfessorService(
            atletaRepository,
            sessaoRepository,
            provaRepository,
            new ObterDashboardAtletaService());

        var resumo = service.ObterResumo(_professorId);

        Assert.Equal(3, resumo.TotalAtletas);
        Assert.Equal(1, resumo.AtletasEmAtencao);
        Assert.Equal(1, resumo.AtletasEmRisco);
        Assert.Equal(1, resumo.AtletasEmTaper);
        Assert.Equal(5, resumo.TreinosRegistradosNaSemana);
        Assert.Equal(2, resumo.AtletasComPlanejamentoConfigurado);
        Assert.Equal("Atleta Risco", resumo.AtletasPrioritarios[0].Nome);
        Assert.Contains(resumo.AtletasPrioritarios, atleta => atleta.Nome == "Atleta Taper" && atleta.EmJanelaDeTaper);
        Assert.Equal("Atleta Estavel", resumo.TreinosRecentes[0].NomeAtleta);
    }

    [Fact]
    public void ObterResumo_SemAtletas_DeveRetornarResumoVazio()
    {
        var service = new ObterResumoDashboardProfessorService(
            new FakeAtletaRepository([]),
            new FakeSessaoDeTreinoRepository(),
            new FakeProvaAlvoRepository(),
            new ObterDashboardAtletaService());

        var resumo = service.ObterResumo(_professorId);

        Assert.Equal(0, resumo.TotalAtletas);
        Assert.Equal(0, resumo.AtletasEmAtencao);
        Assert.Equal(0, resumo.AtletasEmRisco);
        Assert.Equal(0, resumo.AtletasEmTaper);
        Assert.Equal(0, resumo.TreinosRegistradosNaSemana);
        Assert.Empty(resumo.AtletasPrioritarios);
        Assert.Empty(resumo.TreinosRecentes);
    }

    private sealed class FakeAtletaRepository(IReadOnlyList<Atleta> atletas) : IAtletaRepository
    {
        public void Adicionar(Atleta atleta) => throw new NotSupportedException();

        public void AtualizarPlanejamentoBase(Guid atletaId, Guid professorId, int treinosPlanejadosPorSemana)
            => throw new NotSupportedException();

        public Atleta? ObterPorId(Guid atletaId, Guid professorId)
            => atletas.SingleOrDefault(atleta => atleta.Id == atletaId && atleta.ProfessorId == professorId);

        public IReadOnlyList<Atleta> ListarPorProfessor(Guid professorId)
            => atletas.Where(atleta => atleta.ProfessorId == professorId).ToList();
    }

    private sealed class FakeSessaoDeTreinoRepository : ISessaoDeTreinoRepository
    {
        private readonly Dictionary<(Guid atletaId, Guid professorId), IReadOnlyCollection<SessaoDeTreino>> _dados = [];

        public void Adicionar(SessaoDeTreino sessao) => throw new NotSupportedException();

        public IReadOnlyCollection<SessaoDeTreino> ObterPorAtletaId(Guid atletaId, Guid professorId)
            => _dados.TryGetValue((atletaId, professorId), out var sessoes) ? sessoes : [];

        public void Seed(Guid atletaId, Guid professorId, IReadOnlyCollection<SessaoDeTreino> sessoes)
        {
            _dados[(atletaId, professorId)] = sessoes;
        }
    }

    private sealed class FakeProvaAlvoRepository : IProvaAlvoRepository
    {
        private readonly Dictionary<(Guid atletaId, Guid professorId), ProvaAlvo> _dados = [];

        public ProvaAlvo? ObterPorAtletaId(Guid atletaId, Guid professorId)
            => _dados.TryGetValue((atletaId, professorId), out var prova) ? prova : null;

        public void Salvar(Guid atletaId, ProvaAlvo provaAlvo) => throw new NotSupportedException();

        public void Seed(Guid atletaId, Guid professorId, ProvaAlvo provaAlvo)
        {
            _dados[(atletaId, professorId)] = provaAlvo;
        }
    }
}
