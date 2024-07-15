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

using System.Linq;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets;

internal partial class TeleportWidgetPopup : WidgetPopup
{
    private string _selectedExpansion = string.Empty;

    public int MinimumColumns { get; set; } = 1;

    /// <inheritdoc/>
    protected override bool CanOpen()
    {
        return Framework.Service<IPlayer>().CanUseTeleportAction;
    }

    /// <inheritdoc/>
    protected override void OnOpen()
    {
        HydrateAetherytePoints();
        BuildNodes();
        ActivateExpansion(_selectedExpansion, true);
    }

    /// <inheritdoc/>
    protected override void OnClose()
    {
        ExpansionLists.Clear();

        _expansions.Clear();
        _selectedExpansion = string.Empty;
    }

    private void ActivateExpansion(string key, bool force = false)
    {
        if (!force && key == _selectedExpansion) return;

        if (_selectedExpansion != string.Empty) {
            Node.FindById("ExpansionList")!.QuerySelector(_selectedExpansion)!.TagsList.Remove("selected");
        }

        _selectedExpansion = key;
        Node.FindById("ExpansionList")!.QuerySelector(key)!.TagsList.Add("selected");
        Node.FindById("DestinationList")!.ChildNodes = [ExpansionLists[key]];
    }
}
