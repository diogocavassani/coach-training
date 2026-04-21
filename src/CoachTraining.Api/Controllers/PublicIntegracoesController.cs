using CoachTraining.App.Services.Integrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoachTraining.Api.Controllers;

[ApiController]
[Route("public/integracoes")]
[AllowAnonymous]
[Produces("application/json")]
public class PublicIntegracoesController : ControllerBase
{
    private readonly ResolverPaginaPublicaIntegracaoService _resolverPaginaService;

    public PublicIntegracoesController(ResolverPaginaPublicaIntegracaoService resolverPaginaService)
    {
        _resolverPaginaService = resolverPaginaService ?? throw new ArgumentNullException(nameof(resolverPaginaService));
    }

    [HttpGet("{token}")]
    public IActionResult ResolverPagina(string token)
    {
        var payload = _resolverPaginaService.Resolver(token);
        return payload == null ? NotFound(new { erro = "Link de integracao nao encontrado." }) : Ok(payload);
    }
}
