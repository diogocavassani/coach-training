using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.Domain.Entities;
using CoachTraining.Domain.Enums;
using CoachTraining.Domain.ValueObjects;
using CoachTraining.Infra.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace CoachTraining.Infra.Persistence.Repositories;

public class SessaoDeTreinoRepository : ISessaoDeTreinoRepository
{
    private readonly CoachTrainingDbContext _context;

    public SessaoDeTreinoRepository(CoachTrainingDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void Adicionar(SessaoDeTreino sessao)
    {
        var model = new SessaoDeTreinoModel
        {
            Id = sessao.Id,
            AtletaId = sessao.AtletaId,
            Data = sessao.Data,
            Tipo = (int)sessao.Tipo,
            DuracaoMinutos = sessao.DuracaoMinutos,
            DistanciaKm = sessao.DistanciaKm,
            Rpe = sessao.Rpe.Valor
        };

        _context.SessoesDeTreino.Add(model);
        _context.SaveChanges();
    }

    public IReadOnlyCollection<SessaoDeTreino> ObterPorAtletaId(Guid atletaId, Guid professorId)
    {
        var query = from sessao in _context.SessoesDeTreino.AsNoTracking()
                    join atleta in _context.Atletas.AsNoTracking() on sessao.AtletaId equals atleta.Id
                    where sessao.AtletaId == atletaId && atleta.ProfessorId == professorId
                    select sessao;

        return query
            .OrderBy(s => s.Data)
            .AsEnumerable()
            .Select(s => new SessaoDeTreino(
                atletaId: s.AtletaId,
                data: s.Data,
                tipo: ObterTipoDeTreinoValido(s.Tipo, s.Id),
                duracaoMinutos: s.DuracaoMinutos,
                distanciaKm: s.DistanciaKm,
                rpe: new RPE(s.Rpe),
                id: s.Id))
            .ToList();
    }

    private static TipoDeTreino ObterTipoDeTreinoValido(int tipoPersistido, Guid sessaoId)
    {
        if (!Enum.IsDefined(typeof(TipoDeTreino), tipoPersistido))
        {
            throw new InvalidOperationException(
                $"Erro de integridade: tipo de treino invalido '{tipoPersistido}' na sessao '{sessaoId}'.");
        }

        return (TipoDeTreino)tipoPersistido;
    }
}
