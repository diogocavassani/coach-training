using CoachTraining.App.DTOs;
using CoachTraining.App.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoachTraining.Api.Controllers;

[ApiController]
[Route("auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly LoginProfessorService _loginProfessorService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(LoginProfessorService loginProfessorService, ILogger<AuthController> logger)
    {
        _loginProfessorService = loginProfessorService ?? throw new ArgumentNullException(nameof(loginProfessorService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        try
        {
            if (dto == null)
            {
                return BadRequest(new { erro = "Corpo da requisicao nao pode estar vazio." });
            }

            var response = _loginProfessorService.Login(dto);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Tentativa de login com credenciais invalidas.");
            return Unauthorized(new { erro = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Erro de validacao no login.");
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado no login.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { erro = "Erro ao processar requisicao." });
        }
    }
}
