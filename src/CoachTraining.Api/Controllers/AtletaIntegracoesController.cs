using CoachTraining.Api.Security;
using CoachTraining.App.Services.Integrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoachTraining.Api.Controllers;

[ApiController]
[Route("api/atletas/{atletaId:guid}/integracoes")]
[Authorize]
[Produces("application/json")]
public class AtletaIntegracoesController : ControllerBase
{
    private readonly GerarLinkPublicoIntegracaoService _gerarLinkService;
    private readonly ConsultarIntegracoesAtletaService _consultarService;

    public AtletaIntegracoesController(
        GerarLinkPublicoIntegracaoService gerarLinkService,
        ConsultarIntegracoesAtletaService consultarService)
    {
        _gerarLinkService = gerarLinkService ?? throw new ArgumentNullException(nameof(gerarLinkService));
        _consultarService = consultarService ?? throw new ArgumentNullException(nameof(consultarService));
    }

    [HttpGet]
    public IActionResult Consultar(Guid atletaId)
    {
        if (!User.TryGetProfessorId(out var professorId))
        {
            return Unauthorized(new { erro = "Token invalido: professor_id ausente." });
        }

        try
        {
            return Ok(_consultarService.Consultar(atletaId, professorId));
        }
        catch (UnauthorizedAccessException)
        {
            return NotFound(new { erro = "Atleta nao encontrado." });
        }
    }

    [HttpPost("link")]
    public IActionResult GerarOuObterLink(Guid atletaId)
    {
        if (!User.TryGetProfessorId(out var professorId))
        {
            return Unauthorized(new { erro = "Token invalido: professor_id ausente." });
        }

        try
        {
            return Ok(_gerarLinkService.GerarOuObter(atletaId, professorId));
        }
        catch (UnauthorizedAccessException)
        {
            return NotFound(new { erro = "Atleta nao encontrado." });
        }
    }

    [HttpPost("link/regenerar")]
    public IActionResult RegenerarLink(Guid atletaId)
    {
        if (!User.TryGetProfessorId(out var professorId))
        {
            return Unauthorized(new { erro = "Token invalido: professor_id ausente." });
        }

        try
        {
            return Ok(_gerarLinkService.Regenerar(atletaId, professorId));
        }
        catch (UnauthorizedAccessException)
        {
            return NotFound(new { erro = "Atleta nao encontrado." });
        }
    }
}
