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
using Una.Drawing;

namespace Umbra.Widgets;

public abstract class ToolbarWidget(string? guid = null, Dictionary<string, object>? configValues = null) : IDisposable
{
    internal event Action<ToolbarWidget, WidgetPopup>? OpenPopup;
    internal event Action<ToolbarWidget, WidgetPopup>? OpenPopupDelayed;

    /// <summary>
    /// The unique identifier of this widget instance.
    /// </summary>
    public string Id { get; } = guid ?? Guid.NewGuid().ToString();

    /// <summary>
    /// <para>
    /// Defines the name of this widget.
    /// </para>
    /// <para>
    /// Please note that this name is used to identify the widget and should
    /// be unique among all widgets.
    /// </para>
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Defines the node of this widget.
    /// </summary>
    public abstract Node Node { get; }

    /// <summary>
    /// Defines the popup of this widget. Setting a value will make the widget
    /// interactive and will render the popup node when the widget is clicked.
    /// </summary>
    public abstract WidgetPopup? Popup { get; }

    private readonly Dictionary<string, IWidgetConfigVariable> _configVariables = new();
    private readonly Dictionary<string, object>                _configValues    = configValues ?? [];

    public void Setup()
    {
        foreach (var cfg in GetConfigVariables()) {
            _configVariables[cfg.Name] = cfg;

            if (cfg is IUntypedWidgetConfigVariable u) {
                if (false == _configValues.ContainsKey(cfg.Name)) {
                    _configValues[cfg.Name] = u.GetDefaultValue()!;
                }

                u.UntypedValueChanged += value => _configValues[cfg.Name] = value;
            }
        }

        if (_configValues.Count == 0) return;

        List<string> keysToRemove = [];

        foreach ((string key, object value) in _configValues) {
            if (!_configVariables.ContainsKey(key)) {
                keysToRemove.Add(key);
                continue;
            }

            var cfg = _configVariables[key];

            if (cfg is IUntypedWidgetConfigVariable u) {
                u.SetValue(value);
            }
        }

        foreach (string key in keysToRemove) {
            _configValues.Remove(key);
        }

        Initialize();

        if (Popup is null) return;

        Node.OnClick             += _ => OpenPopup?.Invoke(this, Popup);
        Node.OnDelayedMouseEnter += _ => OpenPopupDelayed?.Invoke(this, Popup);
    }

    public void Update()
    {
        OnUpdate();
    }

    /// <summary>
    /// Returns a dictionary of configuration values for this widget instance.
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, object> GetUserConfig()
    {
        return _configValues;
    }

    /// <inheritdoc/>
    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Initialization method that is called when the widget is created and
    /// initial configuration data has been set.
    /// </summary>
    protected abstract void Initialize();

    /// <summary>
    /// Invoked on every frame, just before the widget is rendered.
    /// </summary>
    protected abstract void OnUpdate();

    /// <summary>
    /// Returns a list of configuration variables for this widget that the user
    /// can modify.
    /// </summary>
    protected abstract IEnumerable<IWidgetConfigVariable> GetConfigVariables();

    protected T GetConfigValue<T>(string name)
    {
        if (!_configVariables.TryGetValue(name, out var cfg)) {
            throw new InvalidOperationException($"No config variable with the name '{name}' exists.");
        }

        if (cfg is not WidgetConfigVariable<T> c) {
            throw new InvalidOperationException($"Config variable '{name}' is not of type '{typeof(T).Name}'.");
        }

        return c.Value;
    }
}
