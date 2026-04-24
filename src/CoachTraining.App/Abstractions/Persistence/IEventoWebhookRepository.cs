using CoachTraining.Domain.Entities;

namespace CoachTraining.App.Abstractions.Persistence;

public interface IEventoWebhookRepository
{
    Guid Adicionar(EventoWebhookRecebido evento);
    EventoWebhookRecebido? ObterPorId(Guid id);
    void Atualizar(EventoWebhookRecebido evento);
}
