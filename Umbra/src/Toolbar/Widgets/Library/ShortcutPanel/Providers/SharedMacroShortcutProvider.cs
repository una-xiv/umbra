using Umbra.Common;

namespace Umbra.Widgets.Library.ShortcutPanel.Providers;

[Service]
internal sealed class SharedMacroShortcutProvider : MacroShortcutProvider
{
    public override string ShortcutType          => "SM"; // Shared Macro
    public override string ContextMenuEntryName  => I18N.Translate("Widget.ShortcutPanel.ContextMenu.PickSharedMacro");
    public override int    ContextMenuEntryOrder => -698;
}
