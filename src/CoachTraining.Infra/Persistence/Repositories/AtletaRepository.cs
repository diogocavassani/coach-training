using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.Domain.Entities;
using CoachTraining.Infra.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace CoachTraining.Infra.Persistence.Repositories;

public class AtletaRepository : IAtletaRepository
{
    private readonly CoachTrainingDbContext _context;

    public AtletaRepository(CoachTrainingDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void Adicionar(Atleta atleta)
    {
        var model = new AtletaModel
        {
            Id = atleta.Id,
            ProfessorId = atleta.ProfessorId,
            Nome = atleta.Nome,
            Email = atleta.Email,
            ObservacoesClinicas = atleta.ObservacoesClinicas,
            NivelEsportivo = atleta.NivelEsportivo
        };

        _context.Atletas.Add(model);
        _context.SaveChanges();
    }

    public Atleta? ObterPorId(Guid atletaId, Guid professorId)
    {
        var model = _context.Atletas
            .AsNoTracking()
            .FirstOrDefault(a => a.Id == atletaId && a.ProfessorId == professorId);

        if (model == null)
        {
            return null;
        }

        return new Atleta(
            nome: model.Nome,
            professorId: model.ProfessorId,
            observacoesClinicas: model.ObservacoesClinicas,
            nivelEsportivo: model.NivelEsportivo,
            email: model.Email,
            id: model.Id);
    }

    public IReadOnlyList<Atleta> ListarPorProfessor(Guid professorId)
    {
        return _context.Atletas
            .AsNoTracking()
            .Where(a => a.ProfessorId == professorId)
            .OrderBy(a => a.Nome)
            .Select(model => new Atleta(
                nome: model.Nome,
                professorId: model.ProfessorId,
                observacoesClinicas: model.ObservacoesClinicas,
                nivelEsportivo: model.NivelEsportivo,
                email: model.Email,
                id: model.Id))
            .ToList();
    }
}
