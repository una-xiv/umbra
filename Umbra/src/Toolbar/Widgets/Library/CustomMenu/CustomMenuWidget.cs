using Dalamud.Plugin.Services;
using Dalamud.Utility;
using System.Collections.Generic;
using System.Globalization;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets;

[ToolbarWidget(
    "CustomMenu",
    "Widget.CustomMenu.Name",
    "Widget.CustomMenu.Description",
    ["button", "command", "macro", "action", "url", "website", "menu", "list"]
)]
[DeprecatedToolbarWidget("Widget.DynamicMenu.Name")]
internal sealed partial class CustomMenuWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    public override MenuPopup Popup { get; } = new();

    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Text |
        StandardWidgetFeatures.Icon |
        StandardWidgetFeatures.CustomizableIcon;

    private ICommandManager  CommandManager  { get; } = Framework.Service<ICommandManager>();
    private IChatSender      ChatSender      { get; } = Framework.Service<IChatSender>();
    private ITextureProvider TextureProvider { get; } = Framework.Service<ITextureProvider>();
    private IPlayer          Player          { get; } = Framework.Service<IPlayer>();

    public override string GetInstanceName()
    {
        return $"{I18N.Translate("Widget.CustomMenu.Name")} - {GetConfigValue<string>("Label")}";
    }

    protected override void OnLoad()
    {
        Popup.OnPopupOpen += UpdateItemList;
    }

    protected override void OnDraw()
    {
        SetText(GetConfigValue<bool>("HideLabel") ? null : GetConfigValue<string?>("Label"));
        string tooltipString = GetConfigValue<string>("Tooltip");
        Node.Tooltip            = !string.IsNullOrEmpty(tooltipString) ? tooltipString : null;
    }

    protected override void OnUnload()
    {
        Popup.OnPopupOpen -= UpdateItemList;
    }

    private void UpdateItemList()
    {
        Popup.Clear();

        bool inverseOrder = GetConfigValue<bool>("InverseOrder");

        for (var i = 0; i < MaxButtons; i++) {
            string    label     = GetConfigValue<string>($"ButtonLabel_{i}").Trim();
            string    altLabel  = GetConfigValue<string>($"ButtonAltLabel_{i}").Trim();
            string    command   = GetConfigValue<string>($"ButtonCommand_{i}").Trim();
            string    mode      = GetConfigValue<string>($"ButtonMode_{i}").Trim();
            uint      iconId    = GetConfigValue<uint>($"ButtonIconId_{i}");
            uint      iconColor = GetConfigValue<uint>($"ButtonIconColor_{i}");
            ItemUsage usage     = ParseItemUsageString(GetConfigValue<string>($"ButtonItemUsage_{i}"));

            if (string.IsNullOrEmpty(command) && string.IsNullOrEmpty(label) && mode != "Separator") {
                continue;
            }

            switch (mode) {
                case "Command":
                case "URL":
                    AddMenuItem(i, label, altLabel, iconId, iconColor);
                    break;
                case "Item":
                    AddItemMenuItem(i, command, usage);
                    break;
                case "Separator":
                    Popup.Add(new MenuPopup.Separator());
                    break;
            }
        }
    }

    private void AddMenuItem(int index, string label, string altLabel, uint iconId, uint? iconColor)
    {
        Popup.Add(new MenuPopup.Button(label) {
            AltText   = altLabel,
            Icon      = GetExistingIconId(iconId),
            IconColor = iconColor != null ? new(iconColor.Value) : null,
            OnClick   = () => InvokeMenuItem(index),
        });
    }

    private void AddItemMenuItem(int index, string command, ItemUsage usage)
    {
        if (false == uint.TryParse(command, NumberStyles.Any, null, out uint itemId)) {
            Popup.Add(new MenuPopup.Button($"Invalid item id: {command}"));
            return;
        }

        ResolvedItem? item = Player.FindResolvedItem(itemId);

        if (null == item) {
            Popup.Add(new MenuPopup.Button($"Invalid item id: {command}"));
            return;
        }

        Popup.Add(new MenuPopup.Button(item.Value.Name) {
            Icon       = item.Value.IconId,
            AltText    = Player.GetItemCount(itemId, usage).ToString(),
            IsDisabled = !Player.HasItemInInventory(itemId, 1, usage),
            OnClick    = () => InvokeMenuItem(index),
        });
    }

    private void InvokeMenuItem(int index)
    {
        string mode = GetConfigValue<string>($"ButtonMode_{index}");
        string cmd  = GetConfigValue<string>($"ButtonCommand_{index}");

        switch (mode) {
            case "Command":
                if (string.IsNullOrEmpty(cmd) || !cmd.StartsWith('/')) {
                    return;
                }

                if (CommandManager.Commands.ContainsKey(cmd.Split(" ", 2)[0])) {
                    CommandManager.ProcessCommand(cmd);
                    return;
                }

                ChatSender.Send(cmd);
                return;
            case "URL":
                if (!cmd.StartsWith("http://") && !cmd.StartsWith("https://")) {
                    cmd = "https://" + cmd;
                }

                Util.OpenLink(cmd);
                return;
            case "Item":
                uint      itemId = uint.Parse(cmd, NumberStyles.Any, null);
                ItemUsage usage  = ParseItemUsageString(GetConfigValue<string>($"ButtonItemUsage_{index}"));

                if (!Player.HasItemInInventory(itemId, 1, usage)) {
                    return;
                }

                Player.UseInventoryItem(itemId, usage);
                return;
        }
    }

    /// <summary>
    /// Returns the ID of an icon that is bound to exist.
    /// </summary>
    private uint? GetExistingIconId(uint iconId)
    {
        uint existingCustomIconId = iconId;

        if (existingCustomIconId > 0) {
            // Make sure the icon exists.
            if (IconByIdExists(existingCustomIconId) == false) {
                existingCustomIconId = 0;
            }
        }

        return existingCustomIconId == 0 ? null : existingCustomIconId;
    }

    private bool IconByIdExists(uint iconId)
    {
        try {
            TextureProvider.GetIconPath(iconId);
            return true;
        } catch {
            return false;
        }
    }

    private static ItemUsage ParseItemUsageString(string usage)
    {
        return usage switch {
            "HqBeforeNq" => ItemUsage.HqBeforeNq,
            "NqBeforeHq" => ItemUsage.NqBeforeHq,
            "HqOnly"     => ItemUsage.HqOnly,
            "NqOnly"     => ItemUsage.NqOnly,
            _            => ItemUsage.HqBeforeNq
        };
    }
}
