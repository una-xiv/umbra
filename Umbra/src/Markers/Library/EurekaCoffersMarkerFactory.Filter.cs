using Dalamud.Game.Chat;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;

namespace Umbra.Markers.Library;

internal sealed partial class EurekaCoffersMarkerFactory
{
    private void OnChatMessage(IHandleableChatMessage message)
    {
        if (!_isListeningForNotifications) return;

        var positions = ChatLocationFilter.FilterPositions(
            message.Message.TextValue.Trim(),
            _player.Position,
            CofferPositions[_zoneManager.CurrentZone.TerritoryId]
        );

        if (positions != null) _detectedCofferPositions = positions;

        AddMapMarkers();
    }
}
