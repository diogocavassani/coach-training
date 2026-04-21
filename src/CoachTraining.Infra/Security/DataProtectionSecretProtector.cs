using CoachTraining.App.Abstractions.Security;
using Microsoft.AspNetCore.DataProtection;

namespace CoachTraining.Infra.Security;

public class DataProtectionSecretProtector : ISecretProtector
{
    private readonly IDataProtector _protector;

    public DataProtectionSecretProtector(IDataProtectionProvider dataProtectionProvider)
    {
        ArgumentNullException.ThrowIfNull(dataProtectionProvider);
        _protector = dataProtectionProvider.CreateProtector("CoachTraining.Integrations.Secrets.v1");
    }

    public string Protect(string plaintext)
    {
        if (string.IsNullOrWhiteSpace(plaintext))
        {
            throw new ArgumentException("Secret plaintext obrigatorio.", nameof(plaintext));
        }

        return _protector.Protect(plaintext.Trim());
    }

    public string Unprotect(string protectedValue)
    {
        if (string.IsNullOrWhiteSpace(protectedValue))
        {
            throw new ArgumentException("Secret protegido obrigatorio.", nameof(protectedValue));
        }

        return _protector.Unprotect(protectedValue.Trim());
    }
}
