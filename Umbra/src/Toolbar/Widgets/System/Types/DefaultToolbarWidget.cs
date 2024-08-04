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

using System;
using System.Collections.Generic;
using Dalamud.Game.Text.SeStringHandling;
using System.Numerics;
using Umbra.Common;
using Umbra.Style;
using Una.Drawing;

namespace Umbra.Widgets;

public abstract class DefaultToolbarWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
)
    : ToolbarWidget(info, guid, configValues)
{
    public sealed override Node Node { get; } = new() {
        Stylesheet = WidgetStyles.DefaultWidgetStylesheet,
        ClassList  = ["toolbar-widget-default"],
        ChildNodes = [
            new() {
                Id          = "LeftIcon",
                ClassList   = ["icon"],
                InheritTags = true,
                Style = new() {
                    Margin    = new() { Left = -2 },
                    IsVisible = false
                }
            },
            new() {
                Id          = "Label",
                NodeValue   = "",
                InheritTags = true,
                ChildNodes = [
                    new() { Id = "TopLabel", InheritTags    = true, },
                    new() { Id = "BottomLabel", InheritTags = true, }
                ],
                Style = new() {
                    Padding = new(),
                }
            },
            new() {
                Id          = "RightIcon",
                ClassList   = ["icon"],
                InheritTags = true,
                Style = new() {
                    Margin    = new() { Right = -2 },
                    IsVisible = false
                }
            },
        ],
        BeforeDraw = node => {
            node.Style.Size = new(0, SafeHeight);

            Node leftIconNode    = node.QuerySelector("#LeftIcon")!;
            Node rightIconNode   = node.QuerySelector("#RightIcon")!;
            Node labelNode       = node.QuerySelector("#Label")!;
            Node topLabelNode    = node.QuerySelector("#TopLabel")!;
            Node bottomLabelNode = node.QuerySelector("#BottomLabel")!;

            leftIconNode.Style.IsVisible  = leftIconNode.Style.IconId is not null;
            rightIconNode.Style.IsVisible = rightIconNode.Style.IconId is not null;

            var halfSize = (int)Math.Ceiling(SafeHeight / 2f);

            labelNode.Style.Size           = new(labelNode.Style.Size?.Width ?? 0, SafeHeight);
            labelNode.Style.FontSize       = (halfSize / 2) + 6;
            topLabelNode.Style.Size        = new(topLabelNode.Style.Size?.Width ?? 0, halfSize - 2);
            topLabelNode.Style.FontSize    = (halfSize / 2) + 4;
            bottomLabelNode.Style.Size     = new(bottomLabelNode.Style.Size?.Width ?? 0, halfSize - 2);
            bottomLabelNode.Style.FontSize = (halfSize / 2) + 2;
        }
    };

    private uint? _singleIconId;

    protected override void OnUpdate()
    {
        bool isGhost = !GetConfigValue<bool>("Decorate");

        SetGhost(isGhost);
        SetLabelWidth(GetConfigValue<int>("LabelMaxWidth"));
        SetIconSize(GetConfigValue<int>("IconSize"));

        var displayMode    = GetConfigValue<string>("DisplayMode");
        var iconLocation   = GetConfigValue<string>("IconLocation");
        var desaturateIcon = GetConfigValue<bool>("DesaturateIcon");
        var iconOffset     = new Vector2(0, GetConfigValue<int>("IconYOffset"));

        if (null != _singleIconId) {
            if (displayMode is "TextAndIcon" or "IconOnly") {
                switch (iconLocation) {
                    case "Left":
                        SetLeftIcon(_singleIconId);
                        SetRightIcon(null);
                        break;
                    case "Right":
                        SetLeftIcon(null);
                        SetRightIcon(_singleIconId);
                        break;
                }
            } else {
                SetLeftIcon(null);
                SetRightIcon(null);
            }
        }

        switch (GetConfigValue<string>("TextAlign")) {
            case "Left":
                SetTextAlignLeft();
                break;
            case "Right":
                SetTextAlignRight();
                break;
            case "Center":
                SetTextAlignCenter();
                break;
        }

        if (HasConfigVariable("TextYOffset")) {
            LabelNode.Style.TextOffset = new Vector2(0, GetConfigValue<int>("TextYOffset"));
        }

        if (HasConfigVariable("TextYOffsetTop")) {
            TopLabelNode.Style.TextOffset = new Vector2(0, GetConfigValue<int>("TextYOffsetTop"));
        }

        if (HasConfigVariable("TextYOffsetBottom")) {
            BottomLabelNode.Style.TextOffset = new Vector2(0, GetConfigValue<int>("TextYOffsetBottom"));
        }

        bool hasLabel = !string.IsNullOrEmpty(LabelNode.NodeValue?.ToString())
            || !string.IsNullOrEmpty(TopLabelNode.NodeValue?.ToString())
            || !string.IsNullOrEmpty(BottomLabelNode.NodeValue?.ToString());

        bool liv = LeftIconNode.Style.IsVisible ?? false;
        bool riv = RightIconNode.Style.IsVisible ?? false;

        LabelNode.Style.IsVisible          = displayMode is not "IconOnly" && hasLabel;
        LeftIconNode.Style.ImageOffset     = iconOffset;
        RightIconNode.Style.ImageOffset    = iconOffset;
        LeftIconNode.Style.ImageGrayscale  = Node.IsDisabled || desaturateIcon;
        RightIconNode.Style.ImageGrayscale = Node.IsDisabled || desaturateIcon;
        LeftIconNode.Style.Margin          = new(0);
        RightIconNode.Style.Margin         = new(0);
        LabelNode.Style.Padding            = new(0, riv ? 0 : 5, 0, liv ? 0 : 5);
        TopLabelNode.Style.Padding         = new(0, 1);
        BottomLabelNode.Style.Padding      = new(0, 1);
        Node.Style.Padding                 = new(0, isGhost ? 0 : 3);
        Node.Tooltip                       = displayMode is "IconOnly" ? LabelNode.NodeValue?.ToString() : null;
    }

    protected void SetGhost(bool isGhost)
    {
        switch (isGhost) {
            case true when !Node.TagsList.Contains("ghost"):
                Node.TagsList.Add("ghost");
                break;
            case false when Node.TagsList.Contains("ghost"):
                Node.TagsList.Remove("ghost");
                break;
        }
    }

    protected void SetIcon(uint? iconId)
    {
        if (iconId == 0) iconId = null;

        _singleIconId = iconId;

        if (null == iconId) {
            LeftIconNode.Style.IsVisible  = false;
            RightIconNode.Style.IsVisible = false;
            LeftIconNode.Style.IconId     = null;
            RightIconNode.Style.IconId    = null;
        }
    }

    protected void SetLeftIcon(uint? iconId)
    {
        LeftIconNode.Style.IsVisible = iconId != null;
        LeftIconNode.Style.IconId    = iconId;
    }

    protected void SetRightIcon(uint? iconId)
    {
        RightIconNode.Style.IsVisible = iconId != null;
        RightIconNode.Style.IconId    = iconId;
    }

    protected void SetIconSize(int size)
    {
        Size sz = size > 0 ? new(size, size) : new(SafeHeight - 6, SafeHeight - 6);

        LeftIconNode.Style.FontSize  = size > 0 ? size : (SafeHeight - 2) / 2;
        RightIconNode.Style.FontSize = size > 0 ? size : (SafeHeight - 2) / 2;
        LeftIconNode.Style.Size      = sz;
        RightIconNode.Style.Size     = sz;
    }

    protected void SetLabel(string? label)
    {
        LabelNode.NodeValue       = label;
        LabelNode.Style.IsVisible = !string.IsNullOrEmpty(label);

        TopLabelNode.NodeValue          = null;
        BottomLabelNode.NodeValue       = null;
        TopLabelNode.Style.IsVisible    = false;
        BottomLabelNode.Style.IsVisible = false;
    }

    protected void SetLabel(SeString? label)
    {
        LabelNode.NodeValue = label;

        TopLabelNode.NodeValue          = null;
        BottomLabelNode.NodeValue       = null;
        TopLabelNode.Style.IsVisible    = false;
        BottomLabelNode.Style.IsVisible = false;
    }

    protected void SetTwoLabels(string? topLabel, string? bottomLabel)
    {
        LabelNode.NodeValue = null;

        TopLabelNode.NodeValue    = topLabel;
        BottomLabelNode.NodeValue = bottomLabel;

        TopLabelNode.Style.IsVisible    = !string.IsNullOrEmpty(topLabel);
        BottomLabelNode.Style.IsVisible = !string.IsNullOrEmpty(bottomLabel);
        LabelNode.Style.IsVisible       = !string.IsNullOrEmpty(topLabel) || !string.IsNullOrEmpty(bottomLabel);
    }

    protected void SetLabelWidth(int maxWidth)
    {
        if (null == LabelNode.Style.Size) {
            LabelNode.Style.Size       = new(maxWidth, SafeHeight);
            TopLabelNode.Style.Size    = new(maxWidth, SafeHeight);
            BottomLabelNode.Style.Size = new(maxWidth, SafeHeight);
            return;
        }

        LabelNode.Style.Size!.Width       = maxWidth;
        TopLabelNode.Style.Size!.Width    = maxWidth;
        BottomLabelNode.Style.Size!.Width = maxWidth;
    }

    protected void SetDisabled(bool isDisabled)
    {
        Node.IsDisabled    = isDisabled;
        Node.Style.Opacity = isDisabled ? .66f : 1f;

        Node.QuerySelector("LeftIcon")!.Style.ImageGrayscale  = isDisabled;
        Node.QuerySelector("RightIcon")!.Style.ImageGrayscale = isDisabled;
    }

    protected void SetTextAlignLeft()
    {
        LabelNode.Style.TextAlign       = Anchor.MiddleLeft;
        TopLabelNode.Style.TextAlign    = Anchor.MiddleLeft;
        BottomLabelNode.Style.TextAlign = Anchor.MiddleLeft;
    }

    protected void SetTextAlignRight()
    {
        LabelNode.Style.TextAlign       = Anchor.MiddleRight;
        TopLabelNode.Style.TextAlign    = Anchor.MiddleRight;
        BottomLabelNode.Style.TextAlign = Anchor.MiddleRight;
    }

    protected void SetTextAlignCenter()
    {
        LabelNode.Style.TextAlign       = Anchor.MiddleCenter;
        TopLabelNode.Style.TextAlign    = Anchor.MiddleCenter;
        BottomLabelNode.Style.TextAlign = Anchor.MiddleCenter;
    }

    protected Node LabelNode       => Node.QuerySelector("#Label")!;
    protected Node TopLabelNode    => Node.QuerySelector("#TopLabel")!;
    protected Node BottomLabelNode => Node.QuerySelector("#BottomLabel")!;
    protected Node LeftIconNode    => Node.QuerySelector("#LeftIcon")!;
    protected Node RightIconNode   => Node.QuerySelector("#RightIcon")!;

    protected IList<IWidgetConfigVariable> DefaultToolbarWidgetConfigVariables { get; } = [
        new BooleanWidgetConfigVariable(
            "Decorate",
            I18N.Translate("Widgets.DefaultToolbarWidget.Config.Decorate.Name"),
            I18N.Translate("Widgets.DefaultToolbarWidget.Config.Decorate.Description"),
            true
        ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
        new BooleanWidgetConfigVariable(
            "DesaturateIcon",
            I18N.Translate("Widgets.DefaultToolbarWidget.Config.DesaturateIcon.Name"),
            I18N.Translate("Widgets.DefaultToolbarWidget.Config.DesaturateIcon.Description"),
            false
        ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
        new SelectWidgetConfigVariable(
            "DisplayMode",
            I18N.Translate("Widgets.DefaultToolbarWidget.Config.DisplayMode.Name"),
            I18N.Translate("Widgets.DefaultToolbarWidget.Config.DisplayMode.Description"),
            "TextAndIcon",
            new() {
                { "TextAndIcon", I18N.Translate("Widgets.DefaultToolbarWidget.Config.DisplayMode.Option.TextAndIcon") },
                { "TextOnly", I18N.Translate("Widgets.DefaultToolbarWidget.Config.DisplayMode.Option.TextOnly") },
                { "IconOnly", I18N.Translate("Widgets.DefaultToolbarWidget.Config.DisplayMode.Option.IconOnly") }
            }
        ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
        new SelectWidgetConfigVariable(
            "IconLocation",
            I18N.Translate("Widgets.DefaultToolbarWidget.Config.IconLocation.Name"),
            I18N.Translate("Widgets.DefaultToolbarWidget.Config.IconLocation.Description"),
            "Left",
            new() {
                { "Left", I18N.Translate("Widgets.DefaultToolbarWidget.Config.IconLocation.Option.Left") },
                { "Right", I18N.Translate("Widgets.DefaultToolbarWidget.Config.IconLocation.Option.Right") }
            }
        ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
        new SelectWidgetConfigVariable(
            "TextAlign",
            I18N.Translate("Widgets.DefaultToolbarWidget.Config.TextAlign.Name"),
            I18N.Translate("Widgets.DefaultToolbarWidget.Config.TextAlign.Description"),
            "Left",
            new() {
                { "Left", I18N.Translate("Widgets.DefaultToolbarWidget.Config.TextAlign.Option.Left") },
                { "Center", I18N.Translate("Widgets.DefaultToolbarWidget.Config.TextAlign.Option.Center") },
                { "Right", I18N.Translate("Widgets.DefaultToolbarWidget.Config.TextAlign.Option.Right") }
            }
        ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
        new IntegerWidgetConfigVariable(
            "IconSize",
            I18N.Translate("Widgets.DefaultToolbarWidget.Config.IconSize.Name"),
            I18N.Translate("Widgets.DefaultToolbarWidget.Config.IconSize.Description"),
            0,
            0,
            42
        ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
        new IntegerWidgetConfigVariable(
            "IconYOffset",
            I18N.Translate("Widgets.DefaultToolbarWidget.Config.IconYOffset.Name"),
            I18N.Translate("Widgets.DefaultToolbarWidget.Config.IconYOffset.Description"),
            0,
            -5,
            5
        ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
        new IntegerWidgetConfigVariable(
            "LabelMaxWidth",
            I18N.Translate("Widgets.DefaultToolbarWidget.Config.LabelMaxWidth.Name"),
            I18N.Translate("Widgets.DefaultToolbarWidget.Config.LabelMaxWidth.Description"),
            0,
            0,
            500
        ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
    ];

    protected IList<IWidgetConfigVariable> SingleLabelTextOffsetVariables { get; } = [
        new IntegerWidgetConfigVariable(
            "TextYOffset",
            I18N.Translate("Widgets.DefaultToolbarWidget.Config.TextYOffset.Name"),
            I18N.Translate("Widgets.DefaultToolbarWidget.Config.TextYOffset.Description"),
            0,
            -5,
            5
        ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
    ];

    protected IList<IWidgetConfigVariable> TwoLabelTextOffsetVariables { get; } = [
        new IntegerWidgetConfigVariable(
            "TextYOffsetTop",
            I18N.Translate("Widgets.DefaultToolbarWidget.Config.TextYOffsetTop.Name"),
            I18N.Translate("Widgets.DefaultToolbarWidget.Config.TextYOffsetTop.Description"),
            1,
            -5,
            5
        ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
        new IntegerWidgetConfigVariable(
            "TextYOffsetBottom",
            I18N.Translate("Widgets.DefaultToolbarWidget.Config.TextYOffsetBottom.Name"),
            I18N.Translate("Widgets.DefaultToolbarWidget.Config.TextYOffsetBottom.Description"),
            -1,
            -5,
            5
        ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
    ];
}
