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
using Newtonsoft.Json;
using Umbra.Common;

namespace Umbra.Widgets.System;

[Service]
internal partial class WidgetManager(Toolbar toolbar)
{
    private readonly Dictionary<string, Type>          _widgetTypes = [];
    private readonly Dictionary<string, ToolbarWidget> _instances   = [];

    /// <summary>
    /// Registers a widget type with the given name.
    /// </summary>
    /// <param name="name">The name of the widget.</param>
    /// <typeparam name="TWidgetClass">The type of the widget.</typeparam>
    public void RegisterWidget<TWidgetClass>(string name) where TWidgetClass : ToolbarWidget
    {
        if (_widgetTypes.ContainsKey(name))
            throw new InvalidOperationException($"A widget with the name '{name}' is already registered.");

        _widgetTypes[name] = typeof(TWidgetClass);
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
    /// <exception cref="InvalidOperationException">If the widget does not exist.</exception>
    public void CreateWidget(
        string                      name,
        string                      location,
        int?                        sortIndex    = null,
        string?                     guid         = null,
        Dictionary<string, object>? configValues = null
    )
    {
        if (!_widgetTypes.TryGetValue(name, out var type))
            throw new InvalidOperationException($"No widget with the name '{name}' is registered.");

        var widget = (ToolbarWidget)Activator.CreateInstance(type, guid, configValues)!;
        var panel  = toolbar.GetPanel(location);

        _instances[widget.Id] = widget;

        widget.Setup();
        widget.OpenPopup        += OpenPopup;
        widget.OpenPopupDelayed += OpenPopupIfAnyIsOpen;
        widget.Node.SortIndex   =  sortIndex ?? panel.ChildNodes.Count;

        panel.AppendChild(widget.Node);
    }

    public void RemoveWidget(string id)
    {
        if (!_instances.TryGetValue(id, out var widget)) return;

        if (_currentActivator is not null && _currentActivator == widget) {
            ClosePopup();
        }

        widget.Node.Remove();
        widget.Dispose();
        widget.OpenPopup        -= OpenPopup;
        widget.OpenPopupDelayed -= OpenPopupIfAnyIsOpen;

        _instances.Remove(id);
    }

    public string DumpConfiguration()
    {
        Dictionary<string, WidgetConfig> result = [];

        foreach (var widget in _instances.Values) {
            result[widget.Id] = new() {
                Name      = widget.Name,
                SortIndex = widget.Node.SortIndex,
                Location  = widget.Node.ParentNode!.Id!,
                Config    = widget.GetUserConfig(),
            };
        }

        return JsonConvert.SerializeObject(
            result,
            new JsonSerializerSettings() {
                Formatting = Formatting.Indented,
            }
        );
    }

    private struct WidgetConfig
    {
        public string                     Name      { get; set; }
        public int                        SortIndex { get; set; }
        public string                     Location  { get; set; }
        public Dictionary<string, object> Config    { get; set; }
    }
}
