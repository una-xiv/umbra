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
using Umbra.Game;
using Umbra.Markers.System;

namespace Umbra.Markers;

public abstract class WorldMarkerFactory : IDisposable
{
    /// <summary>
    /// A unique ID that identifies the marker types this factory produces.
    /// </summary>
    public abstract string Id { get; }

    /// <summary>
    /// The display name of the world marker type this factory produces. This
    /// should be a translated string.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// A description of the world marker types this factory produces. This
    /// should be a translated string.
    /// </summary>
    public abstract string Description { get; }

    /// <summary>
    /// Returns a list of config variables that the user can use to customize
    /// the behavior of the created world markers.
    /// </summary>
    public abstract List<IMarkerConfigVariable> GetConfigVariables();

    public event Action? OnConfigValueChanged;

    protected readonly Dictionary<string, WorldMarker>           Markers          = [];
    private readonly   Dictionary<string, IMarkerConfigVariable> _configVariables = new();
    private            Dictionary<string, object>                _configValues    = [];

    private IZoneManager        ZoneManager    { get; set; } = null!;
    private WorldMarkerRegistry MarkerRegistry { get; set; } = null!;

    internal void Setup(Dictionary<string, object> configValues)
    {
        _configValues = configValues;

        ZoneManager    = Framework.Service<IZoneManager>();
        MarkerRegistry = Framework.Service<WorldMarkerRegistry>();

        ZoneManager.ZoneChanged += OnZoneChanged;

        foreach (var cfg in GetConfigVariables()) {
            _configVariables[cfg.Id] = cfg;

            if (cfg is IUntypedMarkerConfigVariable u) {
                if (false == _configValues.ContainsKey(cfg.Id)) {
                    _configValues[cfg.Id] = u.GetDefaultValue()!;
                }

                u.UntypedValueChanged += value => _configValues[cfg.Id] = value;
            }
        }

        if (_configValues.Count > 0) {
            List<string> keysToRemove = [];

            foreach ((string key, object value) in _configValues) {
                if (!_configVariables.TryGetValue(key, out IMarkerConfigVariable? cfg)) {
                    keysToRemove.Add(key);
                    continue;
                }

                if (cfg is IUntypedMarkerConfigVariable u) {
                    u.SetValue(value);
                }
            }

            foreach (string key in keysToRemove) {
                _configValues.Remove(key);
            }
        }

        OnInitialized();
    }

    /// <inheritdoc/>
    public virtual void Dispose()
    {
        ZoneManager.ZoneChanged -= OnZoneChanged;

        foreach (WorldMarker marker in Markers.Values) {
            MarkerRegistry.RemoveMarker(marker);
        }

        Markers.Clear();

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Returns a dictionary of configuration values for this marker factory.
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, object> GetUserConfig()
    {
        return _configValues;
    }

    /// <summary>
    /// Returns a list of registered config options.
    /// </summary>
    /// <returns></returns>
    public List<IMarkerConfigVariable> GetConfigVariableList()
    {
        return _configVariables.Values.ToList();
    }


    public T GetConfigValue<T>(string name)
    {
        if (!_configVariables.TryGetValue(name, out var cfg)) {
            throw new InvalidOperationException($"No config variable with the name '{name}' exists.");
        }

        if (cfg is not MarkerConfigVariable<T> c) {
            throw new InvalidOperationException($"Config variable '{name}' is not of type '{typeof(T).Name}'.");
        }

        return c.Value;
    }

    public void SetConfigValue<T>(string name, T value)
    {
        if (!_configVariables.TryGetValue(name, out var cfg)) {
            throw new InvalidOperationException($"No config variable with the name '{name}' exists.");
        }

        if (cfg is not MarkerConfigVariable<T> c) {
            throw new InvalidOperationException($"Config variable '{name}' is not of type '{typeof(T).Name}'.");
        }

        c.Value = value;

        OnConfigValueChanged?.Invoke();
        OnConfigUpdated(name);
    }

    /// <summary>
    /// Invoked when the factory has been initialized and user configuration
    /// has been set.
    /// </summary>
    protected virtual void OnInitialized() { }

    /// <summary>
    /// Invoked when the current zone has changed.
    /// </summary>
    protected virtual void OnZoneChanged(IZone zone) { }

    /// <summary>
    /// Invoked when a configuration variable has been updated.
    /// </summary>
    protected virtual void OnConfigUpdated(string name) { }

    /// <summary>
    /// A list of default config variables that all factories should have.
    /// </summary>
    protected static IEnumerable<IMarkerConfigVariable> DefaultStateConfigVariables => [
        new BooleanMarkerConfigVariable(
            "Enabled",
            I18N.Translate("Settings.MarkersModule.Config.Enabled.Name"),
            null,
            false
        ),
        new BooleanMarkerConfigVariable(
            "ShowOnCompass",
            I18N.Translate("Settings.MarkersModule.Config.ShowOnCompass.Name"),
            null,
            true
        ),
    ];

    protected static IEnumerable<IMarkerConfigVariable> DefaultFadeConfigVariables => [
        new IntegerMarkerConfigVariable(
            "FadeDistance",
            I18N.Translate("Settings.MarkersModule.Config.FadeDistance.Name"),
            I18N.Translate("Settings.MarkersModule.Config.FadeDistance.Description"),
            32,
            0,
            100
        ),
        new IntegerMarkerConfigVariable(
            "FadeAttenuation",
            I18N.Translate("Settings.MarkersModule.Config.FadeAttenuation.Name"),
            I18N.Translate("Settings.MarkersModule.Config.FadeAttenuation.Description"),
            10,
            0,
            100
        ),
    ];

    /// <summary>
    /// Creates or updates the given world marker.
    /// </summary>
    protected WorldMarker SetMarker(WorldMarker marker)
    {
        string guid = MarkerRegistry.SetMarker(marker);

        return Markers[guid] = marker;
    }

    /// <summary>
    /// Removes the given marker.
    /// </summary>
    protected void RemoveMarker(WorldMarker marker)
    {
        RemoveMarker(marker.Key);
    }

    /// <summary>
    /// Removes a marker with the given key.
    /// </summary>
    protected void RemoveMarker(string key)
    {
        if (Markers.Remove(key, out WorldMarker? marker)) {
            MarkerRegistry.RemoveMarker(marker);
        }
    }

    /// <summary>
    /// Removes all markers except for the ones in the given list.
    /// </summary>
    protected void RemoveMarkersExcept(List<string> keys)
    {
        foreach (string key in Markers.Keys) {
            if (!keys.Contains(key)) RemoveMarker(key);
        }
    }

    /// <summary>
    /// Removes all markers this factory has created.
    /// </summary>
    protected void RemoveAllMarkers()
    {
        if (Markers.Count == 0) return;

        foreach (WorldMarker marker in Markers.Values) {
            MarkerRegistry.RemoveMarker(marker);
        }

        Markers.Clear();
    }
}
