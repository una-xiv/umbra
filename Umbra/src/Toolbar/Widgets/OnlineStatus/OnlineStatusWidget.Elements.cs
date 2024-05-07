using FFXIVClientStructs.FFXIV.Client.UI.Info;
using Umbra.Interface;

namespace Umbra.Toolbar.Widgets.OnlineStatus;

internal partial class OnlineStatusWidget
{
    public Element Element { get; } = new(
        id: "OnlineStatusWidget",
        flow: Flow.Horizontal,
        anchor: Anchor.MiddleRight,
        sortIndex: 1,
        gap: 6,
        size: new(0, 28),
        children: [
            new BackgroundElement(
                color: Theme.Color(ThemeColor.BackgroundDark),
                edgeColor: Theme.Color(ThemeColor.BorderDark),
                edgeThickness: 1,
                rounding: 4
            ),
            new BorderElement(color: Theme.Color(ThemeColor.Border), rounding: 3, padding: new(1)),
            new(
                id: "Icon",
                size: new(28, 19),
                padding: new(left: 4, right: 4),
                anchor: Anchor.MiddleLeft,
                style: new() {
                    Image       = 15,
                    ImageOffset = new(0, -1)
                }
            ),
        ]
    );

    private readonly DropdownElement _dropdownElement = new(
        id: "OnlineStatusDropdown",
        anchor: Anchor.MiddleRight,
        children: [
            new(
                id: "Items",
                flow: Flow.Vertical,
                gap: 6,
                padding: new(6, 2),
                children: []
            )
        ]
    );

    private void AddStatusSwitchButton(Lumina.Excel.GeneratedSheets.OnlineStatus status, bool isAvailable)
    {
        Element list = _dropdownElement.Get("Items");
        var elementId = $"DropdownButton_{status.RowId}";

        if (false == list.Has(elementId)) {
            Element button = new DropdownButtonElement(
                id: $"DropdownButton_{status.RowId}",
                label: status.Name.ToString(),
                icon: status.Icon
            );

            _dropdownElement.Get("Items").AddChild(button);

            button.OnClick += () => {
                _popupContext.Clear();

                if (!isAvailable) return;
                _player.SetOnlineStatus(status.RowId);
            };
        }

        list.Get(elementId).IsVisible = isAvailable;
    }
}
