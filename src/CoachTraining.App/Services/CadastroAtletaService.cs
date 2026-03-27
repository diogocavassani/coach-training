using CoachTraining.App.DTOs;
using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.Domain.Entities;

namespace CoachTraining.App.Services;

/// <summary>
/// Serviço de aplicação responsável por coordenar o cadastro de novos atletas.
/// Orquestra a criação da entidade de domínio e prepara resposta para apresentação.
/// </summary>
public class CadastroAtletaService
{
    private readonly IAtletaRepository _atletaRepository;

    public CadastroAtletaService(IAtletaRepository atletaRepository)
    {
        _atletaRepository = atletaRepository ?? throw new ArgumentNullException(nameof(atletaRepository));
    }

    /// <summary>
    /// Cadastra um novo atleta a partir dos dados fornecidos.
    /// Cria a entidade de domínio, valida e prepara DTO de resposta.
    /// </summary>
    /// <param name="dto">DTO com dados básicos do atleta a cadastrar</param>
    /// <returns>DTO com os dados do atleta cadastrado, incluindo ID gerado</returns>
    /// <exception cref="ArgumentNullException">Se o DTO for nulo</exception>
    /// <exception cref="ArgumentException">Se o nome do atleta for inválido</exception>
    public AtletaDto Cadastrar(CriarAtletaDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto), "DTO de cadastro não pode ser nulo");

        // Cria a entidade de domínio (que faz validações)
        var atleta = new Atleta(
            nome: dto.Nome,
            professorId: Guid.NewGuid(),
            observacoesClinicas: dto.ObservacoesClinicas,
            nivelEsportivo: dto.NivelEsportivo
        );
        _atletaRepository.Adicionar(atleta);

        // Mapeia para DTO de resposta
        return MapearAtletaParaDto(atleta);
    }

    /// <summary>
    /// Obtém entidade de atleta por identificador.
    /// </summary>
    /// <param name="id">Identificador do atleta</param>
    /// <returns>Entidade de atleta, ou nulo se não encontrado</returns>
    public AtletaDto? ObterPorId(Guid id)
    {
        var atleta = ObterEntidadePorId(id);
        if (atleta == null)
            return null;

        return MapearAtletaParaDto(atleta);
    }

    public Atleta? ObterEntidadePorId(Guid id)
    {
        if (id == Guid.Empty)
            return null;

        return _atletaRepository.ObterPorId(id);
    }

    /// <summary>
    /// Mapeia uma entidade Atleta para DTO de resposta.
    /// </summary>
    /// <param name="atleta">Entidade de atleta</param>
    /// <returns>DTO mapeado</returns>
    private AtletaDto MapearAtletaParaDto(Atleta atleta)
    {
        return new AtletaDto
        {
            Id = atleta.Id,
            Nome = atleta.Nome,
            ObservacoesClinicas = atleta.ObservacoesClinicas,
            NivelEsportivo = atleta.NivelEsportivo,
            DataCriacao = DateTime.UtcNow
        };
    }
}

