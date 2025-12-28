using Dalamud.Utility;
using Umbra.Widgets.Library.ShortcutPanel.Providers;

namespace Umbra.Widgets;

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

    protected override Node Node { get; }

    private DynamicMenuEntry? ExpandedCategoryEntry { get; set; }

    private UdtDocument Document { get; } = UmbraDrawing.DocumentFrom("umbra.widgets.popup_dynamic_menu.xml");
    
    private ShortcutProviderRepository Providers      { get; } = Framework.Service<ShortcutProviderRepository>();
    private ICommandManager            CommandManager { get; } = Framework.Service<ICommandManager>();
    private IChatSender                ChatSender     { get; } = Framework.Service<IChatSender>();

    public DynamicMenuPopup()
    {
        Node = Document.RootNode!;
        
        CreateContextMenu();

        EmptyButtonPlaceholder.OnRightClick += _ => OpenContextMenu();
        EmptyButtonPlaceholder.OnClick      += _ => { };

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
        EmptyButtonPlaceholder.Tooltip = I18N.Translate("Widget.DynamicMenu.EmptyButtonPlaceholderTooltip");

        RebuildMenu();
    }

    private void RebuildMenu()
    {
        ClearMenu();

        if (ExpandedCategoryEntry != null && !Entries.Contains(ExpandedCategoryEntry)) {
            ExpandedCategoryEntry = null;
        }

        DynamicMenuEntry? activeCategory = null;
        bool isCategoryExpanded = false;

        foreach (var entry in Entries) {
            if (IsCategoryEntry(entry)) {
                activeCategory = entry;
                isCategoryExpanded = ExpandedCategoryEntry == entry;
            } else if (IsSeparatorEntry(entry)) {
                activeCategory = null;
                isCategoryExpanded = false;
            }

            if (activeCategory != null && !isCategoryExpanded && !IsCategoryEntry(entry)) {
                continue;
            }

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
    private static bool IsCategoryEntry(DynamicMenuEntry entry)
    {
        return entry.Cg;
    }

    private static bool IsSeparatorEntry(DynamicMenuEntry entry)
    {
        return entry is { Ct: null, Cl: "-" } && !entry.Cg;
    }
}
