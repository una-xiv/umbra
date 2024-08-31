using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Umbra.Common;
using Umbra.Common.Utility;

namespace Umbra.Widgets.Library.DynamicMenu;

[ToolbarWidget("DynamicMenu", "Widget.DynamicMenu.Name", "Widget.DynamicMenu.Description")]
internal sealed partial class DynamicMenuWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : DefaultToolbarWidget(info, guid, configValues)
{
    public override DynamicMenuPopup Popup { get; } = new();

    protected override void Initialize()
    {
        Popup.OnEditModeChanged += OnEditModeChanged;
        Popup.OnEntriesChanged  += OnEntriesChanged;

        string data = GetConfigValue<string>("Entries");

        if (string.IsNullOrEmpty(data)) return;

        try {
            var entries =
                JsonConvert.DeserializeObject<List<DynamicMenuPopup.DynamicMenuEntry>>(Compression.Decompress(data));

            if (null != entries) {
                Popup.Entries = entries;
            }
        } catch (Exception e) {
            Logger.Error($"Failed to load dynamic menu entries: {e.Message}");
        }
    }

    protected override void OnDisposed()
    {
        Popup.OnEditModeChanged -= OnEditModeChanged;
        Popup.OnEntriesChanged  -= OnEntriesChanged;
    }

    protected override void OnUpdate()
    {
        Popup.EditModeEnabled  = GetConfigValue<bool>("EditModeEnabled");
        Popup.WidgetInstanceId = Id;

        SetLabel(GetConfigValue<string>("ButtonLabel"));
        SetIcon((uint)GetConfigValue<int>("ButtonIcon"));

        base.OnUpdate();

        string tooltip = GetConfigValue<string>("ButtonTooltip").Trim();
        Node.Tooltip = string.IsNullOrEmpty(tooltip) ? null : tooltip;
    }

    private void OnEditModeChanged(bool state)
    {
        SetConfigValue("EditModeEnabled", state);
    }

    private void OnEntriesChanged()
    {
        string data = Compression.Compress(JsonConvert.SerializeObject(Popup.Entries));
        SetConfigValue("Entries", data);
    }
}
