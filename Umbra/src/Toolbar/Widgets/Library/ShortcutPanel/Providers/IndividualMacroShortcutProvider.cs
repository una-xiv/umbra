using Umbra.Common;

namespace Umbra.Widgets.Library.ShortcutPanel.Providers;

[Service]
internal sealed class IndividualMacroShortcutProvider : MacroShortcutProvider
{
    public override string ShortcutType         => "IM"; // Individual Macro
    public override string ContextMenuEntryName => I18N.Translate("Widget.ShortcutPanel.ContextMenu.PickIndividualMacro");
    public override int    ContextMenuEntryOrder => -699;
}
