using System.Collections.Generic;
using System.Linq;
using Umbra.Common;

namespace Umbra.Widgets.Library.ShortcutPanel.Providers;

[Service]
internal sealed class ShortcutProviderRepository
{
    private Dictionary<string, AbstractShortcutProvider> Providers { get; } = [];

    public ShortcutProviderRepository(AbstractShortcutProvider[] providers)
    {
        foreach (var provider in providers) {
            if (!Providers.TryAdd(provider.ShortcutType, provider)) {
                Logger.Warning($"Another provider with the same shortcut type already exists: {provider.ShortcutType}");
            }
        }
    }

    public AbstractShortcutProvider? GetProvider(string shortcutType)
    {
        return Providers.GetValueOrDefault(shortcutType);
    }

    public IEnumerable<AbstractShortcutProvider> GetAllProviders()
    {
        List<AbstractShortcutProvider> providers = Providers.Values.ToList();
        providers.Sort((a, b) => a.ContextMenuEntryOrder.CompareTo(b.ContextMenuEntryOrder));

        return providers;
    }
}
