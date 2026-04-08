using CoachTraining.App.DTOs;
using CoachTraining.App.Services;
using CoachTraining.DemoSeed.Reports;
using CoachTraining.DemoSeed.Scenarios;
using CoachTraining.Infra.Persistence;

namespace CoachTraining.DemoSeed;

public sealed class DemoSeedRunner
{
    private readonly CoachTrainingDbContext _dbContext;
    private readonly CadastroProfessorService _cadastroProfessorService;
    private readonly CadastroAtletaService _cadastroAtletaService;
    private readonly CadastrarSessaoDeTreinoService _cadastrarSessaoDeTreinoService;
    private readonly GerenciarProvaAlvoService _gerenciarProvaAlvoService;
    private readonly GerenciarPlanejamentoBaseService _gerenciarPlanejamentoBaseService;

    public DemoSeedRunner(
        CoachTrainingDbContext dbContext,
        CadastroProfessorService cadastroProfessorService,
        CadastroAtletaService cadastroAtletaService,
        CadastrarSessaoDeTreinoService cadastrarSessaoDeTreinoService,
        GerenciarProvaAlvoService gerenciarProvaAlvoService,
        GerenciarPlanejamentoBaseService gerenciarPlanejamentoBaseService)
    {
        _dbContext = dbContext;
        _cadastroProfessorService = cadastroProfessorService;
        _cadastroAtletaService = cadastroAtletaService;
        _cadastrarSessaoDeTreinoService = cadastrarSessaoDeTreinoService;
        _gerenciarProvaAlvoService = gerenciarProvaAlvoService;
        _gerenciarPlanejamentoBaseService = gerenciarPlanejamentoBaseService;
    }

    public async Task<DemoSeedReport> RunAsync(
        DemoSeedOptions options,
        DateOnly? referencia = null,
        CancellationToken cancellationToken = default)
    {
        var dataReferencia = referencia ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var profile = DemoScenarioFactory.CreateProfile(options.Profile, dataReferencia);

        await _dbContext.Database.EnsureCreatedAsync(cancellationToken);

        // Limpar dados de demo anteriores se necessário
        if (options.ResetAll)
        {
            _dbContext.ProvasAlvo.RemoveRange(_dbContext.ProvasAlvo);
            _dbContext.SessoesDeTreino.RemoveRange(_dbContext.SessoesDeTreino);
            _dbContext.Atletas.RemoveRange(_dbContext.Atletas);
            _dbContext.Professores.RemoveRange(_dbContext.Professores);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        else if (options.ResetDemo)
        {
            // Remover apenas o dataset demo
            var demoProfessores = _dbContext.Professores
                .Where(p => p.Email.StartsWith("demo."))
                .ToList();

            foreach (var professor in demoProfessores)
            {
                var demoAtletas = _dbContext.Atletas
                    .Where(a => a.ProfessorId == professor.Id && a.Email != null && a.Email.EndsWith("@demo.local"))
                    .ToList();

                foreach (var atleta in demoAtletas)
                {
                    var sessoes = _dbContext.SessoesDeTreino.Where(s => s.AtletaId == atleta.Id).ToList();
                    var provas = _dbContext.ProvasAlvo.Where(p => p.AtletaId == atleta.Id).ToList();

                    _dbContext.SessoesDeTreino.RemoveRange(sessoes);
                    _dbContext.ProvasAlvo.RemoveRange(provas);
                    _dbContext.Atletas.Remove(atleta);
                }

                _dbContext.Professores.Remove(professor);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        // Criar professor demo
        var professorDto = _cadastroProfessorService.Cadastrar(new CriarProfessorDto
        {
            Nome = profile.ProfessorNome,
            Email = profile.ProfessorEmail,
            Senha = profile.ProfessorSenha
        });

        var atletas = new List<DemoSeedReportAtleta>(profile.Cenarios.Count);

        // Criar atletas e seus históricos
        foreach (var scenario in profile.Cenarios)
        {
            var atletaDto = _cadastroAtletaService.Cadastrar(new CriarAtletaDto
            {
                Nome = scenario.Nome,
                Email = scenario.Email,
                NivelEsportivo = scenario.NivelEsportivo,
                ObservacoesClinicas = scenario.ObservacoesClinicas
            }, professorDto.Id);

            _gerenciarPlanejamentoBaseService.Salvar(
                atletaDto.Id,
                new SalvarPlanejamentoBaseDto
                {
                    TreinosPlanejadosPorSemana = scenario.TreinosPlanejadosPorSemana
                },
                professorDto.Id
            );

            // Registrar sessões
            foreach (var sessao in scenario.Sessoes)
            {
                _cadastrarSessaoDeTreinoService.Cadastrar(new CadastrarSessaoDeTreinoDto
                {
                    AtletaId = atletaDto.Id,
                    Data = sessao.Data,
                    Tipo = sessao.Tipo,
                    DuracaoMinutos = sessao.DuracaoMinutos,
                    DistanciaKm = sessao.DistanciaKm,
                    Rpe = sessao.Rpe
                }, professorDto.Id);
            }

            // Registrar prova alvo se existir
            if (scenario.ProvaAlvo != null)
            {
                _gerenciarProvaAlvoService.Salvar(
                    atletaDto.Id,
                    new SalvarProvaAlvoDto
                    {
                        DataProva = scenario.ProvaAlvo.DataProva,
                        DistanciaKm = scenario.ProvaAlvo.DistanciaKm,
                        Objetivo = scenario.ProvaAlvo.Objetivo
                    },
                    professorDto.Id
                );
            }

            atletas.Add(new DemoSeedReportAtleta(scenario.Nome, scenario.Email, scenario.InsightEsperado));
        }

        return new DemoSeedReport(profile.Profile, profile.ProfessorEmail, profile.ProfessorSenha, atletas);
    }
}
