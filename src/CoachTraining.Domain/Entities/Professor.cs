using System.Net.Mail;

namespace CoachTraining.Domain.Entities;

public class Professor
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public string SenhaHash { get; private set; }
    public DateTime DataCriacao { get; private set; }

    public Professor(string nome, string email, string senhaHash, Guid? id = null, DateTime? dataCriacao = null)
    {
        Id = id ?? Guid.NewGuid();
        Nome = string.IsNullOrWhiteSpace(nome)
            ? throw new ArgumentException("Nome obrigatorio", nameof(nome))
            : nome.Trim();
        Email = NormalizarEmail(email);
        SenhaHash = string.IsNullOrWhiteSpace(senhaHash)
            ? throw new ArgumentException("SenhaHash obrigatoria", nameof(senhaHash))
            : senhaHash.Trim();
        DataCriacao = dataCriacao ?? DateTime.UtcNow;
    }

    public static string NormalizarEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email obrigatorio", nameof(email));
        }

        var emailNormalizado = email.Trim().ToLowerInvariant();

        try
        {
            _ = new MailAddress(emailNormalizado);
            return emailNormalizado;
        }
        catch (FormatException ex)
        {
            throw new ArgumentException("Email invalido", nameof(email), ex);
        }
    }
}
