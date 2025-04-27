using System.Collections.Generic;
using Umbra.Common;
using Umbra.Game;
using Una.Drawing;

namespace Umbra.Widgets;

[ToolbarWidget(
    "ItemButton",
    "Widget.ItemButton.Name",
    "Widget.ItemButton.Description",
    ["button", "item", "use", "inventory"]
)]
internal sealed partial class ItemButtonWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
)
    : StandardToolbarWidget(info, guid, configValues)
{
    protected override StandardWidgetFeatures Features => StandardWidgetFeatures.Icon | StandardWidgetFeatures.Text;

    private IPlayer Player { get; } = Framework.Service<IPlayer>();

    private uint    ItemId   { get; set; }
    private uint?   IconId   { get; set; }
    private string? ItemName { get; set; }

    protected override uint DefaultGameIconId => 14;

    public override string GetInstanceName()
    {
        return ItemName is not null
            ? $"{I18N.Translate("Widget.ItemButton.Name")} - {ItemName}"
            : $"{I18N.Translate("Widget.ItemButton.Name")}";
    }

    protected override void OnLoad()
    {
        Node.OnClick += UseItem;
    }

    protected override void OnDraw()
    {
        var itemId = (uint)GetConfigValue<int>("ItemId");

        if (itemId != ItemId) {
            ItemId = itemId;
            var item = Player.FindResolvedItem(itemId);

            ItemName = item?.Name;
            IconId   = item?.IconId;
        }

        IsVisible = !GetConfigValue<bool>("HideIfNotOwned")
                    || Player.HasItemInInventory(ItemId, 1, GetItemUsage());

        bool showLabel = GetConfigValue<bool>("ShowLabel") && ItemName is not null;
        bool showCount = GetConfigValue<bool>("ShowCount");
        int  owned     = Player.GetItemCount(itemId, GetItemUsage());

        string name  = showLabel ? ItemName ?? "" : "";
        string count = showCount ? $"{owned}" : "";
        string label = showLabel && showCount ? $"{ItemName} x {owned}" : name + count;

        SetText(label);
        SetDisabled(!CanUseItem());
        SetGameIconId(IconId ?? 14);
    }

    private void UseItem(Node _)
    {
        if (!CanUseItem()) return;

        Player.UseInventoryItem(ItemId, GetItemUsage());
    }

    private ItemUsage GetItemUsage()
    {
        return GetConfigValue<string>("ItemUsage") switch {
            "HqBeforeNq" => ItemUsage.HqBeforeNq,
            "NqBeforeHq" => ItemUsage.NqBeforeHq,
            "HqOnly"     => ItemUsage.HqOnly,
            "NqOnly"     => ItemUsage.NqOnly,
            _            => ItemUsage.HqBeforeNq
        };
    }

    private bool CanUseItem()
    {
        if (ItemId == 0 || ItemName is null || !Player.HasItemInInventory(ItemId)) return false;

        return !(
            Player.IsCasting
            || Player.IsOccupied
            || Player.IsBetweenAreas
            || Player.IsDead
            || Player.IsInCutscene
            || Player.IsInIdleCam
        );
    }
}
