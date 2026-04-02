using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.App.DTOs;
using CoachTraining.App.Services;
using CoachTraining.Domain.Entities;

namespace CoachTraining.Tests.App.Services;

public class GerenciarProvaAlvoServiceTests
{
    [Fact]
    public void Salvar_DevePersistirProvaAlvo_QuandoAtletaPertenceAoProfessor()
    {
        var professorId = Guid.NewGuid();
        var atleta = new Atleta("Atleta Prova", professorId, id: Guid.NewGuid());
        var atletaRepository = new FakeAtletaRepository(atleta);
        var provaRepository = new FakeProvaAlvoRepository();
        var service = new GerenciarProvaAlvoService(atletaRepository, provaRepository);

        var dto = new SalvarProvaAlvoDto
        {
            DataProva = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(21),
            DistanciaKm = 21.1,
            Objetivo = "Meia maratona alvo"
        };

        var provaSalva = service.Salvar(atleta.Id, dto, professorId);

        Assert.Equal(atleta.Id, provaSalva.AtletaId);
        Assert.Equal(dto.DataProva, provaSalva.DataProva);
        Assert.Equal(dto.DistanciaKm, provaSalva.DistanciaKm);
        Assert.Equal(dto.Objetivo, provaSalva.Objetivo);
        Assert.Single(provaRepository.Itens);
    }

    [Fact]
    public void Salvar_DeveFalhar_QuandoAtletaNaoPertenceAoProfessor()
    {
        var professorA = Guid.NewGuid();
        var professorB = Guid.NewGuid();
        var atleta = new Atleta("Atleta", professorB, id: Guid.NewGuid());
        var atletaRepository = new FakeAtletaRepository(atleta);
        var provaRepository = new FakeProvaAlvoRepository();
        var service = new GerenciarProvaAlvoService(atletaRepository, provaRepository);

        var dto = new SalvarProvaAlvoDto
        {
            DataProva = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(30),
            DistanciaKm = 42.2
        };

        Assert.Throws<UnauthorizedAccessException>(() => service.Salvar(atleta.Id, dto, professorA));
        Assert.Empty(provaRepository.Itens);
    }

    [Fact]
    public void ObterPorAtletaId_DeveRetornarNull_QuandoNaoHouverProvaCadastrada()
    {
        var professorId = Guid.NewGuid();
        var atleta = new Atleta("Atleta Sem Prova", professorId, id: Guid.NewGuid());
        var atletaRepository = new FakeAtletaRepository(atleta);
        var provaRepository = new FakeProvaAlvoRepository();
        var service = new GerenciarProvaAlvoService(atletaRepository, provaRepository);

        var prova = service.ObterPorAtletaId(atleta.Id, professorId);

        Assert.Null(prova);
    }

    private sealed class FakeAtletaRepository : IAtletaRepository
    {
        private readonly Atleta _atleta;

        public FakeAtletaRepository(Atleta atleta)
        {
            _atleta = atleta;
        }

        public void Adicionar(Atleta atleta)
        {
        }

        public void AtualizarPlanejamentoBase(Guid atletaId, Guid professorId, int treinosPlanejadosPorSemana)
        {
        }

        public Atleta? ObterPorId(Guid atletaId, Guid professorId)
        {
            return _atleta.Id == atletaId && _atleta.ProfessorId == professorId ? _atleta : null;
        }

        public IReadOnlyList<Atleta> ListarPorProfessor(Guid professorId)
        {
            return _atleta.ProfessorId == professorId ? [_atleta] : [];
        }
    }

    private sealed class FakeProvaAlvoRepository : IProvaAlvoRepository
    {
        public Dictionary<Guid, ProvaAlvo> Itens { get; } = [];

        public ProvaAlvo? ObterPorAtletaId(Guid atletaId, Guid professorId)
        {
            return Itens.TryGetValue(atletaId, out var prova) ? prova : null;
        }

        public void Salvar(Guid atletaId, ProvaAlvo provaAlvo)
        {
            Itens[atletaId] = provaAlvo;
        }
    }
}
