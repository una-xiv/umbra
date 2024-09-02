using Dalamud.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Umbra.Common;

namespace Umbra.Windows.Library.IconPicker;

public sealed partial class FaIconPickerWindow(FontAwesomeIcon icon) : Window
{
    public FontAwesomeIcon LastIcon { get; private set; } = icon;
    public FontAwesomeIcon Icon     { get; set; }         = icon;

    protected override Vector2 MinSize     { get; } = new(752, 600);
    protected override Vector2 MaxSize     { get; } = new(1200, 900);
    protected override Vector2 DefaultSize { get; } = new(752, 600);
    protected override string  Title       { get; } = I18N.Translate("Window.IconPicker.Title");

    protected override void OnOpen()
    {
        CloseButtonNode.OnMouseUp += _ => Close();

        UndoButtonNode.OnMouseUp += _ => {
            Icon = LastIcon;
            FaIconGridNode? node            = BodyNode.QuerySelector<FaIconGridNode>("IconGrid");
            if (node != null) node.Selected = Icon;
        };

        SearchInputNode.OnValueChanged += OnSearchValueChanged;
    }

    protected override void OnClose()
    {
        SearchInputNode.OnValueChanged -= OnSearchValueChanged;
    }

    protected override void OnUpdate(int instanceId)
    {
        UpdateNodeSizes();
    }

    private void OnSearchValueChanged(string value)
    {
        FaIconGridNode? node = BodyNode.QuerySelector<FaIconGridNode>("IconGrid");
        if (node != null) node.SearchFilter = value;
    }
}
