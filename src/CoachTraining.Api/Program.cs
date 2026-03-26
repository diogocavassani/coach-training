using CoachTraining.App.Services;

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

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

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
