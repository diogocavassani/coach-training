using CoachTraining.App.DTOs;
using CoachTraining.App.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoachTraining.Api.Controllers;

[ApiController]
[Route("professores")]
[Produces("application/json")]
public class ProfessoresController : ControllerBase
{
    private readonly CadastroProfessorService _cadastroProfessorService;
    private readonly ILogger<ProfessoresController> _logger;

    public ProfessoresController(CadastroProfessorService cadastroProfessorService, ILogger<ProfessoresController> logger)
    {
        _cadastroProfessorService = cadastroProfessorService ?? throw new ArgumentNullException(nameof(cadastroProfessorService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost]
    public IActionResult Cadastrar([FromBody] CriarProfessorDto dto)
    {
        try
        {
            if (dto == null)
            {
                return BadRequest(new { erro = "Corpo da requisicao nao pode estar vazio." });
            }

            var professor = _cadastroProfessorService.Cadastrar(dto);
            return Created($"/professores/{professor.Id}", professor);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Erro de validacao no cadastro de professor.");
            return BadRequest(new { erro = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Falha de regra de negocio no cadastro de professor.");
            return Conflict(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado no cadastro de professor.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { erro = "Erro ao processar requisicao." });
        }
    }
}
