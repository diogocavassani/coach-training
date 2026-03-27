using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.Infra.Persistence;
using CoachTraining.Infra.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoachTraining.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

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

        return services;
    }
}
