using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.Domain.Entities;
using CoachTraining.Domain.Enums;

namespace CoachTraining.Infra.Persistence.Repositories;

public class AtividadeImportadaRepository : IAtividadeImportadaRepository
{
    private readonly CoachTrainingDbContext _context;

    public AtividadeImportadaRepository(CoachTrainingDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void Adicionar(AtividadeImportada atividadeImportada)
    {
        _context.AtividadesImportadas.Add(new Persistence.Models.AtividadeImportadaModel
        {
            Id = atividadeImportada.Id,
            Provedor = (int)atividadeImportada.Provedor,
            ConexaoWearableId = atividadeImportada.ConexaoWearableId,
            ExternalActivityId = atividadeImportada.ExternalActivityId,
            SessaoDeTreinoId = atividadeImportada.SessaoDeTreinoId,
            ImportadoEmUtc = atividadeImportada.ImportadoEmUtc,
            PayloadResumoJson = atividadeImportada.PayloadResumoJson
        });
        _context.SaveChanges();
    }

    public bool Existe(ProvedorIntegracao provedor, string externalActivityId)
    {
        return _context.AtividadesImportadas.Any(item => item.Provedor == (int)provedor && item.ExternalActivityId == externalActivityId);
    }
}
