using Dalamud.Interface;
using System;
using System.Collections.Generic;
using Umbra.Common;
using Umbra.Game;
using Umbra.Windows;
using Umbra.Windows.Settings;
using Una.Drawing;

namespace Umbra.Widgets.Library.UnifiedMainMenu;

internal sealed partial class UnifiedMainMenuPopup : WidgetPopup
{
    public event Action<List<string>>? OnPinnedItemsChanged;

    public uint   AvatarIconId         { get; set; } = 76985;
    public string BannerLocation       { get; set; } = "Auto";
    public string BannerNameStyle      { get; set; } = "FirstName";
    public string BannerColorStyle     { get; set; } = "AccentColor";
    public bool   DesaturateIcons      { get; set; } = false;
    public bool   OpenSubMenusOnHover  { get; set; } = false;
    public int    VerticalItemSpacing  { get; set; } = 0;
    public int    MenuHeight           { get; set; } = 400;
    public bool   ReverseCategoryOrder { get; set; } = false;
    public int    PopupFontSize        { get; set; } = 11;

    public List<string> PinnedItems { get; private set; } = [];

    protected override Node Node { get; }

    private IMainMenuRepository MainMenuRepository { get; } = Framework.Service<IMainMenuRepository>();
    private IPlayer             Player             { get; } = Framework.Service<IPlayer>();

    private UdtDocument Document { get; } = UmbraDrawing.DocumentFrom("umbra.widgets.popup_unified_main_menu.xml");

    private bool IsTopAligned => BannerLocation switch {
        "Top"    => true,
        "Bottom" => false,
        _        => !Toolbar.IsTopAligned
    };

    public UnifiedMainMenuPopup()
    {
        Node = Document.RootNode!;
    }

    protected override void OnOpen()
    {
        CreateSidePanelNodes();
        CreateContentNodes();
        CreateContextMenu();
        UpdatePinnedItems();
        
        Node.QuerySelector("#side-panel")!.Style.Gap = 2 + VerticalItemSpacing;
        PinnedListNode.Style.Gap                     = 2 + VerticalItemSpacing;

        foreach (var node in Node.QuerySelectorAll(".category")) {
            node.Style.Gap = 2 + VerticalItemSpacing;
        }

        Node.QuerySelector("#btn-shutdown")!.OnMouseUp += _ => {
            ContextMenu?.SetEntryVisible("MoveUp", false);
            ContextMenu?.SetEntryVisible("MoveDown", false);
            ContextMenu?.SetEntryVisible("Pin", false);
            ContextMenu?.SetEntryVisible("Unpin", false);
            ContextMenu?.SetEntryVisible("Logout", true);
            ContextMenu?.SetEntryVisible("Shutdown", true);
            ContextMenu?.Present();
        };

        Node.QuerySelector("#btn-settings")!.OnMouseUp += _ => {
            Framework.Service<WindowManager>().Present("UmbraSettings", new SettingsWindow());
            Close();
        };
        
        ActivateCategory("Category_Character");
    }

    protected override void OnClose()
    {
        ContextMenu?.Dispose();
    }

    public void SetPinnedItems(List<string> items)
    {
        PinnedItems = items;
    }

    protected override void OnUpdate()
    {
        UpdateHeaderNodes();

        foreach (var node in Node.QuerySelectorAll(".text, .alt")) {
            node.Style.FontSize = PopupFontSize;
        }
    }

    private void ActivateCategory(string id)
    {
        foreach (var node in SidePanelNode.QuerySelectorAll(".category-button")) {
            node.ToggleClass("selected", node.Id == $"{id}_Button");
        }

        foreach (var node in ContentsNode.QuerySelectorAll(".category")) {
            node.Style.IsVisible = node.Id == id;
        }
    }
}
