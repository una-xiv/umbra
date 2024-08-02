using System.Collections.Generic;
using System.Text.Json;
using Umbra.Common;
using NotImplementedException = System.NotImplementedException;

namespace Umbra.Widgets.Library.EmoteList;

[ToolbarWidget("EmoteList", "Widget.EmoteList.Name", "Widget.EmoteList.Description")]
internal sealed partial class EmoteListWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : DefaultToolbarWidget(info, guid, configValues)
{
    public override EmoteListPopup Popup { get; } = new();

    /// <inheritdoc/>
    protected override void Initialize()
    {
        Popup.OnCategoryChanged += OnEmoteCategoryChanged;
        Popup.OnKeepOpenChanged += OnKeepOpenChanged;
        Popup.OnEmotesChanged   += OnEmotesChanged;

        // Load previously stored emotes.
        try {
            var emotes = JsonSerializer.Deserialize<Dictionary<byte, Dictionary<byte, uint>>>(GetConfigValue<string>("EmoteList"));
            if (null != emotes) {
                Popup.Emotes = emotes;
            }
        } catch {
            Logger.Warning("Failed to load emotes from config.");
        }
    }

    /// <inheritdoc/>
    protected override void OnDisposed()
    {
        Popup.OnCategoryChanged -= OnEmoteCategoryChanged;
        Popup.OnKeepOpenChanged -= OnKeepOpenChanged;
        Popup.OnEmotesChanged   -= OnEmotesChanged;
    }

    /// <inheritdoc/>
    protected override void OnUpdate()
    {
        SetLabel(GetConfigValue<string>("Label"));
        SetIcon((uint)GetConfigValue<int>("IconId"));

        Popup.KeepOpenAfterUse     = GetConfigValue<bool>("KeepOpenAfterUse");
        Popup.LastSelectedCategory = (byte)GetConfigValue<int>("LastSelectedCategory");

        Popup.EnabledCategories = [
            !string.IsNullOrEmpty(GetConfigValue<string>("Category_0_Name").Trim()),
            !string.IsNullOrEmpty(GetConfigValue<string>("Category_1_Name").Trim()),
            !string.IsNullOrEmpty(GetConfigValue<string>("Category_2_Name").Trim()),
            !string.IsNullOrEmpty(GetConfigValue<string>("Category_3_Name").Trim()),
        ];

        Popup.CategoryNames = [
            GetConfigValue<string>("Category_0_Name"),
            GetConfigValue<string>("Category_1_Name"),
            GetConfigValue<string>("Category_2_Name"),
            GetConfigValue<string>("Category_3_Name"),
        ];

        base.OnUpdate();
    }

    private void OnEmoteCategoryChanged(byte id)
    {
        SetConfigValue("LastSelectedCategory", (int)id);
    }

    private void OnKeepOpenChanged(bool value)
    {
        SetConfigValue("KeepOpenAfterUse", value);
    }

    private void OnEmotesChanged(Dictionary<byte, Dictionary<byte, uint>> emotes)
    {
        string data = JsonSerializer.Serialize(emotes);
        SetConfigValue("EmoteList", data);
    }
}
