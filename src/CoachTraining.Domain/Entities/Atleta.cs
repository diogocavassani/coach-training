using System;

namespace CoachTraining.Domain.Entities;

public class Atleta
{
    public Guid Id { get; private set; }
    public Guid ProfessorId { get; private set; }
    public string Nome { get; private set; }
    public string? ObservacoesClinicas { get; private set; }
    public string? NivelEsportivo { get; private set; }

    public Atleta(string nome, Guid professorId, string? observacoesClinicas = null, string? nivelEsportivo = null, Guid? id = null)
    {
        ProfessorId = professorId == Guid.Empty
            ? throw new ArgumentException("ProfessorId obrigatorio", nameof(professorId))
            : professorId;
        Id = id ?? Guid.NewGuid();
        Nome = string.IsNullOrWhiteSpace(nome)
            ? throw new ArgumentException("Nome obrigatorio", nameof(nome))
            : nome.Trim();
        ObservacoesClinicas = observacoesClinicas?.Trim();
        NivelEsportivo = nivelEsportivo?.Trim();
    }
}
