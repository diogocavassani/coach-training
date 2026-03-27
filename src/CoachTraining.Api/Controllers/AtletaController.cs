using CoachTraining.App.DTOs;
using CoachTraining.App.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoachTraining.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AtletaController : ControllerBase
{
    private readonly CadastroAtletaService _cadastroService;
    private readonly ILogger<AtletaController> _logger;

    public AtletaController(
        CadastroAtletaService cadastroService,
        ILogger<AtletaController> logger)
    {
        _cadastroService = cadastroService ?? throw new ArgumentNullException(nameof(cadastroService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost]
    public IActionResult Cadastrar([FromBody] CriarAtletaDto dto)
    {
        try
        {
            if (dto == null)
            {
                _logger.LogWarning("Requisicao de cadastro com corpo vazio");
                return BadRequest(new { erro = "Corpo da requisicao nao pode estar vazio" });
            }

            if (string.IsNullOrWhiteSpace(dto.Nome))
            {
                _logger.LogWarning("Tentativa de cadastro com nome vazio ou invalido");
                return BadRequest(new { erro = "Nome do atleta e obrigatorio e nao pode estar vazio" });
            }

            _logger.LogInformation("Recebida requisicao de cadastro de atleta: {NomeAtleta}", dto.Nome);

            var atleta = _cadastroService.Cadastrar(dto, Guid.NewGuid());

            _logger.LogInformation("Atleta {AtletaId} cadastrado com sucesso via API", atleta.Id);

            return CreatedAtAction(nameof(ObterPorId), new { id = atleta.Id }, atleta);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Erro de validacao: {Mensagem}", ex.Message);
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar cadastro de atleta");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { erro = "Erro ao processar requisicao" });
        }
    }

    [HttpGet("{id:guid}")]
    public IActionResult ObterPorId(Guid id)
    {
        if (id == Guid.Empty)
        {
            _logger.LogWarning("Consulta de atleta com id vazio");
            return BadRequest(new { erro = "Id do atleta invalido" });
        }

        var atleta = _cadastroService.ObterPorId(id, Guid.NewGuid());
        if (atleta == null)
        {
            _logger.LogInformation("Atleta {AtletaId} nao encontrado", id);
            return NotFound(new { erro = "Atleta nao encontrado" });
        }

        return Ok(atleta);
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "Atleta service is healthy" });
    }
}
