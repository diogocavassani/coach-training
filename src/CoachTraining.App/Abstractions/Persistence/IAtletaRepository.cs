using CoachTraining.Domain.Entities;

namespace CoachTraining.App.Abstractions.Persistence;

public interface IAtletaRepository
{
    void Adicionar(Atleta atleta);
    Atleta? ObterPorId(Guid atletaId, Guid professorId);
    IReadOnlyList<Atleta> ListarPorProfessor(Guid professorId);
}
