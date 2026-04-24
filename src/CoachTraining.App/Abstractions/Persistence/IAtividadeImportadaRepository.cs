using CoachTraining.Domain.Entities;
using CoachTraining.Domain.Enums;

namespace CoachTraining.App.Abstractions.Persistence;

public interface IAtividadeImportadaRepository
{
    void Adicionar(AtividadeImportada atividadeImportada);
    bool Existe(ProvedorIntegracao provedor, string externalActivityId);
}
