using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoachTraining.Infra.Persistence.Repositories;

public class CredencialWearableRepository : ICredencialWearableRepository
{
    private readonly CoachTrainingDbContext _context;

    public CredencialWearableRepository(CoachTrainingDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public CredencialWearable? ObterPorConexaoWearableId(Guid conexaoWearableId)
    {
        var model = _context.CredenciaisWearable
            .AsNoTracking()
            .FirstOrDefault(item => item.ConexaoWearableId == conexaoWearableId);

        return model == null
            ? null
            : new CredencialWearable(
                conexaoWearableId: model.ConexaoWearableId,
                accessTokenProtegido: model.AccessTokenProtegido,
                refreshTokenProtegido: model.RefreshTokenProtegido,
                expiresAtUtc: model.ExpiresAtUtc,
                atualizadoEmUtc: model.AtualizadoEmUtc,
                id: model.Id);
    }

    public void Salvar(CredencialWearable credencial)
    {
        var existente = _context.CredenciaisWearable.FirstOrDefault(item => item.ConexaoWearableId == credencial.ConexaoWearableId);
        if (existente == null)
        {
            _context.CredenciaisWearable.Add(new Persistence.Models.CredencialWearableModel
            {
                Id = credencial.Id,
                ConexaoWearableId = credencial.ConexaoWearableId,
                AccessTokenProtegido = credencial.AccessTokenProtegido,
                RefreshTokenProtegido = credencial.RefreshTokenProtegido,
                ExpiresAtUtc = credencial.ExpiresAtUtc,
                AtualizadoEmUtc = credencial.AtualizadoEmUtc
            });
        }
        else
        {
            existente.AccessTokenProtegido = credencial.AccessTokenProtegido;
            existente.RefreshTokenProtegido = credencial.RefreshTokenProtegido;
            existente.ExpiresAtUtc = credencial.ExpiresAtUtc;
            existente.AtualizadoEmUtc = credencial.AtualizadoEmUtc;
        }

        _context.SaveChanges();
    }
}
