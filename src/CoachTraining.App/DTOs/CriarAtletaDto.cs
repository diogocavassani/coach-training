namespace CoachTraining.App.DTOs;

/// <summary>
/// DTO para requisicao de cadastro de novo atleta.
/// </summary>
public class CriarAtletaDto
{
    /// <summary>
    /// Nome completo do atleta.
    /// Campo obrigatorio, nao pode estar vazio ou com apenas espacos.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Email do atleta.
    /// Campo opcional.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Observacoes clinicas do atleta.
    /// Campo opcional: alergias, lesoes, medicacoes, etc.
    /// </summary>
    public string? ObservacoesClinicas { get; set; }

    /// <summary>
    /// Nivel/categoria esportiva do atleta.
    /// Campo opcional: ex. "Elite", "Sub-23", "Amador", etc.
    /// </summary>
    public string? NivelEsportivo { get; set; }
}
