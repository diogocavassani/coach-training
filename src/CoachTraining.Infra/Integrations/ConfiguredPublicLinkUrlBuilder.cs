using CoachTraining.App.Abstractions.Integrations;
using Microsoft.Extensions.Configuration;

namespace CoachTraining.Infra.Integrations;

public class ConfiguredPublicLinkUrlBuilder : IPublicLinkUrlBuilder
{
    private readonly string _baseUrl;

    public ConfiguredPublicLinkUrlBuilder(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        _baseUrl = configuration["PublicApp:BaseUrl"]?.TrimEnd('/') ?? "https://coachtraining.com";
    }

    public string BuildConnectionUrl(string tokenPublico)
    {
        if (string.IsNullOrWhiteSpace(tokenPublico))
        {
            throw new ArgumentException("Token publico obrigatorio.", nameof(tokenPublico));
        }

        return $"{_baseUrl}/conectar/{Uri.EscapeDataString(tokenPublico.Trim())}";
    }
}
