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

using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using System.Collections.Generic;
using Umbra.Common;
using Umbra.Game;
using Una.Drawing;

namespace Umbra.Widgets;

internal sealed partial class GearsetSwitcherPopup
{
    private Dictionary<GearsetCategory, List<Gearset>> GearsetsByCategory { get; } = [];
    private Dictionary<GearsetCategory, Node>          RoleContainers     { get; } = [];
    private Dictionary<Gearset, GearsetNode>           NodeByGearset      { get; } = [];

    /// <summary>
    /// Invoked when a gearset has been created.
    /// </summary>
    private void OnGearsetCreated(Gearset gearset)
    {
        if (!ShouldRenderGearset(gearset)) return;

        AssignGearsetToDataLookupTables(gearset);

        GearsetNode node = new(_gearsetRepository, _player, gearset);
        node.OnRightClick += OnGearsetRightClick;

        GetGearsetListNodeFor(gearset.Category).AppendChild(NodeByGearset[gearset] = node);
    }

    /// <summary>
    /// Invoked when a gearset has been removed.
    /// </summary>
    private void OnGearsetRemoved(Gearset gearset)
    {
        if (!NodeByGearset.TryGetValue(gearset, out var gearsetNode)) return;

        gearsetNode.OnRightClick -= OnGearsetRightClick;
        gearsetNode.Remove();

        NodeByGearset.Remove(gearset);
        RemoveGearsetFromDataLookupTables(gearset);
    }

    /// <summary>
    /// Invoked when a gearset has been changed.
    /// </summary>
    private void OnGearsetChanged(Gearset gearset)
    {
        if (!ShouldRenderGearset(gearset)) {
            OnGearsetRemoved(gearset);
            return;
        }

        if (!NodeByGearset.TryGetValue(gearset, out GearsetNode? gearsetNode)) {
            OnGearsetCreated(gearset);
            return;
        }

        AssignGearsetToDataLookupTables(gearset);
        GetGearsetListNodeFor(gearset.Category).AppendChild(gearsetNode);
        SetBackgroundGradientFor(gearset.Category);
    }

    private unsafe void OnGearsetRightClick(Node node)
    {
        if (node is not GearsetNode gsNode) {
            return;
        }

        _ctxSelectedGearset = gsNode.Gearset;
        ContextMenu!.SetEntryLabel("UnlinkGlam", I18N.Translate("Widget.GearsetSwitcher.ContextMenu.UnlinkGlamourPlate", _ctxSelectedGearset.GlamourSetLink == 0 ? "" : _ctxSelectedGearset.GlamourSetLink.ToString()));
        ContextMenu!.SetEntryDisabled("LinkGlam", !UIState.Instance()->IsUnlockLinkUnlocked(15) || !GameMain.IsInSanctuary());
        ContextMenu!.SetEntryDisabled("UnlinkGlam", _ctxSelectedGearset.GlamourSetLink == 0);
        ContextMenu!.SetEntryDisabled("EditBanner", !AgentBannerEditor.Instance()->IsActivatable());
        ContextMenu!.SetEntryDisabled("MoveUp",   _gearsetRepository.FindPrevIdInCategory(_ctxSelectedGearset) == null);
        ContextMenu!.SetEntryDisabled("MoveDown", _gearsetRepository.FindNextIdInCategory(_ctxSelectedGearset) == null);
        ContextMenu!.SetEntryDisabled("Delete",   _ctxSelectedGearset.IsCurrent);
        ContextMenu!.Present();
    }

    /// <summary>
    /// Adds the given gearset to the lookup tables.
    /// </summary>
    private void AssignGearsetToDataLookupTables(Gearset gearset)
    {
        // Remove from previous category.
        foreach ((GearsetCategory category, List<Gearset> gearsets) in GearsetsByCategory) {
            if (category != gearset.Category && gearsets.Contains(gearset)) {
                gearsets.Remove(gearset);
            }
        }

        if (!ShouldRenderGearset(gearset)) return;

        if (!GearsetsByCategory.ContainsKey(gearset.Category)) {
            GearsetsByCategory[gearset.Category] = [gearset];
            return;
        }

        if (!GearsetsByCategory[gearset.Category].Contains(gearset)) {
            GearsetsByCategory[gearset.Category].Add(gearset);
        }
    }

    /// <summary>
    /// Removes the given gearset from the lookup tables.
    /// </summary>
    private void RemoveGearsetFromDataLookupTables(Gearset gearset)
    {
        foreach ((GearsetCategory category, List<Gearset> gearsets) in GearsetsByCategory) {
            if (gearsets.Contains(gearset)) {
                gearsets.Remove(gearset);
                Logger.Debug($"Gearset {gearset.Id} removed from category {category}");
            }
        }

        NodeByGearset.Remove(gearset);
    }

    private bool ShouldRenderGearset(Gearset gearset)
    {
        return string.IsNullOrEmpty(GearsetFilterPrefix) || !gearset.Name.StartsWith(GearsetFilterPrefix);
    }
}
