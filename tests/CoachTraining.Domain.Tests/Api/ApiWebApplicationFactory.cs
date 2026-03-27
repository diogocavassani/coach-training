using CoachTraining.Infra.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CoachTraining.Tests.Api;

public class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"coachtraining-tests-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "CoachTraining",
                ["Jwt:Audience"] = "CoachTraining.Client",
                ["Jwt:Key"] = "coach-training-dev-key-change-this-to-32-plus-chars",
                ["Jwt:ExpirationHours"] = "8",
                ["ConnectionStrings:DefaultConnection"] = string.Empty
            });
        });

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
}
