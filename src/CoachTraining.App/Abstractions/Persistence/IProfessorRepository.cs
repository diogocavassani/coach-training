using CoachTraining.Domain.Entities;

namespace CoachTraining.App.Abstractions.Persistence;

public interface IProfessorRepository
{
    void Adicionar(Professor professor);
    bool ExistePorEmail(string email);
    Professor? ObterPorEmail(string email);
}
