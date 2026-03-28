using CoachTraining.Domain.Entities;

namespace CoachTraining.App.Abstractions.Persistence;

public interface IProvaAlvoRepository
{
    ProvaAlvo? ObterPorAtletaId(Guid atletaId, Guid professorId);
}
