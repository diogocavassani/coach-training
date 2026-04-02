using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.App.DTOs;
using CoachTraining.App.Services;
using CoachTraining.Domain.Entities;

namespace CoachTraining.Tests.App.Services;

public class CadastroAtletaServiceTests
{
    private sealed class AtletaRepositoryFake : IAtletaRepository
    {
        private readonly Dictionary<Guid, Atleta> _atletas = new();

        public void Adicionar(Atleta atleta) => _atletas[atleta.Id] = atleta;

        public void AtualizarPlanejamentoBase(Guid atletaId, Guid professorId, int treinosPlanejadosPorSemana)
        {
            if (!_atletas.TryGetValue(atletaId, out var atleta) || atleta.ProfessorId != professorId)
            {
                throw new InvalidOperationException("Atleta nao encontrado.");
            }

            atleta.DefinirTreinosPlanejadosPorSemana(treinosPlanejadosPorSemana);
        }

        public Atleta? ObterPorId(Guid atletaId, Guid professorId)
            => _atletas.TryGetValue(atletaId, out var atleta) && atleta.ProfessorId == professorId ? atleta : null;

        public IReadOnlyList<Atleta> ListarPorProfessor(Guid professorId)
            => _atletas.Values
                .Where(atleta => atleta.ProfessorId == professorId)
                .OrderBy(atleta => atleta.Nome)
                .ToList();
    }

    [Fact]
    public void Cadastrar_DevePermitirRecuperacaoDaEntidadePorId()
    {
        var service = new CadastroAtletaService(new AtletaRepositoryFake());
        var dto = new CriarAtletaDto
        {
            Nome = "Diogo Teste",
            ObservacoesClinicas = "Sem restricoes",
            NivelEsportivo = "Intermediario"
        };
        var professorId = Guid.NewGuid();

        var atletaCadastrado = service.Cadastrar(dto, professorId);
        var entidadeRecuperada = service.ObterEntidadePorId(atletaCadastrado.Id, professorId);

        Assert.NotNull(entidadeRecuperada);
        Assert.Equal(atletaCadastrado.Id, entidadeRecuperada!.Id);
        Assert.Equal(dto.Nome, entidadeRecuperada.Nome);
        Assert.Equal(professorId, entidadeRecuperada.ProfessorId);
    }

    [Fact]
    public void ObterEntidadePorId_QuandoNaoExiste_DeveRetornarNull()
    {
        var service = new CadastroAtletaService(new AtletaRepositoryFake());

        var entidade = service.ObterEntidadePorId(Guid.NewGuid(), Guid.NewGuid());

        Assert.Null(entidade);
    }

    [Fact]
    public void ListarPorProfessor_DeveRetornarSomenteAtletasDoProfessorOrdenadosPorNome()
    {
        var service = new CadastroAtletaService(new AtletaRepositoryFake());
        var professorA = Guid.NewGuid();
        var professorB = Guid.NewGuid();

        service.Cadastrar(new CriarAtletaDto { Nome = "Zoe" }, professorA);
        service.Cadastrar(new CriarAtletaDto { Nome = "Ana" }, professorA);
        service.Cadastrar(new CriarAtletaDto { Nome = "Bruno" }, professorB);

        var atletasDoProfessorA = service.ListarPorProfessor(professorA);

        Assert.Equal(2, atletasDoProfessorA.Count);
        Assert.Collection(
            atletasDoProfessorA,
            atleta => Assert.Equal("Ana", atleta.Nome),
            atleta => Assert.Equal("Zoe", atleta.Nome));
        Assert.All(atletasDoProfessorA, atleta => Assert.Equal(professorA, atleta.ProfessorId));
    }

    [Fact]
    public void ListarPorProfessor_ProfessorIdVazio_DeveRetornarListaVazia()
    {
        var service = new CadastroAtletaService(new AtletaRepositoryFake());

        var atletas = service.ListarPorProfessor(Guid.Empty);

        Assert.Empty(atletas);
    }

    [Fact]
    public void Cadastrar_ComEmailValido_DevePersistirEmailNoAtleta()
    {
        var service = new CadastroAtletaService(new AtletaRepositoryFake());
        var professorId = Guid.NewGuid();
        var dto = new CriarAtletaDto
        {
            Nome = "Atleta Email",
            Email = "atleta.email@teste.com"
        };

        var atleta = service.Cadastrar(dto, professorId);

        Assert.Equal("atleta.email@teste.com", atleta.Email);
    }

    [Fact]
    public void Cadastrar_ComEmailInvalido_DeveLancarArgumentException()
    {
        var service = new CadastroAtletaService(new AtletaRepositoryFake());
        var professorId = Guid.NewGuid();
        var dto = new CriarAtletaDto
        {
            Nome = "Atleta Email Invalido",
            Email = "email-invalido"
        };

        Assert.Throws<ArgumentException>(() => service.Cadastrar(dto, professorId));
    }
}
