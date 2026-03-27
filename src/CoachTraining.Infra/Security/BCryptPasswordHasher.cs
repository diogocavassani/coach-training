using CoachTraining.App.Abstractions.Security;

namespace CoachTraining.Infra.Security;

public class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string senha)
    {
        if (string.IsNullOrWhiteSpace(senha))
        {
            throw new ArgumentException("Senha obrigatoria.", nameof(senha));
        }

        return BCrypt.Net.BCrypt.HashPassword(senha);
    }

    public bool Verify(string senha, string senhaHash)
    {
        if (string.IsNullOrWhiteSpace(senha) || string.IsNullOrWhiteSpace(senhaHash))
        {
            return false;
        }

        return BCrypt.Net.BCrypt.Verify(senha, senhaHash);
    }
}
