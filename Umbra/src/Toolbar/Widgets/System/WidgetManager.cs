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

using ImGuiNET;
using Lumina.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;
using Umbra.Game;
using Umbra.Windows;
using Umbra.Windows.Library.WidgetConfig;
using Una.Drawing;

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
    private IPlayer Player  { get; }

    private byte          _lastJobId;
    private bool          _quickAccessEnabled;
    private HashSet<Node> _subscribedQuickAccessNodes = [];

    public WidgetManager(Toolbar toolbar, IPlayer player)
    {
        Toolbar = toolbar;
        Player  = player;

        foreach ((Type type, WidgetInfo info) in WidgetRegistry.RegisteredWidgets) {
            if (type.IsSubclassOf(typeof(ToolbarWidget))) {
                _widgetTypes[info.Id] = type;
                _widgetInfos[info.Id] = info;
            }
        }

        if (!_widgetProfiles.ContainsKey("Default") == false) {
            _widgetProfiles["Default"] = WidgetConfigData;
            SaveProfileData();
        }

        ConfigManager.CvarChanged += OnCvarChanged;
        LoadProfileData();
        LoadState();
    }

    public void Dispose()
    {
        ConfigManager.CvarChanged -= OnCvarChanged;

        foreach (var node in _subscribedQuickAccessNodes) {
            node.OnRightClick -= InvokeInstanceQuickSettings;
        }

        _subscribedQuickAccessNodes.Clear();

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
        widget.Node.Id ??= $"UmbraWidget_{Crc32.Get(widget.Id)}";

        widget.Setup();
        widget.OpenPopup        += OpenPopup;
        widget.OpenPopupDelayed += OpenPopupIfAnyIsOpen;

        if (EnableQuickSettingAccess && _subscribedQuickAccessNodes.Add(widget.Node)) {
            widget.Node.OnRightClick += InvokeInstanceQuickSettings;
        }

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

        if (_subscribedQuickAccessNodes.Remove(widget.Node)) {
            widget.Node.OnRightClick -= InvokeInstanceQuickSettings;
        }

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

        lock (_widgetState) {
            _widgetState.Remove(guid);
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
            SaveState();
        }

        OnWidgetRemoved?.Invoke(widget);
    }

    public void CreateCopyOfWidget(string id)
    {
        Framework.DalamudFramework.Run(
            () => {
                lock (_widgetState) {
                    if (!_instances.TryGetValue(id, out var widget)) return;
                    if (!_widgetState.TryGetValue(id, out var config)) return;

                    // Generate a new GUID for the copied widget.
                    string newGuid = Guid.NewGuid().ToString();

                    // Create a new widget instance with the same configuration as the original.
                    CreateWidget(widget.Info.Id, widget.Location, widget.SortIndex, newGuid, new(config.Config));

                    // Since we're copying the widget to the same place, we need to solve the sort indices.
                    SolveSortIndices(widget.Location);

                    SaveState();
                }
            }
        );
    }

    public void UpdateWidgetSortIndex(string id, int direction, bool allTheWay)
    {
        if (!_instances.TryGetValue(id, out var widget)) return;

        if (false == allTheWay) {
            // Swap the sort index of the widget with the one above or below it.
            ToolbarWidget?replacementWidget = _instances.Values.FirstOrDefault(
                w => w.Location == widget.Location && w.SortIndex == widget.SortIndex + direction
            );

            if (null == replacementWidget) return;

            replacementWidget.SortIndex -= direction;
            widget.SortIndex            += direction;

            SolveSortIndices(widget.Location);
            SaveWidgetState(replacementWidget.Id);
            SaveWidgetState(widget.Id);
        } else {
            widget.SortIndex = direction == -1 ? int.MinValue : int.MaxValue;
            SolveSortIndices(widget.Location);
            SaveWidgetState(widget.Id);
        }

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
            if (children[i].SortIndex == i) continue;
            children[i].SortIndex = i;
            SaveWidgetState(children[i].Id);
        }
    }

    [OnTick(interval: 16)]
    private void OnUpdateWidgets()
    {
        byte jobId = Player.JobId;

        if (UseJobAssociatedProfiles && jobId != _lastJobId && ActiveProfile != JobToProfileName[jobId]) {
            _lastJobId = jobId;
            ActivateProfile(JobToProfileName[jobId]);
            return;
        }

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

                widget.Update();

                widget.Node.SortIndex  = widget.SortIndex;
                widget.Node.IsDisabled = Player.IsEditingHud;
            }
        }
    }

    private void ToggleQuickAccessBindings()
    {
        if (_quickAccessEnabled && !EnableQuickSettingAccess) {
            foreach (var instance in _instances.Values) {
                if (_subscribedQuickAccessNodes.Remove(instance.Node)) {
                    instance.Node.OnRightClick -= InvokeInstanceQuickSettings;
                }
            }
            _quickAccessEnabled = false;
            return;
        }

        foreach (var instance in _instances.Values) {
            if (_subscribedQuickAccessNodes.Add(instance.Node)) {
                instance.Node.OnRightClick += InvokeInstanceQuickSettings;
            }
        }

        _quickAccessEnabled = true;
    }

    private void InvokeInstanceQuickSettings(Node node)
    {
        if (node.ParentNode is null) return;
        if (!(ImGui.GetIO().KeyCtrl && ImGui.GetIO().KeyShift)) return;

        foreach ((string guid, ToolbarWidget widget) in _instances) {
            if (widget.Node == node) {
                node.CancelEvent = true;
                Framework.Service<WindowManager>().Present("WidgetInstanceConfig",
                    new WidgetConfigWindow(guid),
                    _ => {
                        SaveWidgetState(widget.Id);
                        SaveState();
                    });
                break;
            }
        }
    }

    private void OnCvarChanged(string name)
    {
        if (name == "Toolbar.WidgetData") LoadState();
        if (name == "Toolbar.ProfileData") LoadProfileData();
        if (name == "Toolbar.EnableQuickSettingAccess") ToggleQuickAccessBindings();
    }

    private struct WidgetConfigStruct
    {
        public string                     Name      { get; init; }
        public int                        SortIndex { get; init; }
        public string                     Location  { get; init; }
        public Dictionary<string, object> Config    { get; init; }
    }
}
