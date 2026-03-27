using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.Domain.Entities;
using CoachTraining.Domain.Enums;
using CoachTraining.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CoachTraining.Infra.Persistence.Repositories;

public class SessaoDeTreinoRepository : ISessaoDeTreinoRepository
{
    private readonly CoachTrainingDbContext _context;

    public SessaoDeTreinoRepository(CoachTrainingDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IReadOnlyCollection<SessaoDeTreino> ObterPorAtletaId(Guid atletaId)
    {
        return _context.SessoesDeTreino
            .AsNoTracking()
            .Where(s => s.AtletaId == atletaId)
            .OrderBy(s => s.Data)
            .Select(s => new SessaoDeTreino(
                data: s.Data,
                tipo: (TipoDeTreino)s.Tipo,
                duracaoMinutos: s.DuracaoMinutos,
                distanciaKm: s.DistanciaKm,
                rpe: new RPE(s.Rpe),
                id: s.Id))
            .ToList();
    }
}
