using CoachTraining.App.DTOs;
using CoachTraining.Domain.Entities;
using System.Collections.Concurrent;

namespace CoachTraining.App.Services;

/// <summary>
/// Serviço de aplicação responsável por coordenar o cadastro de novos atletas.
/// Orquestra a criação da entidade de domínio e prepara resposta para apresentação.
/// </summary>
public class CadastroAtletaService
{
    private static readonly ConcurrentDictionary<Guid, Atleta> _atletas = new();

    public CadastroAtletaService() { }

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
            observacoesClinicas: dto.ObservacoesClinicas,
            nivelEsportivo: dto.NivelEsportivo
        );
        _atletas[atleta.Id] = atleta;

        // Mapeia para DTO de resposta
        return MapearAtletaParaDto(atleta);
    }

    /// <summary>
    /// Obtém entidade de atleta por identificador.
    /// </summary>
    /// <param name="id">Identificador do atleta</param>
    /// <returns>Entidade de atleta, ou nulo se não encontrado</returns>
    public Atleta? ObterEntidadePorId(Guid id)
    {
        if (id == Guid.Empty)
            return null;

        return _atletas.TryGetValue(id, out var atleta) ? atleta : null;
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
