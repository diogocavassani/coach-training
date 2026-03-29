using CoachTraining.Api.Security;
using CoachTraining.App.DTOs;
using CoachTraining.App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoachTraining.Api.Controllers;

[ApiController]
[Route("treinos")]
[Produces("application/json")]
[Authorize]
public class TreinosController : ControllerBase
{
    private readonly CadastrarSessaoDeTreinoService _service;
    private readonly ILogger<TreinosController> _logger;

    public TreinosController(CadastrarSessaoDeTreinoService service, ILogger<TreinosController> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost]
    public IActionResult Cadastrar([FromBody] CadastrarSessaoDeTreinoDto dto)
    {
        try
        {
            if (dto == null)
            {
                return BadRequest(new { erro = "Corpo da requisicao nao pode estar vazio" });
            }

            if (dto.AtletaId == Guid.Empty)
            {
                return BadRequest(new { erro = "AtletaId obrigatorio" });
            }

            if (!User.TryGetProfessorId(out var professorId))
            {
                return Unauthorized(new { erro = "Token invalido: professor_id ausente." });
            }

            var sessao = _service.Cadastrar(dto, professorId);
            return Created($"/treinos/{sessao.Id}", sessao);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Tentativa de cadastro de treino sem ownership valido.");
            return Forbid();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Erro de validacao no cadastro de treino.");
            return BadRequest(new { erro = ex.Message });
        }
        catch (ArgumentOutOfRangeException ex)
        {
            _logger.LogWarning(ex, "Erro de validacao de faixa no cadastro de treino.");
            return BadRequest(new { erro = ex.Message });
        }
    }
}
