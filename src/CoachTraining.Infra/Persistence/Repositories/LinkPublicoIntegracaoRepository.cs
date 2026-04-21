using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.Domain.Entities;
using CoachTraining.Infra.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace CoachTraining.Infra.Persistence.Repositories;

public class LinkPublicoIntegracaoRepository : ILinkPublicoIntegracaoRepository
{
    private readonly CoachTrainingDbContext _context;

    public LinkPublicoIntegracaoRepository(CoachTrainingDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public LinkPublicoIntegracaoData? ObterAtivoPorAtletaId(Guid atletaId)
    {
        var model = _context.LinksPublicosIntegracao
            .AsNoTracking()
            .FirstOrDefault(link => link.AtletaId == atletaId && link.Ativo);

        return model == null ? null : MapearData(model);
    }

    public LinkPublicoIntegracao? ObterPorTokenHash(string tokenHash)
    {
        var model = _context.LinksPublicosIntegracao
            .AsNoTracking()
            .FirstOrDefault(link => link.TokenHash == tokenHash && link.Ativo);

        return model == null ? null : Mapear(model);
    }

    public void Salvar(LinkPublicoIntegracao link, string tokenProtegido)
    {
        var existente = _context.LinksPublicosIntegracao.FirstOrDefault(model => model.Id == link.Id);
        if (existente == null)
        {
            _context.LinksPublicosIntegracao.Add(new LinkPublicoIntegracaoModel
            {
                Id = link.Id,
                AtletaId = link.AtletaId,
                TokenHash = link.TokenHash,
                TokenProtegido = tokenProtegido,
                Ativo = link.Ativo,
                CriadoEmUtc = link.CriadoEmUtc,
                RegeneradoEmUtc = link.RegeneradoEmUtc,
                RevogadoEmUtc = link.RevogadoEmUtc
            });
        }
        else
        {
            existente.TokenHash = link.TokenHash;
            existente.TokenProtegido = tokenProtegido;
            existente.Ativo = link.Ativo;
            existente.RegeneradoEmUtc = link.RegeneradoEmUtc;
            existente.RevogadoEmUtc = link.RevogadoEmUtc;
        }

        _context.SaveChanges();
    }

    private static LinkPublicoIntegracaoData MapearData(LinkPublicoIntegracaoModel model)
        => new(Mapear(model), model.TokenProtegido);

    private static LinkPublicoIntegracao Mapear(LinkPublicoIntegracaoModel model)
        => LinkPublicoIntegracao.Restaurar(
            id: model.Id,
            atletaId: model.AtletaId,
            tokenHash: model.TokenHash,
            ativo: model.Ativo,
            criadoEmUtc: model.CriadoEmUtc,
            regeneradoEmUtc: model.RegeneradoEmUtc,
            revogadoEmUtc: model.RevogadoEmUtc);
}
