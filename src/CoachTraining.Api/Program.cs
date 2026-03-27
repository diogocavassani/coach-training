using CoachTraining.App.Services;
using CoachTraining.Infra;
using CoachTraining.Infra.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(config =>
{
    config.ClearProviders();
    config.AddConsole();
    config.AddDebug();
});

// Registrar Application Services
builder.Services.AddScoped<CadastroAtletaService>();
builder.Services.AddScoped<ObterDashboardAtletaService>();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CoachTrainingDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Imagens Docker oficiais definem DOTNET_RUNNING_IN_CONTAINER; sem TLS no contêiner, HTTPS redirect quebra o cliente HTTP.
if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true")
{
    app.UseHttpsRedirection();
}
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();
