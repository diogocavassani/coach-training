using CoachTraining.Domain.Enums;

namespace CoachTraining.App.Abstractions.Integrations;

public interface IWearableProviderRegistry
{
    IWearableProvider GetRequired(ProvedorIntegracao provedor);
}
