namespace CoachTraining.App.DTOs;

/// <summary>
/// DTO para requisição de cadastro de novo atleta.
/// Recebe dados básicos para criação da entidade de atleta.
/// </summary>
public class CriarAtletaDto
{
    /// <summary>
    /// Nome completo do atleta.
    /// Campo obrigatório, não pode estar vazio ou com apenas espaços.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Observações clínicas do atleta.
    /// Campo opcional: alergias, lesões, medicações, etc.
    /// </summary>
    public string? ObservacoesClinicas { get; set; }

    /// <summary>
    /// Nível/categoria esportiva do atleta.
    /// Campo opcional: ex. "Elite", "Sub-23", "Amador", etc.
    /// </summary>
    public string? NivelEsportivo { get; set; }
}