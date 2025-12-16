using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace Umbra.Game;

[Service]
internal sealed class ZoneManager(ZoneFactory factory, IClientState clientState, IObjectTable objectTable) : IZoneManager, IDisposable
{
    public event Action<IZone>? ZoneChanged;

    private Zone? _zone = null;

    public bool HasCurrentZone => _zone != null;

    public IZone CurrentZone {
        get {
            if (null == _zone) {
                throw new InvalidOperationException("Zone not initialized");
            }

            return _zone;
        }
    }

    public void Dispose()
    {
        foreach (var handler in ZoneChanged?.GetInvocationList() ?? []) ZoneChanged -= (Action<IZone>)handler;

        _zone = null;
    }

    public IZone GetZone(uint zoneId)
    {
        return factory.GetZone(zoneId);
    }

    [OnTick]
    internal void OnTick()
    {
        if (clientState.MapId == 0) {
            return;
        }

        // Don't do anything if the local player isn't available.
        if (objectTable.LocalPlayer == null) return;

        if (null == _zone || _zone.Id != clientState.MapId) {
            _zone = factory.GetZone(clientState.MapId);
            ZoneChanged?.Invoke(_zone);
        }

        _zone?.Update();
    }
}
