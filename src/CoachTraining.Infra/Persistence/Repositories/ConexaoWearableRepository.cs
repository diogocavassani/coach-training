using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.Domain.Entities;
using CoachTraining.Domain.Enums;
using CoachTraining.Infra.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace CoachTraining.Infra.Persistence.Repositories;

public class ConexaoWearableRepository : IConexaoWearableRepository
{
    private readonly CoachTrainingDbContext _context;

    public ConexaoWearableRepository(CoachTrainingDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IReadOnlyList<ConexaoWearable> ListarPorAtletaId(Guid atletaId)
    {
        return _context.ConexoesWearable
            .AsNoTracking()
            .Where(conexao => conexao.AtletaId == atletaId)
            .Select(Mapear)
            .ToList();
    }

    public ConexaoWearable? ObterPorAtletaIdEProvedor(Guid atletaId, ProvedorIntegracao provedor)
    {
        var model = _context.ConexoesWearable
            .AsNoTracking()
            .FirstOrDefault(conexao => conexao.AtletaId == atletaId && conexao.Provedor == (int)provedor);

        return model == null ? null : Mapear(model);
    }

    public ConexaoWearable? ObterPorExternalAthleteId(ProvedorIntegracao provedor, string externalAthleteId)
    {
        var model = _context.ConexoesWearable
            .AsNoTracking()
            .FirstOrDefault(conexao => conexao.Provedor == (int)provedor && conexao.ExternalAthleteId == externalAthleteId);

        return model == null ? null : Mapear(model);
    }

    public void Salvar(ConexaoWearable conexao)
    {
        var existente = _context.ConexoesWearable.FirstOrDefault(model => model.Id == conexao.Id);
        if (existente == null)
        {
            _context.ConexoesWearable.Add(MapearModel(conexao));
        }
        else
        {
            existente.Status = (int)conexao.Status;
            existente.ExternalAthleteId = conexao.ExternalAthleteId;
            existente.ScopesConcedidos = conexao.ScopesConcedidos;
            existente.ConectadoEmUtc = conexao.ConectadoEmUtc;
            existente.DesconectadoEmUtc = conexao.DesconectadoEmUtc;
            existente.UltimaSincronizacaoEmUtc = conexao.UltimaSincronizacaoEmUtc;
            existente.UltimoErro = conexao.UltimoErro;
        }

        _context.SaveChanges();
    }

    private static ConexaoWearable Mapear(ConexaoWearableModel model)
        => new(
            atletaId: model.AtletaId,
            provedor: (ProvedorIntegracao)model.Provedor,
            status: (StatusConexaoIntegracao)model.Status,
            externalAthleteId: model.ExternalAthleteId,
            scopesConcedidos: model.ScopesConcedidos,
            conectadoEmUtc: model.ConectadoEmUtc,
            desconectadoEmUtc: model.DesconectadoEmUtc,
            ultimaSincronizacaoEmUtc: model.UltimaSincronizacaoEmUtc,
            ultimoErro: model.UltimoErro,
            id: model.Id);

    private static ConexaoWearableModel MapearModel(ConexaoWearable conexao)
        => new()
        {
            Id = conexao.Id,
            AtletaId = conexao.AtletaId,
            Provedor = (int)conexao.Provedor,
            Status = (int)conexao.Status,
            ExternalAthleteId = conexao.ExternalAthleteId,
            ScopesConcedidos = conexao.ScopesConcedidos,
            ConectadoEmUtc = conexao.ConectadoEmUtc,
            DesconectadoEmUtc = conexao.DesconectadoEmUtc,
            UltimaSincronizacaoEmUtc = conexao.UltimaSincronizacaoEmUtc,
            UltimoErro = conexao.UltimoErro
        };
}
