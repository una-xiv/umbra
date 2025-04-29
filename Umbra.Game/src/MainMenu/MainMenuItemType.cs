namespace Umbra.Game;

public enum MainMenuItemType
{
    // A native main command.
    MainCommand,

    // A command invoked via a chat command.
    ChatCommand,

    // A command invoked via a callback action.
    Callback,
    Separator,
}
