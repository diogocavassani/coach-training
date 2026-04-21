using CoachTraining.App.DTOs.Integrations;
using CoachTraining.Domain.Entities;
using CoachTraining.Domain.Enums;

namespace CoachTraining.App.Services.Integrations;

internal static class IntegracaoProviderCatalog
{
    public static IReadOnlyList<ProviderIntegrationStatusDto> CriarStatus(IReadOnlyList<ConexaoWearable> conexoes)
    {
        var conexaoStrava = conexoes.FirstOrDefault(conexao => conexao.Provedor == ProvedorIntegracao.Strava);

        return
        [
            new ProviderIntegrationStatusDto
            {
                ProviderKey = "strava",
                DisplayName = "Strava",
                Enabled = true,
                Status = MapearStatus(conexaoStrava?.Status),
                LastSyncAtUtc = conexaoStrava?.UltimaSincronizacaoEmUtc
            }
        ];
    }

    private static string MapearStatus(StatusConexaoIntegracao? status)
    {
        return status switch
        {
            StatusConexaoIntegracao.Conectado => "connected",
            StatusConexaoIntegracao.ErroAutorizacao => "authorization_error",
            StatusConexaoIntegracao.RequerReconexao => "reconnect_required",
            StatusConexaoIntegracao.Desconectado => "disconnected",
            _ => "not_connected"
        };
    }
}
