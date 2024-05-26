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
using System.Linq;
using Umbra.Common;

namespace Umbra.Widgets.System;

[Service]
internal sealed partial class WidgetManager : IDisposable
{
    public event Action<ToolbarWidget>?         OnWidgetCreated;
    public event Action<ToolbarWidget>?         OnWidgetRemoved;
    public event Action<ToolbarWidget, string>? OnWidgetRelocated;

    private readonly Dictionary<string, Type>          _widgetTypes = [];
    private readonly Dictionary<string, WidgetInfo>    _widgetInfos = [];
    private readonly Dictionary<string, ToolbarWidget> _instances   = [];

    private Toolbar Toolbar { get; }

    public WidgetManager(Toolbar toolbar)
    {
        Toolbar = toolbar;

        ConfigManager.CvarChanged += OnCvarChanged;
        LoadState();
    }

    public void Dispose()
    {
        ConfigManager.CvarChanged -= OnCvarChanged;

        foreach (var widget in _instances.Values) {
            widget.Dispose();

            if (widget.Popup is IDisposable disposable) {
                disposable.Dispose();
            }
        }
    }

    /// <summary>
    /// Registers a widget type with the given name.
    /// </summary>
    /// <param name="info">An object containing information about the widget.</param>
    /// <typeparam name="TWidgetClass">The type of the widget.</typeparam>
    public void RegisterWidget<TWidgetClass>(WidgetInfo info) where TWidgetClass : ToolbarWidget
    {
        if (_widgetTypes.ContainsKey(info.Id))
            throw new InvalidOperationException($"A widget with the name '{info.Id}' is already registered.");

        _widgetTypes[info.Id] = typeof(TWidgetClass);
        _widgetInfos[info.Id] = info;

        LoadState();
    }

    public void UnregisterWidget(string name)
    {
        if (!_widgetTypes.ContainsKey(name)) return;

        _widgetTypes.Remove(name);
        _widgetInfos.Remove(name);

        foreach (var widget in _instances.Values.Where(w => w.Info.Id == name).ToList()) {
            RemoveWidget(widget.Id, false);
        }
    }

    /// <summary>
    /// Returns an instance of a widget with the given GUID.
    /// </summary>
    public ToolbarWidget GetInstance(string guid)
    {
        return _instances[guid];
    }

    /// <summary>
    /// Returns a list of <see cref="WidgetInfo"/> objects of all registered
    /// widgets.
    /// </summary>
    /// <returns></returns>
    public List<WidgetInfo> GetWidgetInfoList()
    {
        return _widgetInfos.Values.ToList();
    }

    public List<ToolbarWidget> GetWidgetInstances()
    {
        return _instances.Values.ToList();
    }

    /// <summary>
    /// Creates a widget instance with the given name, location, and
    /// configuration values.
    /// </summary>
    /// <param name="name">Which widget to create.</param>
    /// <param name="location">The location of the widget in the toolbar.</param>
    /// <param name="sortIndex">The sort index of this instance.</param>
    /// <param name="guid">An instance GUID used for user config mapping.</param>
    /// <param name="configValues">A dictionary of config values for this instance.</param>
    /// <param name="saveState">Whether to save the state of the widgets configuration.</param>
    /// <exception cref="InvalidOperationException">If the widget does not exist.</exception>
    public void CreateWidget(
        string                      name,
        string                      location,
        int?                        sortIndex    = null,
        string?                     guid         = null,
        Dictionary<string, object>? configValues = null,
        bool                        saveState    = true
    )
    {
        if (!_widgetTypes.TryGetValue(name, out var type))
            throw new InvalidOperationException($"No widget with the name '{name}' is registered.");

        if (!_widgetInfos.TryGetValue(name, out var info))
            throw new InvalidOperationException($"No widget info for the widget '{name}' is available.");

        var widget = (ToolbarWidget)Activator.CreateInstance(type, info, guid, configValues)!;
        var panel  = Toolbar.GetPanel(location);

        _instances[widget.Id] = widget;

        widget.SortIndex = sortIndex ?? panel.ChildNodes.Count;
        widget.Location  = location;

        widget.Setup();
        widget.OpenPopup        += OpenPopup;
        widget.OpenPopupDelayed += OpenPopupIfAnyIsOpen;

        panel.AppendChild(widget.Node);
        SolveSortIndices(widget.Location);

        OnWidgetCreated?.Invoke(widget);

        SaveWidgetState(widget.Id);
        if (saveState) SaveState();
    }

    public void RemoveWidget(string guid, bool saveState = true)
    {
        if (!_instances.TryGetValue(guid, out var widget)) return;

        if (_currentActivator is not null && _currentActivator == widget) {
            ClosePopup();
        }

        Framework.DalamudFramework.Run(
            () => {
                widget.Node.Remove();
                widget.Dispose();
                widget.OpenPopup        -= OpenPopup;
                widget.OpenPopupDelayed -= OpenPopupIfAnyIsOpen;

                if (widget.Popup is IDisposable disposable) {
                    disposable.Dispose();
                }

                lock (_instances) {
                    _instances.Remove(guid);
                }

                int sortIndexStart = widget.SortIndex;

                var instances = _instances
                    .Values
                    .Where(w => w.Location == widget.Location && w.SortIndex > sortIndexStart)
                    .ToList();

                if (instances.Count > 0) {
                    foreach (var w in instances) {
                        w.SortIndex--;
                        SaveWidgetState(w.Id);
                    }
                }

                if (saveState) {
                    _widgetState.Remove(guid);
                    SaveState();
                }

                OnWidgetRemoved?.Invoke(widget);
            }
        );
    }

    public void UpdateWidgetSortIndex(string id, int direction)
    {
        if (!_instances.TryGetValue(id, out var widget)) return;

        // Swap the sort index of the widget with the one above or below it.
        var replacementWidget = _instances.Values.FirstOrDefault(
            w => w.Location == widget.Location && w.SortIndex == widget.SortIndex + direction
        );

        if (null == replacementWidget) return;

        replacementWidget.SortIndex -= direction;
        widget.SortIndex            += direction;

        SolveSortIndices(widget.Location);
        SaveWidgetState(widget.Id);
        SaveWidgetState(replacementWidget.Id);
        SaveState();
    }

    /// <summary>
    /// Updates the sort indices of all widgets to ensure there are no gaps.
    /// </summary>
    /// <param name="location"></param>
    private void SolveSortIndices(string location)
    {
        if (_isLoadingState) return;

        List<ToolbarWidget> children = _instances
            .Values
            .Where(w => w.Node.ParentNode!.Id == location)
            .OrderBy(w => w.SortIndex)
            .ToList();

        for (var i = 0; i < children.Count; i++) {
            children[i].SortIndex = i;
        }
    }

    [OnTick]
    private void OnUpdateWidgets()
    {
        lock (_instances) {
            foreach (var widget in _instances.Values) {
                if (widget.Node.ParentNode is null) continue;

                string panelId = widget.Node.ParentNode!.Id!;

                if (widget.Location != panelId) {
                    Toolbar.GetPanel(widget.Location).AppendChild(widget.Node);
                    SolveSortIndices(widget.Location);
                    SolveSortIndices(panelId);

                    SaveWidgetState(widget.Id);
                    SaveState();
                    OnWidgetRelocated?.Invoke(widget, panelId);
                }

                widget.Node.SortIndex = widget.SortIndex;
                widget.Update();
            }
        }
    }

    private void OnCvarChanged(string name)
    {
        if (name == "Toolbar.WidgetData") LoadState();
    }

    private struct WidgetConfigStruct
    {
        public string                     Name      { get; init; }
        public int                        SortIndex { get; init; }
        public string                     Location  { get; init; }
        public Dictionary<string, object> Config    { get; init; }
    }
}
