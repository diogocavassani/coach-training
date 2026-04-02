using CoachTraining.Api.Security;
using CoachTraining.App.DTOs;
using CoachTraining.App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoachTraining.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class AtletaController : ControllerBase
{
    private readonly CadastroAtletaService _cadastroService;
    private readonly GerenciarProvaAlvoService _provaAlvoService;
    private readonly GerenciarPlanejamentoBaseService _planejamentoBaseService;
    private readonly ILogger<AtletaController> _logger;

    public AtletaController(
        CadastroAtletaService cadastroService,
        GerenciarProvaAlvoService provaAlvoService,
        GerenciarPlanejamentoBaseService planejamentoBaseService,
        ILogger<AtletaController> logger)
    {
        _cadastroService = cadastroService ?? throw new ArgumentNullException(nameof(cadastroService));
        _provaAlvoService = provaAlvoService ?? throw new ArgumentNullException(nameof(provaAlvoService));
        _planejamentoBaseService = planejamentoBaseService ?? throw new ArgumentNullException(nameof(planejamentoBaseService));
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

            if (!User.TryGetProfessorId(out var professorId))
            {
                return Unauthorized(new { erro = "Token invalido: professor_id ausente." });
            }

            var atleta = _cadastroService.Cadastrar(dto, professorId);

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


    [HttpGet]
    public IActionResult Listar()
    {
        if (!User.TryGetProfessorId(out var professorId))
        {
            return Unauthorized(new { erro = "Token invalido: professor_id ausente." });
        }

        var atletas = _cadastroService.ListarPorProfessor(professorId);
        return Ok(atletas);
    }

    [HttpGet("{id:guid}")]
    public IActionResult ObterPorId(Guid id)
    {
        if (id == Guid.Empty)
        {
            _logger.LogWarning("Consulta de atleta com id vazio");
            return BadRequest(new { erro = "Id do atleta invalido" });
        }

        if (!User.TryGetProfessorId(out var professorId))
        {
            return Unauthorized(new { erro = "Token invalido: professor_id ausente." });
        }

        var atleta = _cadastroService.ObterPorId(id, professorId);
        if (atleta == null)
        {
            _logger.LogInformation("Atleta {AtletaId} nao encontrado", id);
            return NotFound(new { erro = "Atleta nao encontrado" });
        }

        return Ok(atleta);
    }

    [HttpGet("{id:guid}/prova-alvo")]
    public IActionResult ObterProvaAlvo(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest(new { erro = "Id do atleta invalido" });
        }

        if (!User.TryGetProfessorId(out var professorId))
        {
            return Unauthorized(new { erro = "Token invalido: professor_id ausente." });
        }

        var provaAlvo = _provaAlvoService.ObterPorAtletaId(id, professorId);
        if (provaAlvo == null)
        {
            return NotFound(new { erro = "Prova alvo nao encontrada para o atleta informado." });
        }

        return Ok(provaAlvo);
    }

    [HttpPut("{id:guid}/prova-alvo")]
    public IActionResult SalvarProvaAlvo(Guid id, [FromBody] SalvarProvaAlvoDto dto)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new { erro = "Id do atleta invalido" });
            }

            if (dto == null)
            {
                return BadRequest(new { erro = "Corpo da requisicao nao pode estar vazio" });
            }

            if (!User.TryGetProfessorId(out var professorId))
            {
                return Unauthorized(new { erro = "Token invalido: professor_id ausente." });
            }

            var provaAlvo = _provaAlvoService.Salvar(id, dto, professorId);
            return Ok(provaAlvo);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Professor sem ownership para salvar prova alvo do atleta {AtletaId}", id);
            return Forbid();
        }
        catch (ArgumentOutOfRangeException ex)
        {
            _logger.LogWarning(ex, "Erro de faixa ao salvar prova alvo do atleta {AtletaId}", id);
            return BadRequest(new { erro = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Erro de validacao ao salvar prova alvo do atleta {AtletaId}", id);
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpGet("{id:guid}/planejamento-base")]
    public IActionResult ObterPlanejamentoBase(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest(new { erro = "Id do atleta invalido" });
        }

        if (!User.TryGetProfessorId(out var professorId))
        {
            return Unauthorized(new { erro = "Token invalido: professor_id ausente." });
        }

        var planejamentoBase = _planejamentoBaseService.ObterPorAtletaId(id, professorId);
        if (planejamentoBase == null)
        {
            return NotFound(new { erro = "Planejamento base nao encontrado para o atleta informado." });
        }

        return Ok(planejamentoBase);
    }

    [HttpPut("{id:guid}/planejamento-base")]
    public IActionResult SalvarPlanejamentoBase(Guid id, [FromBody] SalvarPlanejamentoBaseDto dto)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new { erro = "Id do atleta invalido" });
            }

            if (dto == null)
            {
                return BadRequest(new { erro = "Corpo da requisicao nao pode estar vazio" });
            }

            if (!User.TryGetProfessorId(out var professorId))
            {
                return Unauthorized(new { erro = "Token invalido: professor_id ausente." });
            }

            var planejamentoBase = _planejamentoBaseService.Salvar(id, dto, professorId);
            return Ok(planejamentoBase);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Professor sem ownership para salvar planejamento base do atleta {AtletaId}", id);
            return Forbid();
        }
        catch (ArgumentOutOfRangeException ex)
        {
            _logger.LogWarning(ex, "Erro de faixa ao salvar planejamento base do atleta {AtletaId}", id);
            return BadRequest(new { erro = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Erro de validacao ao salvar planejamento base do atleta {AtletaId}", id);
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult Health()
    {
        return Ok(new { status = "Atleta service is healthy" });
    }
}
