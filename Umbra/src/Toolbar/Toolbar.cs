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
using ImGuiNET;
using Umbra.Common;
using Umbra.Drawing;

namespace Umbra;

[Service]
internal sealed partial class Toolbar
{
    [ConfigVariable(
        "Toolbar.IsTopAligned",
        "Toolbar Settings",
        "Place the toolbar at the top of the screen.",
        "Aligns the toolbar to the top of the screen rather than the bottom."
    )]
    private static bool IsTopAligned { get; set; } = false;

    [ConfigVariable(
        "Toolbar.Height",
        "Toolbar Settings",
        "Toolbar Height",
        "Defines the height of the toolbar in pixels.",
        min: 24,
        max: 48,
        step: 1
    )]
    private static int Height { get; set; } = 32;

    private const uint TopColor    = 0xFF2F2E2F;
    private const uint BottomColor = 0xFF212021;

    private int _y;

    public Toolbar(IToolbarWidget[] widgetList)
    {
        foreach (var widget in widgetList) {
            RelocateWidget(widget.Element);
        }
    }

    [OnDraw]
    public void OnDraw()
    {
        UpdateWidgetContainers();
        UpdateToolbarSize();
        UpdateToolbarPosition();

        // Logger.Info($"Height: {Height}");

        ImDrawListPtr drawList = ImGui.GetBackgroundDrawList();

        _toolbarBackgroundElement.Render(drawList, new(0, _y));
        _toolbarLeftWidgetContainer.Render(drawList, new(0, _y));
        _toolbarCenterWidgetContainer.Render(drawList, new(0, _y));
        _toolbarRightWidgetContainer.Render(drawList, new(0, _y));
    }

    private void UpdateWidgetContainers()
    {
        _toolbarLeftWidgetContainer
            .Children.ToList()
            .ForEach(
                child => {
                    if (!child.Anchor.HasFlag(Anchor.Left)) {
                        _toolbarLeftWidgetContainer.RemoveChild(child);
                        RelocateWidget(child);
                    }
                }
            );

        _toolbarCenterWidgetContainer
            .Children.ToList()
            .ForEach(
                child => {
                    if (!child.Anchor.HasFlag(Anchor.Center)) {
                        _toolbarCenterWidgetContainer.RemoveChild(child);
                        RelocateWidget(child);
                    }
                }
            );

        _toolbarRightWidgetContainer
            .Children.ToList()
            .ForEach(
                child => {
                    if (!child.Anchor.HasFlag(Anchor.Right)) {
                        _toolbarRightWidgetContainer.RemoveChild(child);
                        RelocateWidget(child);
                    }
                }
            );
    }

    private void RelocateWidget(Element element)
    {
        if (element.Anchor.HasFlag(Anchor.Left))
            _toolbarLeftWidgetContainer.AddChild(element);
        else if (element.Anchor.HasFlag(Anchor.Center))
            _toolbarCenterWidgetContainer.AddChild(element);
        else if (element.Anchor.HasFlag(Anchor.Right)) _toolbarRightWidgetContainer.AddChild(element);
    }

    private void UpdateToolbarPosition()
    {
        _y = (int)(IsTopAligned ? ImGui.GetMainViewport().WorkPos.Y : ImGui.GetIO().DisplaySize.Y - Height);
    }

    private void UpdateToolbarSize()
    {
        var screenWidth = ImGui.GetIO().DisplaySize.X;
        var toolbarSize = new Size(screenWidth, Height);

        _toolbarBackgroundElement.Size               = toolbarSize;
        _toolbarBackgroundElement.Get("Border").Size = new(screenWidth, 1);

        // Update widget container sizes.
        _toolbarLeftWidgetContainer.Size   = toolbarSize;
        _toolbarCenterWidgetContainer.Size = toolbarSize;
        _toolbarRightWidgetContainer.Size  = toolbarSize;

        // Configure gradients & border based on alignment.
        _toolbarBackgroundElement.Get("Border").Anchor =
            IsTopAligned ? Anchor.Bottom | Anchor.Left : Anchor.Top | Anchor.Left;

        _toolbarBackgroundElement.GetNode<RectNode>().Gradients = IsTopAligned
            ? new(BottomColor, BottomColor, TopColor, TopColor)
            : new(TopColor, TopColor, BottomColor, BottomColor);
    }
}
