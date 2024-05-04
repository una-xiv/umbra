/* Umbra | (c) 2024 by Una              ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra is free software: you can redistribute  \/     \/             \/
 *     it and/or modify it under the terms of the GNU Affero General Public
 *     License as published by the Free Software Foundation, either version 3
 *     of the License, or (at your option) any later version.
 *
 *     Umbra UI is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System.Linq;
using Dalamud.Interface;
using Umbra.Common;
using Umbra.Interface;
using Umbra.Toolbar;

namespace Umbra.Windows.Settings;

internal partial class SettingsWindow
{
    private void BuildLayoutEditorButton()
    {
        CreateCategory("LayoutEditorPanel", I18N.Translate("Config.LayoutEditor"));
    }

    private Element _leftLayoutEditorColumn  = null!;
    private Element _rightLayoutEditorColumn = null!;

    private void BuildLayoutEditorPanel()
    {
        Element wrapper = BuildCategoryPanelWrapper("LayoutEditorPanel", I18N.Translate("Config.LayoutEditor"));

        _leftLayoutEditorColumn  = BuildLayoutEditorColumn("Left").Get("Content");
        _rightLayoutEditorColumn = BuildLayoutEditorColumn("Right").Get("Content");

        Element panel = new(
            id: "LayoutEditor",
            flow: Flow.Horizontal,
            anchor: Anchor.TopLeft,
            gap: 16,
            children: [
                _leftLayoutEditorColumn.Parent!,
                _rightLayoutEditorColumn.Parent!,
            ]
        );

        panel.OnBeforeCompute += () => {
            panel.Size = new(WindowWidth - 268, WindowHeight - 118);

            for (var i = 0; i < panel.Children.Count(); i++) {
                panel.Children.ElementAt(i).SortIndex = i;
                panel.Children.ElementAt(i).Size      = panel.Size with { Width = panel.Size.Width / 2 };
            }
        };

        foreach (var widget in Framework.Service<ToolbarLayout>().Widgets.Values) {
            Element ctrl = BuildLayoutEditorWidgetControl(widget);

            switch (widget.Element.Anchor) {
                case Anchor.MiddleLeft:
                    _leftLayoutEditorColumn.AddChild(ctrl);
                    break;
                case Anchor.MiddleRight:
                    _rightLayoutEditorColumn.AddChild(ctrl);
                    break;
            }
        }

        wrapper.AddChild(panel);
    }

    private Element BuildLayoutEditorColumn(string id)
    {
        return new OverflowContainer(
            id: id,
            children: [
                new(
                    id: "Content",
                    padding: new(6),
                    flow: Flow.Vertical,
                    anchor: Anchor.TopLeft,
                    children: []
                )
            ]
        ) {
            Style = new() {
                BackgroundColor = Theme.Color(ThemeColor.Background),
                Gradient = Gradient.Vertical(Theme.Color(ThemeColor.Background), Theme.Color(ThemeColor.BackgroundDark))
            }
        };
    }

    private Element BuildLayoutEditorWidgetControl(IToolbarWidget widget)
    {
        Element ctrl = new(
            id: $"Widget_Control_{widget.Element.Id}",
            flow: Flow.Horizontal,
            anchor: Anchor.TopLeft,
            gap: 6,
            children: [
                new ButtonElement(
                    id: "MoveUpButton",
                    icon: FontAwesomeIcon.ArrowUp
                ) { Tooltip = I18N.Translate("Config.LayoutEditor.MoveUp") },
                new ButtonElement(
                    id: "MoveDownButton",
                    icon: FontAwesomeIcon.ArrowDown
                ) { Tooltip = I18N.Translate("Config.LayoutEditor.MoveDown") },
                new ButtonElement(
                    id: "SwitchAnchorButton",
                    icon: FontAwesomeIcon.ArrowsAltH
                ) { Tooltip = I18N.Translate("Config.LayoutEditor.Remove") },
                new(
                    id: "Label",
                    text: I18N.Translate($"Widget.{widget.Element.Id}"),
                    style: new() {
                        Font         = Font.Axis,
                        TextColor    = Theme.Color(ThemeColor.Text),
                        TextAlign    = Anchor.MiddleLeft,
                        TextOffset   = new(0, 4),
                        OutlineColor = Theme.Color(ThemeColor.TextOutline),
                        OutlineWidth = 1,
                    }
                ),
            ]
        );

        ctrl.Get("SwitchAnchorButton").OnMouseUp += () => {
            Framework.Service<ToolbarLayout>().SwapAnchors(widget.Element.Id);
            UpdateLayoutWidgetControlColumns();
        };

        ctrl.Get("MoveUpButton").OnMouseUp += () => {
            Framework.Service<ToolbarLayout>().MoveWidgetUp(widget.Element.Id);
        };

        ctrl.Get("MoveDownButton").OnMouseUp += () => {
            Framework.Service<ToolbarLayout>().MoveWidgetDown(widget.Element.Id);
        };

        ctrl.OnBeforeCompute += () => {
            int lastSortIndex = Framework.Service<ToolbarLayout>().GetLastSortIndexOf(widget.Element.Anchor);

            ctrl.Size      = new(ctrl.Parent!.Size.Width, 32);
            ctrl.SortIndex = widget.Element.SortIndex;

            ctrl.Get("Label").Style.Opacity          = widget.Element.IsVisible ? 1 : 0.5f;
            ctrl.Get("MoveUpButton").IsDisabled      = widget.Element.SortIndex == 0;
            ctrl.Get("MoveUpButton").Style.Opacity   = widget.Element.SortIndex == 0 ? 0.5f : 1;
            ctrl.Get("MoveDownButton").IsDisabled    = widget.Element.SortIndex == lastSortIndex;
            ctrl.Get("MoveDownButton").Style.Opacity = widget.Element.SortIndex == lastSortIndex ? 0.5f : 1;
        };

        return ctrl;
    }

    private void UpdateLayoutWidgetControlColumns()
    {
        Framework.DalamudFramework.Run(
            () => {
                foreach (var widget in Framework.Service<ToolbarLayout>().Widgets.Values) {
                    var id = $"Widget_Control_{widget.Element.Id}";

                    if (widget.Element.Anchor == Anchor.MiddleLeft
                     && !_leftLayoutEditorColumn.Has(id)
                     && _rightLayoutEditorColumn.Has(id)) {
                        Element el = _rightLayoutEditorColumn.Get(id);

                        _rightLayoutEditorColumn.RemoveChild(el);
                        _leftLayoutEditorColumn.AddChild(el);
                    } else if (widget.Element.Anchor == Anchor.MiddleRight
                            && !_rightLayoutEditorColumn.Has(id)
                            && _leftLayoutEditorColumn.Has(id)) {
                        Element el = _leftLayoutEditorColumn.Get(id);

                        _leftLayoutEditorColumn.RemoveChild(el);
                        _rightLayoutEditorColumn.AddChild(el);
                    }
                }
            }
        );
    }
}
