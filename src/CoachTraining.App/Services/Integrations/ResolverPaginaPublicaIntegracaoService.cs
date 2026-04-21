using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.App.DTOs.Integrations;

namespace CoachTraining.App.Services.Integrations;

public class ResolverPaginaPublicaIntegracaoService
{
    private readonly ILinkPublicoIntegracaoRepository _linkRepository;
    private readonly IConexaoWearableRepository _conexaoRepository;

    public ResolverPaginaPublicaIntegracaoService(
        ILinkPublicoIntegracaoRepository linkRepository,
        IConexaoWearableRepository conexaoRepository)
    {
        _linkRepository = linkRepository ?? throw new ArgumentNullException(nameof(linkRepository));
        _conexaoRepository = conexaoRepository ?? throw new ArgumentNullException(nameof(conexaoRepository));
    }

    public PublicIntegrationPageDto? Resolver(string tokenPublico)
    {
        if (string.IsNullOrWhiteSpace(tokenPublico))
        {
            return null;
        }

        var tokenHash = GerarLinkPublicoIntegracaoService.GerarHash(tokenPublico.Trim());
        var link = _linkRepository.ObterPorTokenHash(tokenHash);
        if (link == null || !link.Ativo)
        {
            return null;
        }

        return new PublicIntegrationPageDto
        {
            Titulo = "Conectar aplicativos ao CoachTraining",
            Descricao = "Autorize o envio automatico das atividades concluidas para o CoachTraining sem precisar fazer login.",
            Provedores = IntegracaoProviderCatalog.CriarStatus(_conexaoRepository.ListarPorAtletaId(link.AtletaId))
        };
    }
}
