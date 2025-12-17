using System;
using CoachTraining.Domain.ValueObjects;

namespace CoachTraining.Domain.Entities;

public class Atleta
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public string? ObservacoesClinicas { get; private set; }
    public string? NivelEsportivo { get; private set; }

    public Atleta(string nome, string? observacoesClinicas = null, string? nivelEsportivo = null)
    {
        Id = Guid.NewGuid();
        Nome = string.IsNullOrWhiteSpace(nome) ? throw new ArgumentException("Nome obrigatório", nameof(nome)) : nome.Trim();
        ObservacoesClinicas = observacoesClinicas?.Trim();
        NivelEsportivo = nivelEsportivo?.Trim();
    }
}
