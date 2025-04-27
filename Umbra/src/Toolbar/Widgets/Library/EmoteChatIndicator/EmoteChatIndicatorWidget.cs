using Dalamud.Interface;
using Dalamud.Plugin.Services;
using System.Collections.Generic;
using Umbra.Common;

namespace Umbra.Widgets.Library.EmoteChatIndicator;

[ToolbarWidget(
    "EmoteChatIndicator",
    "Widget.EmoteChatIndicator.Name",
    "Widget.EmoteChatIndicator.Description",
    ["emote", "chat", "indicator"]
)]
internal class EmoteChatIndicatorWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    protected override StandardWidgetFeatures Features => StandardWidgetFeatures.Icon;

    private IGameConfig GameConfig { get; } = Framework.Service<IGameConfig>();

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            ..base.GetConfigVariables(),

            new BooleanWidgetConfigVariable(
                "OnlyShowWhenEnabled",
                I18N.Translate("Widget.EmoteChatIndicator.Config.OnlyShowWhenEnabled.Name"),
                I18N.Translate("Widget.EmoteChatIndicator.Config.OnlyShowWhenEnabled.Description"),
                true
            ),
            new FaIconWidgetConfigVariable(
                "OffIcon",
                I18N.Translate("Widget.EmoteChatIndicator.Config.OffIcon.Name"),
                I18N.Translate("Widget.EmoteChatIndicator.Config.OffIcon.Description"),
                FontAwesomeIcon.CommentSlash
            ),
            new FaIconWidgetConfigVariable(
                "OnIcon",
                I18N.Translate("Widget.EmoteChatIndicator.Config.OnIcon.Name"),
                I18N.Translate("Widget.EmoteChatIndicator.Config.OnIcon.Description"),
                FontAwesomeIcon.CommentDots
            ),
        ];
    }

    protected override void OnLoad()
    {
        Node.OnMouseUp += _ => IsEmoteChatEnabled = !IsEmoteChatEnabled;
    }

    protected override void OnDraw()
    {
        if (IsEmoteChatEnabled) {
            IsVisible    = true;
            Node.Tooltip = I18N.Translate("Widget.EmoteChatIndicator.Tooltip.Enabled");

            SetFontAwesomeIcon(GetConfigValue<FontAwesomeIcon>("OnIcon"));
        } else {
            IsVisible    = !GetConfigValue<bool>("OnlyShowWhenEnabled");
            Node.Tooltip = I18N.Translate("Widget.EmoteChatIndicator.Tooltip.Disabled");

            SetFontAwesomeIcon(GetConfigValue<FontAwesomeIcon>("OffIcon"));
        }
    }

    private bool IsEmoteChatEnabled {
        get => GameConfig.UiConfig.GetBool("EmoteTextType");
        set => GameConfig.UiConfig.Set("EmoteTextType", value);
    }
}
