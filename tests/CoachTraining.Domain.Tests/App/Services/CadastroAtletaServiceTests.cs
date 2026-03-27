using CoachTraining.App.DTOs;
using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.App.Services;
using CoachTraining.Domain.Entities;

namespace CoachTraining.Tests.App.Services;

public class CadastroAtletaServiceTests
{
    private sealed class AtletaRepositoryFake : IAtletaRepository
    {
        private readonly Dictionary<Guid, Atleta> _atletas = new();

        public void Adicionar(Atleta atleta) => _atletas[atleta.Id] = atleta;

        public Atleta? ObterPorId(Guid atletaId)
            => _atletas.TryGetValue(atletaId, out var atleta) ? atleta : null;
    }

    [Fact]
    public void Cadastrar_DevePermitirRecuperacaoDaEntidadePorId()
    {
        var service = new CadastroAtletaService(new AtletaRepositoryFake());
        var dto = new CriarAtletaDto
        {
            Nome = "Diogo Teste",
            ObservacoesClinicas = "Sem restrições",
            NivelEsportivo = "Intermediário"
        };

        var atletaCadastrado = service.Cadastrar(dto);
        var entidadeRecuperada = service.ObterEntidadePorId(atletaCadastrado.Id);

        Assert.NotNull(entidadeRecuperada);
        Assert.Equal(atletaCadastrado.Id, entidadeRecuperada!.Id);
        Assert.Equal(dto.Nome, entidadeRecuperada.Nome);
    }

    [Fact]
    public void ObterEntidadePorId_QuandoNaoExiste_DeveRetornarNull()
    {
        var service = new CadastroAtletaService(new AtletaRepositoryFake());

        var entidade = service.ObterEntidadePorId(Guid.NewGuid());

        Assert.Null(entidade);
    }
}
