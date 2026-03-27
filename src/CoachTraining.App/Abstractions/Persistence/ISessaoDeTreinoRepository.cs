using CoachTraining.Domain.Entities;

namespace CoachTraining.App.Abstractions.Persistence;

public interface ISessaoDeTreinoRepository
{
    IReadOnlyCollection<SessaoDeTreino> ObterPorAtletaId(Guid atletaId);
}
