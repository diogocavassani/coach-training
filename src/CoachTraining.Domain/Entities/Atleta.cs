using System;
using System.Net.Mail;

namespace CoachTraining.Domain.Entities;

public class Atleta
{
    public Guid Id { get; private set; }
    public Guid ProfessorId { get; private set; }
    public string Nome { get; private set; }
    public string? Email { get; private set; }
    public string? ObservacoesClinicas { get; private set; }
    public string? NivelEsportivo { get; private set; }

    public Atleta(
        string nome,
        Guid professorId,
        string? observacoesClinicas = null,
        string? nivelEsportivo = null,
        string? email = null,
        Guid? id = null)
    {
        ProfessorId = professorId == Guid.Empty
            ? throw new ArgumentException("ProfessorId obrigatorio", nameof(professorId))
            : professorId;
        Id = id ?? Guid.NewGuid();
        Nome = string.IsNullOrWhiteSpace(nome)
            ? throw new ArgumentException("Nome obrigatorio", nameof(nome))
            : nome.Trim();
        Email = NormalizarEmail(email);
        ObservacoesClinicas = observacoesClinicas?.Trim();
        NivelEsportivo = nivelEsportivo?.Trim();
    }

    private static string? NormalizarEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        var emailNormalizado = email.Trim();
        if (!MailAddress.TryCreate(emailNormalizado, out _))
        {
            throw new ArgumentException("Email invalido", nameof(email));
        }

        return emailNormalizado;
    }
}
