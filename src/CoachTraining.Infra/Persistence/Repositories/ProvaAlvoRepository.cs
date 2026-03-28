using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoachTraining.Infra.Persistence.Repositories;

public class ProvaAlvoRepository : IProvaAlvoRepository
{
    private readonly CoachTrainingDbContext _context;

    public ProvaAlvoRepository(CoachTrainingDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public ProvaAlvo? ObterPorAtletaId(Guid atletaId, Guid professorId)
    {
        var model = (
            from prova in _context.ProvasAlvo.AsNoTracking()
            join atleta in _context.Atletas.AsNoTracking() on prova.AtletaId equals atleta.Id
            where prova.AtletaId == atletaId && atleta.ProfessorId == professorId
            select prova)
            .FirstOrDefault();

        if (model == null)
        {
            return null;
        }

        return new ProvaAlvo(
            dataProva: model.DataProva,
            distanciaKm: model.DistanciaKm,
            objetivo: model.Objetivo,
            id: model.Id);
    }
}
