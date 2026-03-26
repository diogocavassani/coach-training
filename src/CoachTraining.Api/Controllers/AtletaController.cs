using CoachTraining.App.DTOs;
using CoachTraining.App.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace CoachTraining.Api.Controllers;

/// <summary>
/// Controlador responsável por gerenciar operações relacionadas a atletas.
/// Fornece endpoints para cadastro, consulta e gerenciamento de dados de atletas.
/// </summary>
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

    /// <summary>
    /// Cadastra um novo atleta no sistema.
    /// Cria um registro de atleta com dados básicos fornecidos.
    /// </summary>
    /// <param name="dto">DTO contendo nome, observações clínicas e nível esportivo</param>
    /// <returns>DTO com os dados do atleta cadastrado, incluindo ID gerado</returns>
    /// <response code="201">Atleta criado com sucesso</response>
    /// <response code="400">Dados inválidos fornecidos</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    public IActionResult Cadastrar([FromBody] CriarAtletaDto dto)
    {
        try
        {
            if (dto == null)
            {
                _logger.LogWarning("Requisição de cadastro com corpo vazio");
                return BadRequest(new { erro = "Corpo da requisição não pode estar vazio" });
            }

            // Validação básica do DTO
            if (string.IsNullOrWhiteSpace(dto.Nome))
            {
                _logger.LogWarning("Tentativa de cadastro com nome vazio ou inválido");
                return BadRequest(new { erro = "Nome do atleta é obrigatório e não pode estar vazio" });
            }

            _logger.LogInformation("Recebida requisição de cadastro de atleta: {NomeAtleta}", dto.Nome);

            // Chama o serviço de aplicação
            var atleta = _cadastroService.Cadastrar(dto);

            _logger.LogInformation("Atleta {AtletaId} cadastrado com sucesso via API", atleta.Id);

            // Retorna 201 Created com a URL do recurso criado
            return CreatedAtAction(nameof(Cadastrar), new { id = atleta.Id }, atleta);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Erro de validação: {Mensagem}", ex.Message);
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar cadastro de atleta");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { erro = "Erro ao processar requisição" });
        }
    }

    /// <summary>
    /// Health check para validar disponibilidade do serviço de atletas.
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "Atleta service is healthy" });
    }
}
