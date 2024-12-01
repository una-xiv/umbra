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
    "Messenger" // XIV Instant Messenger internal plugin name.
)]
public sealed partial class XivInstantMessengerWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : DefaultToolbarWidget(info, guid, configValues)
{
    public override MenuPopup Popup { get; } = new();

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            ..DefaultToolbarWidgetConfigVariables,
            ..SingleLabelTextOffsetVariables
        ];
    }

    /// <inheritdoc/>
    protected override void Initialize()
    {
        SetIcon(63933);

        Node.OnMouseDown  += OnMouseDown;
        Node.OnRightClick += OpenXimWindow;
    }

    /// <inheritdoc/>
    protected override void OnUpdate()
    {
        int count  = GetConversationCount();
        int unread = GetUnreadConversationCount();

        SetDisabled(count == 0);
        SetLabel(unread > 0 ? I18N.Translate("Widget.XivInstantMessenger.UnreadCountLabel", unread) : null);

        Popup.IsDisabled = count == 0;

        base.OnUpdate();
    }

    /// <inheritdoc/>
    protected override void OnDisposed()
    {
        Node.OnMouseDown  -= OnMouseDown;
        Node.OnRightClick -= OpenXimWindow;

        base.OnDisposed();
    }

    /// <summary>
    /// Invoked when the user opens the popup.
    /// </summary>
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

            Popup.AddButton(
                $"C_{index}",
                label: nameParts[0],
                altText: nameParts[1],
                sortIndex: index,
                iconId: isUnread ? (uint)63933 : null,
                onClick: () => OpenMessenger(name, true)
            );
        }
    }

    private static void OpenXimWindow(Node _)
    {
        Framework.Service<ICommandManager>().ProcessCommand("/xim");
    }
}
