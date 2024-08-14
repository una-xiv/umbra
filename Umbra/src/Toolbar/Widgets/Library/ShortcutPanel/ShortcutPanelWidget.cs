using System.Collections.Generic;
using Umbra.Common;

namespace Umbra.Widgets.Library.ShortcutPanel;

[ToolbarWidget("ShortcutPanel", "Widget.ShortcutPanel.Name", "Widget.ShortcutPanel.Description")]
internal sealed partial class ShortcutPanelWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : DefaultToolbarWidget(info, guid, configValues)
{
    public override ShortcutPanelPopup Popup { get; } = new();

    private string _lastShortcutData = string.Empty;

    protected override void Initialize()
    {
        Popup.OnShortcutsChanged += OnShortcutsChanged;
    }

    protected override void OnDisposed()
    {
        Popup.OnShortcutsChanged -= OnShortcutsChanged;
    }

    public override string GetInstanceName()
    {
        return $"{Info.Name} - {GetConfigValue<string>("ButtonLabel")}";
    }

    protected override void OnUpdate()
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

        SetLabel(GetConfigValue<string>("ButtonLabel"));
        SetIcon((uint)GetConfigValue<int>("ButtonIconId"));

        base.OnUpdate();
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

        Logger.Info($"SlotConfig Changed: {shortcutData}");
    }
}
