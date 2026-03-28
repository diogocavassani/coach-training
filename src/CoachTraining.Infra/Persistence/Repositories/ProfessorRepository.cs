using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.Domain.Entities;
using CoachTraining.Infra.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace CoachTraining.Infra.Persistence.Repositories;

public class ProfessorRepository : IProfessorRepository
{
    private readonly CoachTrainingDbContext _context;

    public ProfessorRepository(CoachTrainingDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void Adicionar(Professor professor)
    {
        var model = new ProfessorModel
        {
            Id = professor.Id,
            Nome = professor.Nome,
            Email = professor.Email,
            SenhaHash = professor.SenhaHash,
            DataCriacao = professor.DataCriacao
        };

        _context.Professores.Add(model);
        _context.SaveChanges();
    }

    public bool ExistePorEmail(string email)
    {
        var emailNormalizado = Professor.NormalizarEmail(email);
        return _context.Professores.Any(x => x.Email == emailNormalizado);
    }

    public Professor? ObterPorEmail(string email)
    {
        var emailNormalizado = Professor.NormalizarEmail(email);
        var model = _context.Professores
            .AsNoTracking()
            .FirstOrDefault(x => x.Email == emailNormalizado);

        if (model == null)
        {
            return null;
        }

        return new Professor(
            nome: model.Nome,
            email: model.Email,
            senhaHash: model.SenhaHash,
            id: model.Id,
            dataCriacao: model.DataCriacao);
    }
}
