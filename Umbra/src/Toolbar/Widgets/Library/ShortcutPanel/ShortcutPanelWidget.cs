using System.Collections.Generic;
using Umbra.Common;

namespace Umbra.Widgets.Library.ShortcutPanel;

[ToolbarWidget("ShortcutPanel", "Widget.ShortcutPanel.Name", "Widget.ShortcutPanel.Description", ["shortcut", "panel", "hotbar", "action", "ability", "macro", "emote", "item", "macro", "command", "url", "website", "menu"])]
internal sealed partial class ShortcutPanelWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    public override ShortcutPanelPopup Popup { get; } = new();

    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Text |
        StandardWidgetFeatures.Icon |
        StandardWidgetFeatures.CustomizableIcon;

    private string _lastShortcutData = string.Empty;

    protected override void OnLoad()
    {
        Popup.OnShortcutsChanged += OnShortcutsChanged;
    }

    protected override void OnUnload()
    {
        Popup.OnShortcutsChanged -= OnShortcutsChanged;
    }

    public override string GetInstanceName()
    {
        return $"{Info.Name} - {GetConfigValue<string>("ButtonLabel")}";
    }

    protected override void OnDraw()
    {
        Popup.WidgetInstanceId = Id;
        Popup.NumRows          = (byte)GetConfigValue<int>("NumRows");
        Popup.NumCols          = (byte)GetConfigValue<int>("NumCols");
        Popup.ShowEmptySlots   = GetConfigValue<bool>("ShowEmptySlots");
        Popup.AutoCloseOnUse   = GetConfigValue<bool>("AutoCloseOnUse");

        UpdateNodeCategoryNames();

        string shortcutData = GetConfigValue<string>("SlotConfig");

        if (shortcutData != _lastShortcutData) {
            Popup.LoadShortcuts(shortcutData);
            _lastShortcutData = shortcutData;
        }

        SetText(GetConfigValue<string>("ButtonLabel"));
    }

    private void UpdateNodeCategoryNames()
    {
        List<string> categoryNames = [];

        for (var i = 0; i < 4; i++) {
            string name = GetConfigValue<string>($"CategoryName_{i}");
            if (!string.IsNullOrEmpty(name.Trim())) categoryNames.Add(name);
        }

        Popup.CategoryNames = categoryNames.ToArray();
    }

    private void OnShortcutsChanged(string shortcutData)
    {
        SetConfigValue("SlotConfig", shortcutData);
    }
}
