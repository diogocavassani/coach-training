using CoachTraining.Domain.Entities;
using CoachTraining.Domain.Enums;

namespace CoachTraining.App.Abstractions.Persistence;

public interface IConexaoWearableRepository
{
    IReadOnlyList<ConexaoWearable> ListarPorAtletaId(Guid atletaId);
    ConexaoWearable? ObterPorAtletaIdEProvedor(Guid atletaId, ProvedorIntegracao provedor);
    ConexaoWearable? ObterPorExternalAthleteId(ProvedorIntegracao provedor, string externalAthleteId);
    void Salvar(ConexaoWearable conexao);
}
