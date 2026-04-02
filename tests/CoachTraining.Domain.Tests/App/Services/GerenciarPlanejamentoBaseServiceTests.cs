using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.App.DTOs;
using CoachTraining.App.Services;
using CoachTraining.Domain.Entities;

namespace CoachTraining.Tests.App.Services;

public class GerenciarPlanejamentoBaseServiceTests
{
    [Fact]
    public void Salvar_DevePersistirPlanejamentoBase_QuandoAtletaPertenceAoProfessor()
    {
        var professorId = Guid.NewGuid();
        var atleta = new Atleta("Atleta Planejamento", professorId, id: Guid.NewGuid());
        var atletaRepository = new FakeAtletaRepository(atleta);
        var service = new GerenciarPlanejamentoBaseService(atletaRepository);

        var dto = new SalvarPlanejamentoBaseDto
        {
            TreinosPlanejadosPorSemana = 5
        };

        var planejamento = service.Salvar(atleta.Id, dto, professorId);

        Assert.Equal(atleta.Id, planejamento.AtletaId);
        Assert.Equal(5, planejamento.TreinosPlanejadosPorSemana);
        Assert.Equal(5, atletaRepository.AtualizacaoRealizadaPara);
    }

    [Fact]
    public void Salvar_DeveFalhar_QuandoAtletaNaoPertenceAoProfessor()
    {
        var professorA = Guid.NewGuid();
        var professorB = Guid.NewGuid();
        var atleta = new Atleta("Atleta", professorB, id: Guid.NewGuid());
        var atletaRepository = new FakeAtletaRepository(atleta);
        var service = new GerenciarPlanejamentoBaseService(atletaRepository);

        var dto = new SalvarPlanejamentoBaseDto
        {
            TreinosPlanejadosPorSemana = 4
        };

        Assert.Throws<UnauthorizedAccessException>(() => service.Salvar(atleta.Id, dto, professorA));
    }

    [Fact]
    public void ObterPorAtletaId_DeveRetornarPlanejamentoQuandoConfigurado()
    {
        var professorId = Guid.NewGuid();
        var atleta = new Atleta("Atleta", professorId, treinosPlanejadosPorSemana: 6, id: Guid.NewGuid());
        var atletaRepository = new FakeAtletaRepository(atleta);
        var service = new GerenciarPlanejamentoBaseService(atletaRepository);

        var planejamento = service.ObterPorAtletaId(atleta.Id, professorId);

        Assert.NotNull(planejamento);
        Assert.Equal(6, planejamento!.TreinosPlanejadosPorSemana);
    }

    private sealed class FakeAtletaRepository : IAtletaRepository
    {
        private Atleta _atleta;

        public FakeAtletaRepository(Atleta atleta)
        {
            _atleta = atleta;
        }

        public int? AtualizacaoRealizadaPara { get; private set; }

        public void Adicionar(Atleta atleta)
        {
            _atleta = atleta;
        }

        public void AtualizarPlanejamentoBase(Guid atletaId, Guid professorId, int treinosPlanejadosPorSemana)
        {
            if (_atleta.Id != atletaId || _atleta.ProfessorId != professorId)
            {
                throw new InvalidOperationException("Atleta nao encontrado.");
            }

            AtualizacaoRealizadaPara = treinosPlanejadosPorSemana;
            _atleta = new Atleta(
                nome: _atleta.Nome,
                professorId: _atleta.ProfessorId,
                observacoesClinicas: _atleta.ObservacoesClinicas,
                nivelEsportivo: _atleta.NivelEsportivo,
                email: _atleta.Email,
                treinosPlanejadosPorSemana: treinosPlanejadosPorSemana,
                id: _atleta.Id);
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
}
