using Dalamud.Plugin.Ipc;

namespace Umbra.Widgets.Library.XIVInstantMessengerWidget;

public sealed partial class XivInstantMessengerWidget
{
    private readonly ICallGateSubscriber<int>? _getConversationCount =
        Framework.DalamudPlugin.GetIpcSubscriber<int>("Messenger.GetConversationCount");

    private readonly ICallGateSubscriber<int>? _getUnreadConversationCount =
        Framework.DalamudPlugin.GetIpcSubscriber<int>("Messenger.GetUnreadConversationCount");

    private readonly ICallGateSubscriber<List<(string nameWithWorld, bool isUnread)>>? _getConversations =
        Framework.DalamudPlugin.GetIpcSubscriber<List<(string nameWithWorld, bool isUnread)>>("Messenger.GetConversations");

    private readonly ICallGateSubscriber<string, bool, bool>? _openMessenger =
        Framework.DalamudPlugin.GetIpcSubscriber<string, bool, bool>("Messenger.OpenMessenger");

    /// <summary>
    /// Returns the number of loaded conversations.
    /// </summary>
    private int GetConversationCount()
    {
        try {
            return _getConversationCount?.InvokeFunc() ?? 0;
        } catch {
            return 0;
        }
    }

    /// <summary>
    /// Returns the number of unread conversations.
    /// </summary>
    private int GetUnreadConversationCount()
    {
        try {
            return _getUnreadConversationCount?.InvokeFunc() ?? 0;
        } catch {
            return 0;
        }
    }

    /// <summary>
    /// Returns a list of conversations.
    /// </summary>
    /// <remarks>
    /// Relatively expensive operation, use sparingly.
    /// </remarks>
    private List<(string nameWithWorld, bool isUnread)> GetConversations()
    {
        try {
            return _getConversations?.InvokeFunc() ?? [];
        } catch {
            return [];
        }
    }

    /// <summary>
    /// Opens the messenger window.
    /// </summary>
    /// <param name="nameWithWorld">Name of the recipient in the Name@World format.</param>
    /// <param name="focusInputOnOpen">Whether to focus the input field on open.</param>
    private void OpenMessenger(string nameWithWorld, bool focusInputOnOpen)
    {
        try {
            _openMessenger?.InvokeAction(nameWithWorld, focusInputOnOpen);
        } catch {
            // ignored
        }
    }
}
