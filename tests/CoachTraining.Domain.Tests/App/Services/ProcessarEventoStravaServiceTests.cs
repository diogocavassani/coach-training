using CoachTraining.App.Abstractions.Integrations;
using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.App.Abstractions.Security;
using CoachTraining.App.Services.Integrations;
using CoachTraining.Domain.Entities;
using CoachTraining.Domain.Enums;
using CoachTraining.Domain.ValueObjects;

namespace CoachTraining.Tests.App.Services;

public class ProcessarEventoStravaServiceTests
{
    private sealed class EventoWebhookRepositoryFake : IEventoWebhookRepository
    {
        private readonly Dictionary<Guid, EventoWebhookRecebido> _eventos = new();

        public Guid Adicionar(EventoWebhookRecebido evento)
        {
            _eventos[evento.Id] = evento;
            return evento.Id;
        }

        public EventoWebhookRecebido? ObterPorId(Guid id)
            => _eventos.TryGetValue(id, out var evento) ? evento : null;

        public void Atualizar(EventoWebhookRecebido evento)
        {
            _eventos[evento.Id] = evento;
        }
    }

    private sealed class AtividadeImportadaRepositoryFake : IAtividadeImportadaRepository
    {
        public List<AtividadeImportada> Itens { get; } = [];

        public void Adicionar(AtividadeImportada atividadeImportada)
        {
            Itens.Add(atividadeImportada);
        }

        public bool Existe(ProvedorIntegracao provedor, string externalActivityId)
            => Itens.Any(item => item.Provedor == provedor && item.ExternalActivityId == externalActivityId);
    }

    private sealed class ConexaoRepositoryFake : IConexaoWearableRepository
    {
        public List<ConexaoWearable> Itens { get; } = [];

        public IReadOnlyList<ConexaoWearable> ListarPorAtletaId(Guid atletaId)
            => Itens.Where(item => item.AtletaId == atletaId).ToList();

        public ConexaoWearable? ObterPorAtletaIdEProvedor(Guid atletaId, ProvedorIntegracao provedor)
            => Itens.FirstOrDefault(item => item.AtletaId == atletaId && item.Provedor == provedor);

        public ConexaoWearable? ObterPorExternalAthleteId(ProvedorIntegracao provedor, string externalAthleteId)
            => Itens.FirstOrDefault(item => item.Provedor == provedor && item.ExternalAthleteId == externalAthleteId);

        public void Salvar(ConexaoWearable conexao)
        {
            var index = Itens.FindIndex(item => item.Id == conexao.Id);
            if (index >= 0)
            {
                Itens[index] = conexao;
                return;
            }

            Itens.Add(conexao);
        }
    }

    private sealed class CredencialRepositoryFake : ICredencialWearableRepository
    {
        public List<CredencialWearable> Itens { get; } = [];

        public CredencialWearable? ObterPorConexaoWearableId(Guid conexaoWearableId)
            => Itens.FirstOrDefault(item => item.ConexaoWearableId == conexaoWearableId);

        public void Salvar(CredencialWearable credencial)
        {
            var index = Itens.FindIndex(item => item.ConexaoWearableId == credencial.ConexaoWearableId);
            if (index >= 0)
            {
                Itens[index] = credencial;
                return;
            }

            Itens.Add(credencial);
        }
    }

    private sealed class SessaoRepositoryFake : ISessaoDeTreinoRepository
    {
        public List<SessaoDeTreino> Itens { get; } = [];

        public void Adicionar(SessaoDeTreino sessao)
        {
            Itens.Add(sessao);
        }

        public IReadOnlyCollection<SessaoDeTreino> ObterPorAtletaId(Guid atletaId, Guid professorId)
            => Itens.Where(item => item.AtletaId == atletaId).ToList();
    }

    private sealed class SecretProtectorFake : ISecretProtector
    {
        public string Protect(string plaintext) => $"protected::{plaintext}";
        public string Unprotect(string protectedValue) => protectedValue.Replace("protected::", string.Empty, StringComparison.Ordinal);
    }

    private sealed class WearableProviderFake : IWearableProvider
    {
        public ProvedorIntegracao Provedor => ProvedorIntegracao.Strava;

        public string BuildAuthorizationUrl(string redirectUri, string state) => string.Empty;

        public Task<WearableTokenExchangeResult> ExchangeAuthorizationCodeAsync(string code, CancellationToken cancellationToken)
            => Task.FromResult(new WearableTokenExchangeResult("987", "access", "refresh", DateTime.UtcNow.AddHours(6), "activity:read"));

        public Task<WearableTokenExchangeResult> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken)
            => Task.FromResult(new WearableTokenExchangeResult("987", "access-novo", "refresh-novo", DateTime.UtcNow.AddHours(6), "activity:read"));

        public Task<WearableActivityDto> GetActivityAsync(string accessToken, string externalActivityId, CancellationToken cancellationToken)
            => Task.FromResult(new WearableActivityDto(
                ExternalActivityId: externalActivityId,
                ExternalAthleteId: "987",
                SportType: "Run",
                StartDateUtc: DateTime.UtcNow.AddDays(-1),
                DistanceMeters: 10000,
                MovingTimeSeconds: 3600,
                ElapsedTimeSeconds: 3700));
    }

    private sealed class WearableProviderRegistryFake : IWearableProviderRegistry
    {
        private readonly IWearableProvider _provider;

        public WearableProviderRegistryFake(IWearableProvider provider)
        {
            _provider = provider;
        }

        public IWearableProvider GetRequired(ProvedorIntegracao provedor) => _provider;
    }

    [Fact]
    public async Task ProcessarActivityCreate_DeveCriarSessaoImportadaERegistrarImportacao()
    {
        var eventoRepo = new EventoWebhookRepositoryFake();
        var atividadeRepo = new AtividadeImportadaRepositoryFake();
        var conexaoRepo = new ConexaoRepositoryFake();
        var credencialRepo = new CredencialRepositoryFake();
        var sessaoRepo = new SessaoRepositoryFake();
        var protector = new SecretProtectorFake();
        var conexao = new ConexaoWearable(
            atletaId: Guid.NewGuid(),
            provedor: ProvedorIntegracao.Strava,
            status: StatusConexaoIntegracao.Conectado,
            externalAthleteId: "987",
            scopesConcedidos: "activity:read",
            conectadoEmUtc: DateTime.UtcNow);
        conexaoRepo.Salvar(conexao);
        credencialRepo.Salvar(new CredencialWearable(conexao.Id, protector.Protect("access"), protector.Protect("refresh"), DateTime.UtcNow.AddHours(3), DateTime.UtcNow));
        var evento = new EventoWebhookRecebido(
            provedor: ProvedorIntegracao.Strava,
            objectType: "activity",
            objectId: "12345",
            ownerId: "987",
            aspectType: "create",
            payloadJson: "{\"object_type\":\"activity\",\"object_id\":12345,\"owner_id\":987,\"aspect_type\":\"create\"}",
            fingerprint: "activity:create:12345:987",
            recebidoEmUtc: DateTime.UtcNow);
        var eventoId = eventoRepo.Adicionar(evento);
        var service = new ProcessarEventoStravaService(
            eventoRepo,
            atividadeRepo,
            conexaoRepo,
            credencialRepo,
            sessaoRepo,
            new WearableProviderRegistryFake(new WearableProviderFake()),
            protector);

        await service.ProcessarAsync(eventoId, CancellationToken.None);

        Assert.Single(sessaoRepo.Itens);
        Assert.Equal(OrigemTreino.Strava, sessaoRepo.Itens[0].Origem);
        Assert.Equal(TipoDeTreino.Leve, sessaoRepo.Itens[0].Tipo);
        Assert.Equal(new RPE(5).Valor, sessaoRepo.Itens[0].Rpe.Valor);
        Assert.Single(atividadeRepo.Itens);
    }

    [Fact]
    public async Task ReprocessarMesmaAtividade_DeveSerIdempotente()
    {
        var eventoRepo = new EventoWebhookRepositoryFake();
        var atividadeRepo = new AtividadeImportadaRepositoryFake();
        var conexaoRepo = new ConexaoRepositoryFake();
        var credencialRepo = new CredencialRepositoryFake();
        var sessaoRepo = new SessaoRepositoryFake();
        var protector = new SecretProtectorFake();
        var conexao = new ConexaoWearable(
            atletaId: Guid.NewGuid(),
            provedor: ProvedorIntegracao.Strava,
            status: StatusConexaoIntegracao.Conectado,
            externalAthleteId: "987",
            scopesConcedidos: "activity:read",
            conectadoEmUtc: DateTime.UtcNow);
        conexaoRepo.Salvar(conexao);
        credencialRepo.Salvar(new CredencialWearable(conexao.Id, protector.Protect("access"), protector.Protect("refresh"), DateTime.UtcNow.AddHours(3), DateTime.UtcNow));
        var evento = new EventoWebhookRecebido(
            provedor: ProvedorIntegracao.Strava,
            objectType: "activity",
            objectId: "12345",
            ownerId: "987",
            aspectType: "create",
            payloadJson: "{\"object_type\":\"activity\",\"object_id\":12345,\"owner_id\":987,\"aspect_type\":\"create\"}",
            fingerprint: "activity:create:12345:987",
            recebidoEmUtc: DateTime.UtcNow);
        var eventoId = eventoRepo.Adicionar(evento);
        var service = new ProcessarEventoStravaService(
            eventoRepo,
            atividadeRepo,
            conexaoRepo,
            credencialRepo,
            sessaoRepo,
            new WearableProviderRegistryFake(new WearableProviderFake()),
            protector);

        await service.ProcessarAsync(eventoId, CancellationToken.None);
        await service.ProcessarAsync(eventoId, CancellationToken.None);

        Assert.Single(sessaoRepo.Itens);
        Assert.Single(atividadeRepo.Itens);
    }
}
