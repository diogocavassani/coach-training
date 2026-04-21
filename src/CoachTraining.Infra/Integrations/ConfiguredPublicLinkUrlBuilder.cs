using CoachTraining.App.Abstractions.Integrations;
using Microsoft.Extensions.Configuration;

namespace CoachTraining.Infra.Integrations;

public class ConfiguredPublicLinkUrlBuilder : IPublicLinkUrlBuilder
{
    private readonly string _baseUrl;
    private readonly string _apiBaseUrl;

    public ConfiguredPublicLinkUrlBuilder(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        _baseUrl = configuration["PublicApp:BaseUrl"]?.TrimEnd('/') ?? "https://coachtraining.com";
        _apiBaseUrl = configuration["PublicApi:BaseUrl"]?.TrimEnd('/') ?? _baseUrl;
    }

    public string BuildConnectionUrl(string tokenPublico)
    {
        if (string.IsNullOrWhiteSpace(tokenPublico))
        {
            throw new ArgumentException("Token publico obrigatorio.", nameof(tokenPublico));
        }

        return $"{_baseUrl}/conectar/{Uri.EscapeDataString(tokenPublico.Trim())}";
    }

    public string BuildStravaCallbackUrl()
    {
        return $"{_apiBaseUrl}/public/integracoes/strava/callback";
    }

    public string BuildStravaReturnUrl(string status, string? reason = null)
    {
        var url = $"{_baseUrl}/conectar/strava/retorno?status={Uri.EscapeDataString(status)}";
        if (string.IsNullOrWhiteSpace(reason))
        {
            return url;
        }

        return $"{url}&reason={Uri.EscapeDataString(reason)}";
    }
}
