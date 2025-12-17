using System;

namespace CoachTraining.Domain.Entities;

public class Coach
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }

    public Coach(string nome)
    {
        Id = Guid.NewGuid();
        Nome = string.IsNullOrWhiteSpace(nome) ? throw new ArgumentException("Nome obrigatório", nameof(nome)) : nome.Trim();
    }
}
