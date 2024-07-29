using Dalamud.Interface;
using Dalamud.Plugin.Services;
using System.Collections.Generic;
using Umbra.Common;

namespace Umbra.Widgets.Library.EmoteChatIndicator;

[ToolbarWidget("EmoteChatIndicator", "Widget.EmoteChatIndicator.Name", "Widget.EmoteChatIndicator.Description")]
internal class EmoteChatIndicatorWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : IconToolbarWidget(info, guid, configValues)
{
    private IGameConfig GameConfig { get; } = Framework.Service<IGameConfig>();

    /// <inheritdoc/>
    public override WidgetPopup? Popup => null;

    /// <inheritdoc/>
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new BooleanWidgetConfigVariable(
                "OnlyShowWhenEnabled",
                I18N.Translate("Widget.EmoteChatIndicator.Config.OnlyShowWhenEnabled.Name"),
                I18N.Translate("Widget.EmoteChatIndicator.Config.OnlyShowWhenEnabled.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new BooleanWidgetConfigVariable(
                "Decorate",
                I18N.Translate("Widget.EmoteChatIndicator.Config.Decorate.Name"),
                I18N.Translate("Widget.EmoteChatIndicator.Config.Decorate.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "IconYOffset",
                I18N.Translate("Widget.EmoteChatIndicator.Config.IconYOffset.Name"),
                I18N.Translate("Widget.EmoteChatIndicator.Config.IconYOffset.Description"),
                -1,
                -5,
                5
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
        ];
    }

    /// <inheritdoc/>
    protected override void Initialize()
    {
        Node.OnMouseUp += _ => IsEmoteChatEnabled = !IsEmoteChatEnabled;
    }

    protected override void OnUpdate()
    {
        SetGhost(!GetConfigValue<bool>("Decorate"));
        SetIconYOffset(GetConfigValue<int>("IconYOffset"));

        if (IsEmoteChatEnabled) {
            Node.Style.IsVisible = true;
            Node.Tooltip         = I18N.Translate("Widget.EmoteChatIndicator.Tooltip.Enabled");

            SetIcon(FontAwesomeIcon.CommentDots);
        } else {
            Node.Style.IsVisible = !GetConfigValue<bool>("OnlyShowWhenEnabled");
            Node.Tooltip         = I18N.Translate("Widget.EmoteChatIndicator.Tooltip.Disabled");

            SetIcon(FontAwesomeIcon.CommentSlash);
        }
    }

    private bool IsEmoteChatEnabled {
        get => GameConfig.UiConfig.GetBool("EmoteTextType");
        set => GameConfig.UiConfig.Set("EmoteTextType", value);
    }
}
