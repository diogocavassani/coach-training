using CoachTraining.Domain.Entities;

namespace CoachTraining.App.Abstractions.Persistence;

public interface ICredencialWearableRepository
{
    CredencialWearable? ObterPorConexaoWearableId(Guid conexaoWearableId);
    void Salvar(CredencialWearable credencial);
}
