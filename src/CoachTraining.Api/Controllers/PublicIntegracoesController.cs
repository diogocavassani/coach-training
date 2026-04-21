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
    private readonly IniciarAutorizacaoStravaService _iniciarAutorizacaoStravaService;
    private readonly ConcluirAutorizacaoStravaService _concluirAutorizacaoStravaService;

    public PublicIntegracoesController(
        ResolverPaginaPublicaIntegracaoService resolverPaginaService,
        IniciarAutorizacaoStravaService iniciarAutorizacaoStravaService,
        ConcluirAutorizacaoStravaService concluirAutorizacaoStravaService)
    {
        _resolverPaginaService = resolverPaginaService ?? throw new ArgumentNullException(nameof(resolverPaginaService));
        _iniciarAutorizacaoStravaService = iniciarAutorizacaoStravaService ?? throw new ArgumentNullException(nameof(iniciarAutorizacaoStravaService));
        _concluirAutorizacaoStravaService = concluirAutorizacaoStravaService ?? throw new ArgumentNullException(nameof(concluirAutorizacaoStravaService));
    }

    [HttpGet("{token}")]
    public IActionResult ResolverPagina(string token)
    {
        var payload = _resolverPaginaService.Resolver(token);
        return payload == null ? NotFound(new { erro = "Link de integracao nao encontrado." }) : Ok(payload);
    }

    [HttpPost("{token}/strava/autorizar")]
    public IActionResult IniciarAutorizacaoStrava(string token)
    {
        try
        {
            return Ok(_iniciarAutorizacaoStravaService.Iniciar(token));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { erro = "Link de integracao nao encontrado." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpGet("strava/callback")]
    public async Task<IActionResult> CallbackStrava(
        [FromQuery] string? code,
        [FromQuery] string? scope,
        [FromQuery] string? state,
        [FromQuery] string? error)
    {
        var resultado = await _concluirAutorizacaoStravaService.ConcluirAsync(code, scope, state, error, HttpContext.RequestAborted);
        return Redirect(resultado.RedirectUrl);
    }
}
