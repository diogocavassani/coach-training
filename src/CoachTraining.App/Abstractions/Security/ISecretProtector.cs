namespace CoachTraining.App.Abstractions.Security;

public interface ISecretProtector
{
    string Protect(string plaintext);
    string Unprotect(string protectedValue);
}
