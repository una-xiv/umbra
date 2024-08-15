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

using Dalamud.Game.Text.SeStringHandling;
using System.Collections.Generic;
using Dalamud.Plugin.Services;
using Umbra.Common;
using Umbra.Game;
using Una.Drawing;

namespace Umbra.Widgets;

[ToolbarWidget("DtrBar", "Widget.DtrBar.Name", "Widget.DtrBar.Description")]
internal sealed partial class DtrBarWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
)
    : ToolbarWidget(info, guid, configValues)
{
    private IDtrBarEntryRepository? _repository;
    private IGameGui?               _gameGui;

    private readonly Dictionary<string, Node> _entries = [];

    /// <inheritdoc/>
    protected override void Initialize()
    {
        _gameGui    = Framework.Service<IGameGui>();
        _repository = Framework.Service<IDtrBarEntryRepository>();

        _repository.OnEntryAdded   += OnDtrBarEntryAdded;
        _repository.OnEntryRemoved += OnDtrBarEntryRemoved;
        _repository.OnEntryUpdated += OnDtrBarEntryUpdated;

        foreach (var entry in _repository.GetEntries()) OnDtrBarEntryAdded(entry);
    }

    /// <inheritdoc/>
    protected override void OnUpdate()
    {
        UpdateNativeServerInfoBar();

        var decorateMode = GetConfigValue<string>("DecorateMode");
        var textOffset   = GetConfigValue<int>("TextYOffset");

        Node.Style.Gap  = GetConfigValue<int>("ItemSpacing");
        Node.Style.Size = new(0, SafeHeight);

        foreach ((string id, Node node) in _entries) {
            switch (decorateMode) {
                case "Always":
                    node.TagsList.Remove("ghost");
                    break;
                case "Never":
                    if (!node.TagsList.Contains("ghost")) node.TagsList.Add("ghost");
                    break;
                case "Auto" when node.IsInteractive:
                    node.TagsList.Remove("ghost");
                    break;
                case "Auto" when !node.IsInteractive:
                    if (!node.TagsList.Contains("ghost")) node.TagsList.Add("ghost");
                    break;
            }

            node.Style.Size = new(0, SafeHeight);

            var entry     = _repository!.Get(id);
            var labelNode = node.FindById("Label");

            if (null != labelNode && entry is { IsVisible: true }) {
                SetNodeLabel(node, entry);
                labelNode.Style.MaxWidth   = MaxTextWidth;
                labelNode.Style.TextOffset = new(0, textOffset);
                labelNode.Style.FontSize   = GetConfigValue<int>("TextSize");
            }
        }

        switch (GetConfigValue<string>("DecorateMode")) {
            case "Always":
                foreach (Node node in Node.QuerySelectorAll(".dtr-bar-entry")) {
                    node.TagsList.Remove("ghost");
                }

                break;
            case "Never":
                foreach (Node node in Node.QuerySelectorAll(".dtr-bar-entry")) {
                    if (!node.TagsList.Contains("ghost")) node.TagsList.Add("ghost");
                }

                break;
            case "Auto":
                foreach (Node node in Node.QuerySelectorAll(".dtr-bar-entry")) {
                    if (node.IsInteractive) {
                        node.TagsList.Remove("ghost");
                    } else {
                        if (!node.TagsList.Contains("ghost")) node.TagsList.Add("ghost");
                    }
                }

                break;
        }
    }

    /// <inheritdoc/>
    protected override void OnDisposed()
    {
        if (_repository is not null) {
            _repository.OnEntryAdded   -= OnDtrBarEntryAdded;
            _repository.OnEntryRemoved -= OnDtrBarEntryRemoved;
            _repository.OnEntryUpdated -= OnDtrBarEntryUpdated;
        }

        SetNativeServerInfoBarVisibility(true);
    }

    private void OnDtrBarEntryAdded(DtrBarEntry entry)
    {
        if (_entries.ContainsKey(entry.Name)) {
            OnDtrBarEntryUpdated(entry);
            return;
        }

        Node node = new() {
            ClassList = ["toolbar-widget-default", "dtr-bar-entry"],
            SortIndex = entry.SortIndex,
            Style = new() {
                Anchor = Anchor.MiddleRight
            },
            ChildNodes = [
                new() {
                    Id          = "Label",
                    NodeValue   = entry.Text,
                    InheritTags = true,
                    Style = new() {
                        MaxWidth     = MaxTextWidth,
                        WordWrap     = false,
                        TextOverflow = false,
                    }
                }
            ]
        };

        if (entry.IsInteractive) {
            node.Tooltip =  entry.TooltipText?.TextValue;
            node.OnClick += _ => entry.InvokeClickAction();
        }

        _entries.Add(entry.Name, node);

        Node.AppendChild(node);
    }

    private void OnDtrBarEntryRemoved(DtrBarEntry entry)
    {
        if (!_entries.TryGetValue(entry.Name, out Node? node)) return;

        node.Remove();

        _entries.Remove(entry.Name);
    }

    private void OnDtrBarEntryUpdated(DtrBarEntry entry)
    {
        if (!_entries.TryGetValue(entry.Name, out Node? node)) return;

        if (node.Style.IsVisible != entry.IsVisible) {
            node.Style.IsVisible = entry.IsVisible;
        }

        if (entry.IsVisible) {
            SetNodeLabel(node, entry);
        }

        node.Tooltip   = entry.TooltipText?.TextValue;
        node.SortIndex = entry.SortIndex;
    }

    private void SetNodeLabel(Node node, DtrBarEntry entry)
    {
        var labelNode = node.FindById("Label");
        if (labelNode == null) return;

        labelNode.NodeValue = GetConfigValue<bool>("PlainText")
            ? entry.Text?.TextValue ?? ""
            : entry.Text;
    }

    private int? MaxTextWidth => GetConfigValue<int>("MaxTextWidth") switch {
        0 => null,
        _ => GetConfigValue<int>("MaxTextWidth")
    };
}
