using System.Collections.Generic;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Umbra.Common;
using Umbra.Game;
using Una.Drawing;

namespace Umbra.Widgets;

[ToolbarWidget("CustomButton", "Widget.CustomButton.Name", "Widget.CustomButton.Description", ["button", "command", "macro", "action", "url", "website"])]
internal sealed partial class CustomButtonWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
)
    : StandardToolbarWidget(info, guid, configValues)
{
    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Text |
        StandardWidgetFeatures.Icon | 
        StandardWidgetFeatures.CustomizableIcon;

    private IChatSender      ChatSender      { get; } = Framework.Service<IChatSender>();
    private ICommandManager  CommandManager  { get; } = Framework.Service<ICommandManager>();

    public override string GetInstanceName()
    {
        return $"{I18N.Translate("Widget.CustomButton.Name")} - {GetConfigValue<string>("Label")}";
    }

    protected override uint DefaultGameIconId => 14u;

    protected override void OnLoad()
    {
        Node.OnClick += InvokeCommand;
        Node.OnRightClick += InvokeAltCommand;
    }

    protected override void OnDraw()
    {
        SetText(GetConfigValue<bool>("HideLabel") ? null : GetConfigValue<string?>("Label"));

        string tooltipString = GetConfigValue<string>("Tooltip");
        Node.Tooltip = !string.IsNullOrEmpty(tooltipString) ? tooltipString : null;
    }

    private void InvokeCommand(Node _)
    {
        InvokeCommand(GetConfigValue<string>("Mode"), GetConfigValue<string>("Command").Trim());
    }

    private void InvokeAltCommand(Node _)
    {
        InvokeCommand(GetConfigValue<string>("AltMode"), GetConfigValue<string>("AltCommand").Trim());
    }

    private void InvokeCommand(string mode, string command)
    {
        switch (mode) {
            case "Command":
                if (string.IsNullOrEmpty(command) || !command.StartsWith('/')) {
                    return;
                }

                if (CommandManager.Commands.ContainsKey(command.Split(" ", 2)[0])) {
                    CommandManager.ProcessCommand(command);
                    return;
                }

                ChatSender.Send(command);
                return;
            case "URL":
                if (!command.StartsWith("http://") && !command.StartsWith("https://")) {
                    command = $"https://{command}";
                }
                Util.OpenLink(command);
                return;
        }
    }
}
