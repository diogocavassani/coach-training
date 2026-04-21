using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.App.Abstractions.Integrations;
using CoachTraining.App.Abstractions.Security;
using CoachTraining.Infra.Integrations;
using CoachTraining.Infra.Persistence;
using CoachTraining.Infra.Persistence.Repositories;
using CoachTraining.Infra.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoachTraining.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDataProtection();

        services.AddDbContext<CoachTrainingDbContext>(options =>
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                options.UseNpgsql(connectionString);
                return;
            }

            options.UseInMemoryDatabase("CoachTrainingDb");
        });

        services.AddScoped<IAtletaRepository, AtletaRepository>();
        services.AddScoped<IProfessorRepository, ProfessorRepository>();
        services.AddScoped<ISessaoDeTreinoRepository, SessaoDeTreinoRepository>();
        services.AddScoped<IProvaAlvoRepository, ProvaAlvoRepository>();
        services.AddScoped<ILinkPublicoIntegracaoRepository, LinkPublicoIntegracaoRepository>();
        services.AddScoped<IConexaoWearableRepository, ConexaoWearableRepository>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<ISecretProtector, DataProtectionSecretProtector>();
        services.AddSingleton<IPublicLinkUrlBuilder, ConfiguredPublicLinkUrlBuilder>();

        return services;
    }
}
