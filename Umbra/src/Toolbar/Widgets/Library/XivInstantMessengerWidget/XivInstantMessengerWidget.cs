using Dalamud.Plugin.Services;
using System;
using System.Collections.Generic;
using Umbra.Common;
using Una.Drawing;

namespace Umbra.Widgets.Library.XIVInstantMessengerWidget;

[InteropToolbarWidget(
    "XIVInstantMessengerWidget",
    "Widget.XivInstantMessenger.Name",
    "Widget.XivInstantMessenger.Description",
    "Messenger", // XIV Instant Messenger internal plugin name.
    ["xim", "instant", "messenger", "whisper", "chat", "xiv"]
)]
public sealed partial class XivInstantMessengerWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Text |
        StandardWidgetFeatures.Icon;

    public override MenuPopup Popup { get; } = new();

    protected override void OnLoad()
    {
        SetGameIconId(63933);

        Node.OnMouseDown  += OnMouseDown;
        Node.OnRightClick += OpenXimWindow;
    }

    protected override void OnDraw()
    {
        int count  = GetConversationCount();
        int unread = GetUnreadConversationCount();

        SetDisabled(count == 0);
        SetText(unread > 0 ? I18N.Translate("Widget.XivInstantMessenger.UnreadCountLabel", unread) : null);

        Popup.IsDisabled = count == 0;
    }

    protected override void OnUnload()
    {
        Node.OnMouseDown  -= OnMouseDown;
        Node.OnRightClick -= OpenXimWindow;
    }

    private void OnMouseDown(Node _)
    {
        Popup.Clear();

        if (GetConversationCount() == 0) {
            return;
        }

        List<(string name, bool isUnread)> conversations = GetConversations();
        conversations.Sort((a, b) => String.Compare(a.name, b.name, StringComparison.OrdinalIgnoreCase));

        int index = 0;

        foreach (var (name, isUnread) in conversations) {
            index++;

            string[] nameParts = name.Split('@');

            Popup.Add(new MenuPopup.Button($"C_{index}") {
                Label             = nameParts[0],
                AltText           = nameParts[1],
                Icon              = isUnread ? (uint)63933 : null,
                ClosePopupOnClick = false,
                OnClick           = () => OpenMessenger(name, true),
            });
        }
    }

    private static void OpenXimWindow(Node _)
    {
        Framework.Service<ICommandManager>().ProcessCommand("/xim");
    }
}
