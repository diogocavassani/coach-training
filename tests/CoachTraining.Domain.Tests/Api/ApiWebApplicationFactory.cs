using CoachTraining.Infra.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CoachTraining.Tests.Api;

public class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string JwtIssuerEnvironmentVariable = "Jwt__Issuer";
    private const string JwtAudienceEnvironmentVariable = "Jwt__Audience";
    private const string JwtKeyEnvironmentVariable = "Jwt__Key";
    private const string JwtExpirationHoursEnvironmentVariable = "Jwt__ExpirationHours";

    private readonly string _databaseName = $"coachtraining-tests-{Guid.NewGuid()}";
    private readonly Dictionary<string, string?> _originalEnvironmentValues = new(StringComparer.Ordinal);

    public ApiWebApplicationFactory()
    {
        SetEnvironmentVariableForTest(JwtIssuerEnvironmentVariable, "CoachTraining");
        SetEnvironmentVariableForTest(JwtAudienceEnvironmentVariable, "CoachTraining.Client");
        SetEnvironmentVariableForTest(JwtKeyEnvironmentVariable, "testing-jwt-secret-key-with-32-plus-characters");
        SetEnvironmentVariableForTest(JwtExpirationHoursEnvironmentVariable, "8");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<CoachTrainingDbContext>>();
            services.RemoveAll<CoachTrainingDbContext>();

            services.AddDbContext<CoachTrainingDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            foreach (var (key, value) in _originalEnvironmentValues)
            {
                Environment.SetEnvironmentVariable(key, value);
            }
        }

        base.Dispose(disposing);
    }

    private void SetEnvironmentVariableForTest(string name, string value)
    {
        if (!_originalEnvironmentValues.ContainsKey(name))
        {
            _originalEnvironmentValues[name] = Environment.GetEnvironmentVariable(name);
        }

        Environment.SetEnvironmentVariable(name, value);
    }
}
