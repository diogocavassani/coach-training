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

        public Atleta? ObterPorId(Guid atletaId, Guid professorId)
            => _atletas.TryGetValue(atletaId, out var atleta) && atleta.ProfessorId == professorId ? atleta : null;
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
}
