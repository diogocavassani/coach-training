using CoachTraining.App.DTOs;
using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.App.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace CoachTraining.Api.Controllers;

/// <summary>
/// Controlador responsável por servir as métricas consolidadas do dashboard.
/// Fornece visão holística da saúde e progressão do treinamento do atleta.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
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

    /// <summary>
    /// Obtém o dashboard consolidado de um atleta.
    /// Retorna carga de treino, ACWR, fase do ciclo, risco e informações de taper.
    /// </summary>
    /// <param name="id">Identificador único do atleta (GUID)</param>
    /// <returns>DTO com métricas consolidadas do atleta</returns>
    /// <response code="200">Dashboard recuperado com sucesso</response>
    /// <response code="404">Atleta não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("atleta/{id}")]
    public IActionResult ObterDashboard(Guid id)
    {
        try
        {
            _logger.LogInformation("Requisição de dashboard para atleta {AtletaId}", id);

            if (id == Guid.Empty)
            {
                _logger.LogWarning("AtletaId vazio recebido");
                return BadRequest(new { erro = "AtletaId inválido" });
            }

            var professorId = Guid.NewGuid();
            var atleta = _atletaRepository.ObterPorId(id, professorId);
            if (atleta == null)
            {
                _logger.LogInformation("Atleta {AtletaId} não encontrado", id);
                return NotFound(new { erro = "Atleta não encontrado" });
            }

            var sessoes = _sessaoDeTreinoRepository.ObterPorAtletaId(id, professorId);
            var provaAlvo = _provaAlvoRepository.ObterPorAtletaId(id, professorId);
            var dashboard = _dashboardService.ObterDashboard(atleta, sessoes, provaAlvo);
            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter dashboard para atleta {AtletaId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { erro = "Erro ao processar requisição" });
        }
    }

    /// <summary>
    /// Health check para validar disponibilidade do serviço de dashboard.
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "Dashboard service is healthy" });
    }
}
