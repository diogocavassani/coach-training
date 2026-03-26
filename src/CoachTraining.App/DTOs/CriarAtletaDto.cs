namespace CoachTraining.App.DTOs;

/// <summary>
/// DTO para requisi??o de cadastro de novo atleta.
/// Recebe dados b??sicos para cria??o da entidade de atleta.
/// </summary>
public class CriarAtletaDto
{
    /// <summary>
    /// Nome completo do atleta.
    /// Campo obrigat??rio, n??o pode estar vazio ou com apenas espa??os.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Observa??es cl??nicas do atleta.
    /// Campo opcional: alergias, les??es, medica??es, etc.
    /// </summary>
    public string? ObservacoesClinicas { get; set; }

    /// <summary>
    /// N??vel/categoria esportiva do atleta.
    /// Campo opcional: ex. "Elite", "Sub-23", "Amador", etc.
    /// </summary>
    public string? NivelEsportivo { get; set; }
}
