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
using System.Linq;
using Umbra.Common;
using Umbra.Markers;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Windows.Settings.Modules;

internal sealed partial class MarkersModule
{
    private static Node? RenderControlNode(WorldMarkerFactory factory, IMarkerConfigVariable cvar)
    {
        switch (cvar) {
            case BooleanMarkerConfigVariable b:
                return RenderCheckboxNode(factory, b);
            case IntegerMarkerConfigVariable i:
                return RenderIntegerNode(factory, i);
            case SelectMarkerConfigVariable s:
                return RenderSelectNode(factory, s);
            default:
                Logger.Warning($"Cannot render control node for config variable: {cvar.Id}");
                return null;
        }
    }

    private static CheckboxNode RenderCheckboxNode(WorldMarkerFactory factory, BooleanMarkerConfigVariable cvar)
    {
        CheckboxNode node = new(
            cvar.Id,
            factory.GetConfigValue<bool>(cvar.Id),
            cvar.Name,
            cvar.Description
        );

        node.OnValueChanged += v => {
            factory.SetConfigValue(cvar.Id, v);
        };

        return node;
    }

    private static IntegerInputNode RenderIntegerNode(WorldMarkerFactory factory, IntegerMarkerConfigVariable cvar)
    {
        IntegerInputNode node = new(
            cvar.Id,
            factory.GetConfigValue<int>(cvar.Id),
            cvar.MinValue,
            cvar.MaxValue,
            cvar.Name,
            cvar.Description
        );

        node.OnValueChanged += v => {
            factory.SetConfigValue(cvar.Id, v);
        };

        return node;
    }

    private static SelectNode RenderSelectNode(WorldMarkerFactory factory, SelectMarkerConfigVariable cvar)
    {
        if (cvar.Options.Count == 0)
            throw new InvalidOperationException("A select control must have at least one option.");

        if (!cvar.Options.TryGetValue(factory.GetConfigValue<string>(cvar.Id), out string? selectedValue)) {
            selectedValue = cvar.Options.First().Value;
        }

        Logger.Info($"Selected value: {selectedValue}");

        var node = new SelectNode(
            cvar.Id,
            selectedValue,
            cvar.Options.Values.ToList(),
            cvar.Name,
            cvar.Description
        );

        node.OnValueChanged += newValue => {
            if (cvar.Options.ContainsValue(newValue)) {
                Logger.Info($"Setting {cvar.Id} to {newValue} ({cvar.Options.First(x => x.Value == newValue).Key})");
                factory.SetConfigValue(cvar.Id, cvar.Options.First(x => x.Value == newValue).Key);
            }
        };

        return node;
    }
}
