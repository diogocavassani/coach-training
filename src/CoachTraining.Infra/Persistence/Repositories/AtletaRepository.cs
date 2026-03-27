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
            Nome = atleta.Nome,
            ObservacoesClinicas = atleta.ObservacoesClinicas,
            NivelEsportivo = atleta.NivelEsportivo
        };

        _context.Atletas.Add(model);
        _context.SaveChanges();
    }

    public Atleta? ObterPorId(Guid atletaId)
    {
        var model = _context.Atletas
            .AsNoTracking()
            .FirstOrDefault(a => a.Id == atletaId);

        if (model == null)
        {
            return null;
        }

        return new Atleta(
            nome: model.Nome,
            professorId: Guid.NewGuid(),
            observacoesClinicas: model.ObservacoesClinicas,
            nivelEsportivo: model.NivelEsportivo,
            id: model.Id);
    }
}
