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

namespace Umbra.Widgets;

/// <summary>
/// Registers the annotated class as toolbar widget type that has a dependency
/// on the existence of another plugin with the given name.
/// </summary>
/// <param name="id">A unique ID of the widget.</param>
/// <param name="name">The display name of the widget.</param>
/// <param name="description">A description of this widget.</param>
/// <param name="pluginName">The internal name of the plugin this widget relies on.</param>
[AttributeUsage(AttributeTargets.Class)]
public class InteropToolbarWidgetAttribute(string id, string name, string description, string pluginName) : Attribute
{
    public string Id          { get; } = id;
    public string Name        { get; } = name;
    public string Description { get; } = description;
    public string PluginName  { get; } = pluginName;
}
