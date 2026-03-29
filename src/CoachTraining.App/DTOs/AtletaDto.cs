namespace CoachTraining.App.DTOs;

/// <summary>
/// DTO para resposta de cadastro ou consulta de atleta.
/// </summary>
public class AtletaDto
{
    /// <summary>
    /// Identificador unico do atleta.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Identificador do professor dono do atleta.
    /// </summary>
    public Guid ProfessorId { get; set; }

    /// <summary>
    /// Nome completo do atleta.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Email do atleta.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Observacoes clinicas do atleta.
    /// </summary>
    public string? ObservacoesClinicas { get; set; }

    /// <summary>
    /// Nivel/categoria esportiva do atleta.
    /// </summary>
    public string? NivelEsportivo { get; set; }

    /// <summary>
    /// Data de criacao do registro do atleta.
    /// </summary>
    public DateTime DataCriacao { get; set; }
}
