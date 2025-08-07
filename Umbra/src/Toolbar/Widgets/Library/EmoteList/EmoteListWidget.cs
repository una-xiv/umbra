using System.Text.Json;
using NotImplementedException = System.NotImplementedException;

namespace Umbra.Widgets.Library.EmoteList;

[ToolbarWidget("EmoteList", "Widget.EmoteList.Name", "Widget.EmoteList.Description", ["emote", "list", "panel", "shortcuts"])]
internal sealed partial class EmoteListWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    public override EmoteListPopup Popup { get; } = new();

    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Text |
        StandardWidgetFeatures.Icon |
        StandardWidgetFeatures.CustomizableIcon;

    protected override void OnLoad()
    {
        Node.OnRightClick += _ => {
            Framework.Service<IChatSender>().Send("/emotelist");
        };

        Popup.OnCategoryChanged += OnEmoteCategoryChanged;
        Popup.OnKeepOpenChanged += OnKeepOpenChanged;
        Popup.OnEmotesChanged   += OnEmotesChanged;

        try {
            var emotes =
                JsonSerializer.Deserialize<Dictionary<byte, Dictionary<byte, uint>>>(
                    GetConfigValue<string>("EmoteList")
                );

            if (null != emotes) {
                Popup.Emotes = emotes;
            }
        } catch {
            Logger.Warning("Failed to load emotes from config.");
        }
    }

    protected override void OnUnload()
    {
        Popup.OnCategoryChanged -= OnEmoteCategoryChanged;
        Popup.OnKeepOpenChanged -= OnKeepOpenChanged;
        Popup.OnEmotesChanged   -= OnEmotesChanged;
    }

    protected override void OnDraw()
    {
        SetText(GetConfigValue<string>("Label"));

        Popup.KeepOpenAfterUse     = GetConfigValue<bool>("KeepOpenAfterUse");
        Popup.LastSelectedCategory = (byte)GetConfigValue<int>("LastSelectedCategory");
        Popup.ShowEmptySlots       = GetConfigValue<bool>("ShowEmptySlots");

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
