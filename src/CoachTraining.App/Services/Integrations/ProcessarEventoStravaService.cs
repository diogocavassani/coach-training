using CoachTraining.App.Abstractions.Integrations;
using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.App.Abstractions.Security;
using CoachTraining.Domain.Entities;
using CoachTraining.Domain.Enums;
using CoachTraining.Domain.ValueObjects;

namespace CoachTraining.App.Services.Integrations;

public class ProcessarEventoStravaService
{
    private readonly IEventoWebhookRepository _eventoRepository;
    private readonly IAtividadeImportadaRepository _atividadeImportadaRepository;
    private readonly IConexaoWearableRepository _conexaoRepository;
    private readonly ICredencialWearableRepository _credencialRepository;
    private readonly ISessaoDeTreinoRepository _sessaoRepository;
    private readonly IWearableProviderRegistry _providerRegistry;
    private readonly ISecretProtector _secretProtector;

    public ProcessarEventoStravaService(
        IEventoWebhookRepository eventoRepository,
        IAtividadeImportadaRepository atividadeImportadaRepository,
        IConexaoWearableRepository conexaoRepository,
        ICredencialWearableRepository credencialRepository,
        ISessaoDeTreinoRepository sessaoRepository,
        IWearableProviderRegistry providerRegistry,
        ISecretProtector secretProtector)
    {
        _eventoRepository = eventoRepository ?? throw new ArgumentNullException(nameof(eventoRepository));
        _atividadeImportadaRepository = atividadeImportadaRepository ?? throw new ArgumentNullException(nameof(atividadeImportadaRepository));
        _conexaoRepository = conexaoRepository ?? throw new ArgumentNullException(nameof(conexaoRepository));
        _credencialRepository = credencialRepository ?? throw new ArgumentNullException(nameof(credencialRepository));
        _sessaoRepository = sessaoRepository ?? throw new ArgumentNullException(nameof(sessaoRepository));
        _providerRegistry = providerRegistry ?? throw new ArgumentNullException(nameof(providerRegistry));
        _secretProtector = secretProtector ?? throw new ArgumentNullException(nameof(secretProtector));
    }

    public async Task ProcessarAsync(Guid eventoId, CancellationToken cancellationToken)
    {
        var evento = _eventoRepository.ObterPorId(eventoId)
            ?? throw new InvalidOperationException("Evento webhook nao encontrado.");

        if (!string.Equals(evento.ObjectType, "activity", StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(evento.AspectType, "create", StringComparison.OrdinalIgnoreCase))
        {
            evento.MarcarComoIgnorado(DateTime.UtcNow, "evento nao suportado");
            _eventoRepository.Atualizar(evento);
            return;
        }

        if (_atividadeImportadaRepository.Existe(ProvedorIntegracao.Strava, evento.ObjectId))
        {
            evento.MarcarComoIgnorado(DateTime.UtcNow, "atividade ja importada");
            _eventoRepository.Atualizar(evento);
            return;
        }

        var conexao = _conexaoRepository.ObterPorExternalAthleteId(ProvedorIntegracao.Strava, evento.OwnerId);
        if (conexao == null)
        {
            evento.MarcarComoIgnorado(DateTime.UtcNow, "conexao nao encontrada");
            _eventoRepository.Atualizar(evento);
            return;
        }

        var credencial = _credencialRepository.ObterPorConexaoWearableId(conexao.Id);
        if (credencial == null)
        {
            evento.MarcarComoFalho(DateTime.UtcNow, "credencial nao encontrada");
            _eventoRepository.Atualizar(evento);
            return;
        }

        try
        {
            var provider = _providerRegistry.GetRequired(ProvedorIntegracao.Strava);
            var accessToken = await ObterAccessTokenValidoAsync(provider, credencial, cancellationToken);
            var atividade = await provider.GetActivityAsync(accessToken, evento.ObjectId, cancellationToken);

            var sessao = new SessaoDeTreino(
                atletaId: conexao.AtletaId,
                data: DateOnly.FromDateTime(atividade.StartDateUtc),
                tipo: MapearTipoDeTreino(atividade.SportType),
                duracaoMinutos: Math.Max(1, atividade.MovingTimeSeconds > 0 ? atividade.MovingTimeSeconds / 60 : atividade.ElapsedTimeSeconds / 60),
                distanciaKm: atividade.DistanceMeters / 1000d,
                rpe: new RPE(5),
                origem: OrigemTreino.Strava);

            _sessaoRepository.Adicionar(sessao);
            _atividadeImportadaRepository.Adicionar(new AtividadeImportada(
                provedor: ProvedorIntegracao.Strava,
                conexaoWearableId: conexao.Id,
                externalActivityId: atividade.ExternalActivityId,
                sessaoDeTreinoId: sessao.Id,
                importadoEmUtc: DateTime.UtcNow));

            conexao.AtualizarUltimaSincronizacao(DateTime.UtcNow);
            _conexaoRepository.Salvar(conexao);

            evento.MarcarComoProcessado(DateTime.UtcNow);
            _eventoRepository.Atualizar(evento);
        }
        catch (Exception ex)
        {
            evento.MarcarComoFalho(DateTime.UtcNow, ex.Message);
            _eventoRepository.Atualizar(evento);
            throw;
        }
    }

    private async Task<string> ObterAccessTokenValidoAsync(
        IWearableProvider provider,
        CredencialWearable credencial,
        CancellationToken cancellationToken)
    {
        if (credencial.ExpiresAtUtc > DateTime.UtcNow.AddMinutes(1))
        {
            return _secretProtector.Unprotect(credencial.AccessTokenProtegido);
        }

        var refreshToken = _secretProtector.Unprotect(credencial.RefreshTokenProtegido);
        var resultado = await provider.RefreshAccessTokenAsync(refreshToken, cancellationToken);
        var credencialAtualizada = new CredencialWearable(
            conexaoWearableId: credencial.ConexaoWearableId,
            accessTokenProtegido: _secretProtector.Protect(resultado.AccessToken),
            refreshTokenProtegido: _secretProtector.Protect(resultado.RefreshToken),
            expiresAtUtc: resultado.ExpiresAtUtc,
            atualizadoEmUtc: DateTime.UtcNow,
            id: credencial.Id);
        _credencialRepository.Salvar(credencialAtualizada);

        return resultado.AccessToken;
    }

    private static TipoDeTreino MapearTipoDeTreino(string sportType)
    {
        return sportType switch
        {
            "TrailRun" => TipoDeTreino.Longo,
            "Workout" => TipoDeTreino.Ritmo,
            "Run" => TipoDeTreino.Leve,
            "Ride" => TipoDeTreino.Leve,
            "VirtualRide" => TipoDeTreino.Leve,
            _ => TipoDeTreino.Leve
        };
    }
}
