using Dalamud.Plugin.Services;
using Dalamud.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;
using Umbra.Game;
using Umbra.Widgets.Library.ShortcutPanel.Providers;
using Una.Drawing;

namespace Umbra.Widgets.Library.DynamicMenu;

internal sealed partial class DynamicMenuPopup : WidgetPopup
{
    public bool                   EditModeEnabled  { get; set; } = false;
    public string                 WidgetInstanceId { get; set; } = "-";
    public int                    EntryHeight      { get; set; } = 36;
    public int                    EntryFontSize    { get; set; } = 13;
    public int                    AltEntryFontSize { get; set; } = 11;
    public bool                   ShowSubIcons     { get; set; } = true;
    public bool                   ShowItemCount    { get; set; } = true;
    public List<DynamicMenuEntry> Entries          { get; set; } = [];

    public event Action?       OnEntriesChanged;
    public event Action<bool>? OnEditModeChanged;

    private ShortcutProviderRepository Providers      { get; } = Framework.Service<ShortcutProviderRepository>();
    private ICommandManager            CommandManager { get; } = Framework.Service<ICommandManager>();
    private IChatSender                ChatSender     { get; } = Framework.Service<IChatSender>();

    public DynamicMenuPopup()
    {
        CreateContextMenu();

        EmptyButtonPlaceholder.OnRightClick += _ => OpenContextMenu();

        Framework.DalamudFramework.Run(RebuildMenu);
    }

    protected override bool CanOpen()
    {
        return EditModeEnabled || Entries.Count > 0;
    }

    protected override void OnOpen()
    {
        EmptyButtonPlaceholder.Style.IsVisible   = EditModeEnabled;
        EmptyButtonPlaceholder.Style.BorderWidth = new() { Top = Entries.Count > 0 ? 1 : 0 };

        RebuildMenu();
    }

    private void RebuildMenu()
    {
        ClearMenu();

        foreach (var entry in Entries) {
            Node? node = CreateEntryNode(entry);
            if (node == null) continue;

            ItemList.AppendChild(node);
        }
    }

    private void ClearMenu()
    {
        foreach (var node in ItemList.ChildNodes.ToArray()) {
            node.Dispose();
        }
    }

    private void InvokeCustomEntry(DynamicMenuEntry entry)
    {
        if (string.IsNullOrWhiteSpace(entry.Ct)) return;
        if (string.IsNullOrWhiteSpace(entry.Cc)) return;

        string cmd = entry.Cc.Trim();

        switch (entry.Ct) {
            case "Chat":
                if (string.IsNullOrEmpty(cmd) || !cmd.StartsWith('/')) {
                    return;
                }

                if (CommandManager.Commands.ContainsKey(cmd.Split(" ", 2)[0])) {
                    CommandManager.ProcessCommand(cmd);
                    return;
                }

                ChatSender.Send(cmd);
                return;
            case "URL":
                if (!cmd.StartsWith("http://") && !cmd.StartsWith("https://")) {
                    cmd = $"https://{cmd}";
                }

                Util.OpenLink(cmd);
                return;
            default:
                Logger.Warning($"Invalid custom entry type: {entry.Ct} for command: {cmd}");
                break;
        }
    }
}
