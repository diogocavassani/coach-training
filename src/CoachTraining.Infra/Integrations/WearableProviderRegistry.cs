using CoachTraining.App.Abstractions.Integrations;
using CoachTraining.Domain.Enums;

namespace CoachTraining.Infra.Integrations;

public class WearableProviderRegistry : IWearableProviderRegistry
{
    private readonly IReadOnlyDictionary<ProvedorIntegracao, IWearableProvider> _providers;

    public WearableProviderRegistry(IEnumerable<IWearableProvider> providers)
    {
        _providers = providers.ToDictionary(provider => provider.Provedor);
    }

    public IWearableProvider GetRequired(ProvedorIntegracao provedor)
    {
        if (_providers.TryGetValue(provedor, out var provider))
        {
            return provider;
        }

        throw new InvalidOperationException($"Provedor '{provedor}' nao registrado.");
    }
}
