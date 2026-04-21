using CoachTraining.Domain.Enums;

namespace CoachTraining.Domain.Entities;

public class ConexaoWearable
{
    public ConexaoWearable(
        Guid atletaId,
        ProvedorIntegracao provedor,
        StatusConexaoIntegracao status,
        string externalAthleteId,
        string scopesConcedidos,
        DateTime conectadoEmUtc,
        Guid? id = null)
    {
        if (atletaId == Guid.Empty)
        {
            throw new ArgumentException("AtletaId obrigatorio.", nameof(atletaId));
        }

        if (string.IsNullOrWhiteSpace(externalAthleteId))
        {
            throw new ArgumentException("ExternalAthleteId obrigatorio.", nameof(externalAthleteId));
        }

        Id = id ?? Guid.NewGuid();
        AtletaId = atletaId;
        Provedor = provedor;
        Status = status;
        ExternalAthleteId = externalAthleteId.Trim();
        ScopesConcedidos = scopesConcedidos?.Trim() ?? string.Empty;
        ConectadoEmUtc = conectadoEmUtc;
    }

    public Guid Id { get; private set; }
    public Guid AtletaId { get; private set; }
    public ProvedorIntegracao Provedor { get; private set; }
    public StatusConexaoIntegracao Status { get; private set; }
    public string ExternalAthleteId { get; private set; }
    public string ScopesConcedidos { get; private set; }
    public DateTime ConectadoEmUtc { get; private set; }
    public DateTime? DesconectadoEmUtc { get; private set; }
    public DateTime? UltimaSincronizacaoEmUtc { get; private set; }
    public string? UltimoErro { get; private set; }

    public void MarcarComoDesconectado(DateTime quando)
    {
        Status = StatusConexaoIntegracao.Desconectado;
        DesconectadoEmUtc = quando;
    }

    public void AtualizarUltimaSincronizacao(DateTime quando)
    {
        UltimaSincronizacaoEmUtc = quando;
    }

    public void MarcarComoErroAutorizacao(string? ultimoErro)
    {
        Status = StatusConexaoIntegracao.ErroAutorizacao;
        UltimoErro = ultimoErro?.Trim();
    }

    public void MarcarComoRequerReconexao(string? ultimoErro)
    {
        Status = StatusConexaoIntegracao.RequerReconexao;
        UltimoErro = ultimoErro?.Trim();
    }
}
