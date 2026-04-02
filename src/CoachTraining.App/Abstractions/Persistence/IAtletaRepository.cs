using CoachTraining.Domain.Entities;

namespace CoachTraining.App.Abstractions.Persistence;

public interface IAtletaRepository
{
    void Adicionar(Atleta atleta);
    void AtualizarPlanejamentoBase(Guid atletaId, Guid professorId, int treinosPlanejadosPorSemana);
    Atleta? ObterPorId(Guid atletaId, Guid professorId);
    IReadOnlyList<Atleta> ListarPorProfessor(Guid professorId);
}
