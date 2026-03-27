namespace CoachTraining.App.Abstractions.Security;

public interface IPasswordHasher
{
    string Hash(string senha);
    bool Verify(string senha, string senhaHash);
}
