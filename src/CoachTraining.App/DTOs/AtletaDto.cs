namespace CoachTraining.App.DTOs;

/// <summary>
/// DTO para resposta de cadastro ou consulta de atleta.
/// Retorna dados da entidade de atleta criada ou consultada.
/// </summary>
public class AtletaDto
{
    /// <summary>
    /// Identificador ??nico do atleta (GUID).
    /// Gerado automaticamente no cadastro.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome completo do atleta.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Observa??es cl??nicas do atleta.
    /// </summary>
    public string? ObservacoesClinicas { get; set; }

    /// <summary>
    /// N??vel/categoria esportiva do atleta.
    /// </summary>
    public string? NivelEsportivo { get; set; }

    /// <summary>
    /// Data de cria??o do registro do atleta.
    /// </summary>
    public DateTime DataCriacao { get; set; }
}
