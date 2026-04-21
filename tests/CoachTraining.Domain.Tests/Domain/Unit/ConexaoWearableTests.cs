using CoachTraining.Domain.Entities;
using CoachTraining.Domain.Enums;

namespace CoachTraining.Tests.Domain.Unit;

public class ConexaoWearableTests
{
    [Fact]
    public void Criar_DeveRegistrarStatusInicialConectado()
    {
        var atletaId = Guid.NewGuid();
        var conectadoEmUtc = DateTime.UtcNow;

        var conexao = new ConexaoWearable(
            atletaId: atletaId,
            provedor: ProvedorIntegracao.Strava,
            status: StatusConexaoIntegracao.Conectado,
            externalAthleteId: "123",
            scopesConcedidos: "activity:read",
            conectadoEmUtc: conectadoEmUtc);

        Assert.Equal(atletaId, conexao.AtletaId);
        Assert.Equal(ProvedorIntegracao.Strava, conexao.Provedor);
        Assert.Equal(StatusConexaoIntegracao.Conectado, conexao.Status);
        Assert.Equal("123", conexao.ExternalAthleteId);
        Assert.Equal("activity:read", conexao.ScopesConcedidos);
        Assert.Equal(conectadoEmUtc, conexao.ConectadoEmUtc);
    }

    [Fact]
    public void MarcarComoDesconectado_DeveAtualizarStatusEDesconectadoEm()
    {
        var conexao = new ConexaoWearable(
            atletaId: Guid.NewGuid(),
            provedor: ProvedorIntegracao.Strava,
            status: StatusConexaoIntegracao.Conectado,
            externalAthleteId: "123",
            scopesConcedidos: "activity:read",
            conectadoEmUtc: DateTime.UtcNow);
        var quando = DateTime.UtcNow.AddMinutes(5);

        conexao.MarcarComoDesconectado(quando);

        Assert.Equal(StatusConexaoIntegracao.Desconectado, conexao.Status);
        Assert.Equal(quando, conexao.DesconectadoEmUtc);
    }

    [Fact]
    public void AtualizarUltimaSincronizacao_DevePersistirTimestamp()
    {
        var conexao = new ConexaoWearable(
            atletaId: Guid.NewGuid(),
            provedor: ProvedorIntegracao.Strava,
            status: StatusConexaoIntegracao.Conectado,
            externalAthleteId: "123",
            scopesConcedidos: "activity:read",
            conectadoEmUtc: DateTime.UtcNow);
        var quando = DateTime.UtcNow.AddMinutes(10);

        conexao.AtualizarUltimaSincronizacao(quando);

        Assert.Equal(quando, conexao.UltimaSincronizacaoEmUtc);
    }
}
