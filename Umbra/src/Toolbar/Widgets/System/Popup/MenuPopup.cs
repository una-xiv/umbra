using System.Collections.Generic;
using System.Collections.Immutable;
using Umbra.Common;
using Una.Drawing;

namespace Umbra.Widgets;

public sealed partial class MenuPopup : WidgetPopup
{
    public bool IsDisabled          { get; set; }
    public bool DesaturateMenuIcons { get; set; }

    protected override Node Node { get; }

    private int VerticalItemSpacing { get; set; }

    private readonly UdtDocument _document = UmbraDrawing.DocumentFrom("umbra.widgets._popup_menu.xml");

    private readonly Dictionary<Node, IMenuItem> _items = [];

    public MenuPopup()
    {
        Node = _document.RootNode!;
    }

    protected override void OnUpdate()
    {
        foreach (var button in Node.QuerySelectorAll(".icon")) {
            button.Style.ImageGrayscale = DesaturateMenuIcons;
        }

        Node.Style.Gap = 2 + VerticalItemSpacing;

        foreach (var group in Node.QuerySelectorAll(".group > .content")) {
            group.Style.Gap = 2 + VerticalItemSpacing;
        }
    }

    protected override void UpdateConfigVariables(ToolbarWidget widget)
    {
        DesaturateMenuIcons = widget.HasConfigVariable("DesaturateMenuIcons") && widget.GetConfigValue<bool>("DesaturateMenuIcons");
        VerticalItemSpacing = widget.HasConfigVariable("VerticalItemSpacing") ? widget.GetConfigValue<int>("VerticalItemSpacing") : 0;
    }

    public override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new BooleanWidgetConfigVariable(
                "DesaturateMenuIcons",
                I18N.Translate("Widgets.MenuPopup.Config.DesaturateMenuIcons.Name"),
                I18N.Translate("Widgets.MenuPopup.Config.DesaturateMenuIcons.Description"),
                false
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new IntegerWidgetConfigVariable(
                "VerticalItemSpacing",
                I18N.Translate("Widgets.MenuPopup.Config.VerticalItemSpacing.Name"),
                I18N.Translate("Widgets.MenuPopup.Config.VerticalItemSpacing.Description"),
                0,
                0,
                100
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
        ];
    }

    public void Clear(bool disposeChildren = false)
    {
        foreach (var item in _items.Values.ToImmutableArray()) {
            Remove(item, disposeChildren);
        }

        _items.Clear();
    }

    public void Add(IMenuItem item)
    {
        if (!_items.TryAdd(item.Node, item)) return;

        Node.AppendChild(item.Node);

        if (item is Button) item.Node.OnClick      += OnButtonClicked;
        if (item is Group grp) grp.OnButtonClicked += OnGroupButtonClicked;
    }

    public void Remove(IMenuItem item, bool dispose = false)
    {
        if (_items.Remove(item.Node)) {
            item.Node.Remove(dispose);
            if (item is Button) item.Node.OnClick      -= OnButtonClicked;
            if (item is Group grp) grp.OnButtonClicked -= OnGroupButtonClicked;
        }
    }

    public void RemoveById(string id, bool dispose = false)
    {
        foreach (var item in _items.Values.ToImmutableArray()) {
            if (item is Group group) {
                group.RemoveById(id, dispose);
            } else if (item.Node.Id == id) {
                Remove(item);
            }
        }
    }

    protected override bool CanOpen()
    {
        return !IsDisabled && base.CanOpen();
    }

    private void OnButtonClicked(Node node)
    {
        if (_items.TryGetValue(node, out IMenuItem? item)) {
            if (item is Button { ClosePopupOnClick: true }) {
                Close();
            }
        }
    }

    private void OnGroupButtonClicked(IMenuItem item)
    {
        if (item is Button { ClosePopupOnClick: true }) {
            Close();
        }
    }
}
