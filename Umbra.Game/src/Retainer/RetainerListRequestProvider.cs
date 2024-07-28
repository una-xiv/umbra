using Dalamud.Plugin.Services;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game;
using Umbra.Common;

namespace Umbra.Game.Retainer;

[Service]
internal unsafe class RetainerListRequestProvider
{
    private delegate void RequestRetainerListDelegate(RetainerManager* mgr, nint callbackTarget = 0);

    [Signature("E8 ?? ?? ?? ?? 32 C0 C6 47 34 01")]
    private readonly RequestRetainerListDelegate? _requestRetainerList = null;

    private readonly IPlayer _player;

    public RetainerListRequestProvider(IPlayer player, IGameInteropProvider interopProvider)
    {
        _player = player;
        interopProvider.InitializeFromAttributes(this);
    }

    /// <summary>
    /// Refresh the retainer list as seen in the /timer window.
    /// </summary>
    /// <remarks>
    /// This function makes a request to the server. To stay in line with
    /// Dalamud's plugin guidelines, it MUST only be called after the user
    /// specifically does a manual interaction. In our case, it is only called
    /// if and only if the user opens the retainer widget and the information
    /// is not already available.
    /// </remarks>
    internal void RequestRetainerList()
    {
        if (_player.CurrentWorldName != _player.HomeWorldName || _player.IsBoundByInstancedDuty) {
            // The game performs the same checks when opening the timer window. Retainer info
            // is only accessible on your home world and not while in an instance. (Thanks Hasel!)
            return;
        }

        if (null == _requestRetainerList) {
            Logger.Warning("Failed to find RefreshRetainerList signature.");
            return;
        }

        RetainerManager* rm = RetainerManager.Instance();
        if (rm == null) return;

        _requestRetainerList(rm);
    }
}
