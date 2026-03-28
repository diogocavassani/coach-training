using CoachTraining.Api.Security;
using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoachTraining.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly ObterDashboardAtletaService _dashboardService;
    private readonly IAtletaRepository _atletaRepository;
    private readonly ISessaoDeTreinoRepository _sessaoDeTreinoRepository;
    private readonly IProvaAlvoRepository _provaAlvoRepository;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IAtletaRepository atletaRepository,
        ISessaoDeTreinoRepository sessaoDeTreinoRepository,
        IProvaAlvoRepository provaAlvoRepository,
        ObterDashboardAtletaService dashboardService,
        ILogger<DashboardController> logger)
    {
        _atletaRepository = atletaRepository ?? throw new ArgumentNullException(nameof(atletaRepository));
        _sessaoDeTreinoRepository = sessaoDeTreinoRepository ?? throw new ArgumentNullException(nameof(sessaoDeTreinoRepository));
        _provaAlvoRepository = provaAlvoRepository ?? throw new ArgumentNullException(nameof(provaAlvoRepository));
        _dashboardService = dashboardService ?? throw new ArgumentNullException(nameof(dashboardService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("atleta/{id:guid}")]
    public IActionResult ObterDashboard(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new { erro = "AtletaId invalido." });
            }

            if (!User.TryGetProfessorId(out var professorId))
            {
                return Unauthorized(new { erro = "Token invalido: professor_id ausente." });
            }

            var atleta = _atletaRepository.ObterPorId(id, professorId);
            if (atleta == null)
            {
                return NotFound(new { erro = "Atleta nao encontrado." });
            }

            var sessoes = _sessaoDeTreinoRepository.ObterPorAtletaId(id, professorId);
            var provaAlvo = _provaAlvoRepository.ObterPorAtletaId(id, professorId);
            var dashboard = _dashboardService.ObterDashboard(atleta, sessoes, provaAlvo);

            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter dashboard para atleta {AtletaId}.", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { erro = "Erro ao processar requisicao." });
        }
    }

    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult Health()
    {
        return Ok(new { status = "Dashboard service is healthy" });
    }
}
