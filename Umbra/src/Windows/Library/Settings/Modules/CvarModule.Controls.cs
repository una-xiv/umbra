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

using System.Collections.Generic;
using Umbra.Common;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Windows.Settings.Modules;

internal partial class CvarModule
{
    private Node? RenderCvar(Cvar cvar)
    {
        if (cvar.Default is bool) {
            var node = new CheckboxNode(
                cvar.Slug,
                (bool)(cvar.Value ?? false),
                I18N.Translate($"CVAR.{cvar.Id}.Name"),
                I18N.Has($"CVAR.{cvar.Id}.Description") ? I18N.Translate($"CVAR.{cvar.Id}.Description") : null
            );

            node.ClassList.Add("cvar");
            node.OnValueChanged += value => ConfigManager.Set(cvar.Id, value);

            return node;
        }

        if (cvar.Options is not null && cvar.Options.Count > 0 && cvar.Default is string) {
            var node = new SelectNode(
                cvar.Slug,
                (string)(cvar.Value ?? cvar.Default),
                cvar.Options,
                I18N.Translate($"CVAR.{cvar.Id}.Name"),
                I18N.Has($"CVAR.{cvar.Id}.Description") ? I18N.Translate($"CVAR.{cvar.Id}.Description") : null
            );

            node.ClassList.Add("cvar");
            node.OnValueChanged += value => ConfigManager.Set(cvar.Id, value);

            return node;
        }

        if (cvar is { Default: int, Min: not null, Max: not null }) {
            var node = new IntegerInputNode(
                cvar.Slug,
                (int)(cvar.Value ?? cvar.Default),
                (int)cvar.Min,
                (int)cvar.Max,
                I18N.Translate($"CVAR.{cvar.Id}.Name"),
                I18N.Has($"CVAR.{cvar.Id}.Description") ? I18N.Translate($"CVAR.{cvar.Id}.Description") : null
            );

            node.ClassList.Add("cvar");
            node.OnValueChanged += value => ConfigManager.Set(cvar.Id, value);

            return node;
        }

        if (cvar is { Default: float, Min: not null, Max: not null }) {
            var node = new FloatInputNode(
                cvar.Slug,
                (float)(cvar.Value ?? cvar.Default),
                (float)cvar.Min,
                (float)cvar.Max,
                I18N.Translate($"CVAR.{cvar.Id}.Name"),
                I18N.Has($"CVAR.{cvar.Id}.Description") ? I18N.Translate($"CVAR.{cvar.Id}.Description") : null
            );

            node.ClassList.Add("cvar");
            node.OnValueChanged += value => ConfigManager.Set(cvar.Id, value);

            return node;
        }

        return null;
    }
}
