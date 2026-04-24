using CoachTraining.App.Abstractions.Persistence;
using CoachTraining.Domain.Entities;
using CoachTraining.Domain.Enums;
using CoachTraining.Infra.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace CoachTraining.Infra.Persistence.Repositories;

public class EventoWebhookRecebidoRepository : IEventoWebhookRepository
{
    private readonly CoachTrainingDbContext _context;

    public EventoWebhookRecebidoRepository(CoachTrainingDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Guid Adicionar(EventoWebhookRecebido evento)
    {
        _context.EventosWebhookRecebidos.Add(new EventoWebhookRecebidoModel
        {
            Id = evento.Id,
            Provedor = (int)evento.Provedor,
            ObjectType = evento.ObjectType,
            ObjectId = evento.ObjectId,
            OwnerId = evento.OwnerId,
            AspectType = evento.AspectType,
            PayloadJson = evento.PayloadJson,
            Fingerprint = evento.Fingerprint,
            RecebidoEmUtc = evento.RecebidoEmUtc,
            ProcessadoEmUtc = evento.ProcessadoEmUtc,
            StatusProcessamento = evento.StatusProcessamento,
            ErroProcessamento = evento.ErroProcessamento
        });
        _context.SaveChanges();
        return evento.Id;
    }

    public EventoWebhookRecebido? ObterPorId(Guid id)
    {
        var model = _context.EventosWebhookRecebidos.AsNoTracking().FirstOrDefault(item => item.Id == id);
        if (model == null)
        {
            return null;
        }

        var evento = new EventoWebhookRecebido(
            provedor: (ProvedorIntegracao)model.Provedor,
            objectType: model.ObjectType,
            objectId: model.ObjectId,
            ownerId: model.OwnerId,
            aspectType: model.AspectType,
            payloadJson: model.PayloadJson,
            fingerprint: model.Fingerprint,
            recebidoEmUtc: model.RecebidoEmUtc,
            id: model.Id);

        if (string.Equals(model.StatusProcessamento, "Processado", StringComparison.Ordinal))
        {
            evento.MarcarComoProcessado(model.ProcessadoEmUtc ?? model.RecebidoEmUtc);
        }
        else if (string.Equals(model.StatusProcessamento, "Ignorado", StringComparison.Ordinal))
        {
            evento.MarcarComoIgnorado(model.ProcessadoEmUtc ?? model.RecebidoEmUtc, model.ErroProcessamento ?? string.Empty);
        }
        else if (string.Equals(model.StatusProcessamento, "Falhou", StringComparison.Ordinal))
        {
            evento.MarcarComoFalho(model.ProcessadoEmUtc ?? model.RecebidoEmUtc, model.ErroProcessamento ?? string.Empty);
        }

        return evento;
    }

    public void Atualizar(EventoWebhookRecebido evento)
    {
        var model = _context.EventosWebhookRecebidos.First(item => item.Id == evento.Id);
        model.ProcessadoEmUtc = evento.ProcessadoEmUtc;
        model.StatusProcessamento = evento.StatusProcessamento;
        model.ErroProcessamento = evento.ErroProcessamento;
        _context.SaveChanges();
    }
}
