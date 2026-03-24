using CoachTraining.App.DTOs;
using CoachTraining.App.Services;

namespace CoachTraining.Tests.App.Services;

public class CadastroAtletaServiceTests
{
    [Fact]
    public void Cadastrar_DevePermitirRecuperacaoDaEntidadePorId()
    {
        var service = new CadastroAtletaService();
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
        var service = new CadastroAtletaService();

        var entidade = service.ObterEntidadePorId(Guid.NewGuid());

        Assert.Null(entidade);
    }
}
