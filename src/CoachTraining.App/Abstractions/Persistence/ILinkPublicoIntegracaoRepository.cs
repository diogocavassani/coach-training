using CoachTraining.Domain.Entities;

namespace CoachTraining.App.Abstractions.Persistence;

public sealed record LinkPublicoIntegracaoData(LinkPublicoIntegracao Link, string TokenProtegido);

public interface ILinkPublicoIntegracaoRepository
{
    LinkPublicoIntegracaoData? ObterAtivoPorAtletaId(Guid atletaId);
    LinkPublicoIntegracao? ObterPorTokenHash(string tokenHash);
    void Salvar(LinkPublicoIntegracao link, string tokenProtegido);
}
