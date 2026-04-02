using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.App.DTOs;
using CoachTraining.App.Services;
using CoachTraining.Domain.Entities;
using CoachTraining.Domain.Enums;

namespace CoachTraining.Tests.App.Services;

public class CadastrarSessaoDeTreinoServiceTests
{
    [Fact]
    public void Cadastrar_DevePersistir_QuandoAtletaPertenceAoProfessor()
    {
        var professorId = Guid.NewGuid();
        var atleta = new Atleta("Atleta", professorId, id: Guid.NewGuid());
        var atletaRepo = new FakeAtletaRepository(atleta);
        var sessaoRepo = new FakeSessaoRepository();
        var service = new CadastrarSessaoDeTreinoService(atletaRepo, sessaoRepo);

        var dto = new CadastrarSessaoDeTreinoDto
        {
            AtletaId = atleta.Id,
            Data = DateOnly.FromDateTime(DateTime.UtcNow),
            Tipo = TipoDeTreino.Intervalado,
            DuracaoMinutos = 45,
            DistanciaKm = 8,
            Rpe = 7
        };

        var result = service.Cadastrar(dto, professorId);

        Assert.Equal(atleta.Id, result.AtletaId);
        Assert.Single(sessaoRepo.Itens);
    }

    [Fact]
    public void Cadastrar_DeveFalhar_QuandoAtletaNaoPertenceAoProfessor()
    {
        var professorA = Guid.NewGuid();
        var professorB = Guid.NewGuid();
        var atleta = new Atleta("Atleta", professorB, id: Guid.NewGuid());
        var atletaRepo = new FakeAtletaRepository(atleta);
        var sessaoRepo = new FakeSessaoRepository();
        var service = new CadastrarSessaoDeTreinoService(atletaRepo, sessaoRepo);

        var dto = new CadastrarSessaoDeTreinoDto
        {
            AtletaId = atleta.Id,
            Data = DateOnly.FromDateTime(DateTime.UtcNow),
            Tipo = TipoDeTreino.Leve,
            DuracaoMinutos = 30,
            DistanciaKm = 5,
            Rpe = 4
        };

        Assert.Throws<UnauthorizedAccessException>(() => service.Cadastrar(dto, professorA));
        Assert.Empty(sessaoRepo.Itens);
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

        public IReadOnlyList<Atleta> ListarPorProfessor(Guid professorId)
        {
            return _atleta.ProfessorId == professorId ? [_atleta] : [];
        }

        public Atleta? ObterPorId(Guid atletaId, Guid professorId)
        {
            return _atleta.Id == atletaId && _atleta.ProfessorId == professorId ? _atleta : null;
        }
    }

    private sealed class FakeSessaoRepository : ISessaoDeTreinoRepository
    {
        public List<SessaoDeTreino> Itens { get; } = [];

        public void Adicionar(SessaoDeTreino sessao)
        {
            Itens.Add(sessao);
        }

        public IReadOnlyCollection<SessaoDeTreino> ObterPorAtletaId(Guid atletaId, Guid professorId)
        {
            return Itens.Where(i => i.AtletaId == atletaId).ToList();
        }
    }
}
