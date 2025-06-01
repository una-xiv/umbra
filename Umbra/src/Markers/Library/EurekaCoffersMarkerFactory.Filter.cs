using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Umbra.Game;

namespace Umbra.Markers.Library;

internal sealed partial class EurekaCoffersMarkerFactory
{
    private void OnChatMessage(
        XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled
    )
    {
        if (!_isListeningForNotifications) return;

        var positions = ChatLocationFilter.FilterPositions(
            message.TextValue.Trim(),
            _player.Position,
            CofferPositions[_zoneManager.CurrentZone.TerritoryId]
        );

        if (positions != null) _detectedCofferPositions = positions;

        AddMapMarkers();
    }
}
