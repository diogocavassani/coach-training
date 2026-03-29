using CoachTraining.Domain.Entities;

namespace CoachTraining.App.Abstractions.Persistence;

public interface ISessaoDeTreinoRepository
{
    void Adicionar(SessaoDeTreino sessao);
    IReadOnlyCollection<SessaoDeTreino> ObterPorAtletaId(Guid atletaId, Guid professorId);
}
