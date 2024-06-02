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
using Umbra.Game;
using Una.Drawing;

namespace Umbra.Widgets;

internal sealed partial class GearsetSwitcherPopup
{
    private void CreateColumnsNode()
    {
        Node node = new() {
            Stylesheet = GearsetSwitcherColumnsStylesheet,
            Id         = "Columns",
            ChildNodes = [
                new() {
                    Id        = "LeftColumn",
                    ClassList = ["column"],
                },
                new() {
                    Id        = "MiddleColumn",
                    ClassList = ["column"],
                },
                new() {
                    Id        = "RightColumn",
                    ClassList = ["column"],
                },
            ],
        };

        Node.AppendChild(node);
    }

    private void CreateRoleContainer(GearsetCategory category, string label)
    {
        Node node = new() {
            Stylesheet = GearsetSwitcherColumnsStylesheet,
            Id         = $"RoleContainer_{category}",
            ClassList  = ["role-container"],
            ChildNodes = [
                new() {
                    Id        = "RoleHeader",
                    NodeValue = label,
                },
                new() {
                    Id       = "RoleBody",
                    Overflow = false,
                    ChildNodes = [
                        new() {Id = "GearsetList" }
                    ]
                }
            ]
        };

        RoleContainers[category] = node;
        GetColumnForRole(category).AppendChild(node);
    }

    private Node GetColumnForRole(GearsetCategory category)
    {
        return category switch {
            GearsetCategory.Tank     => Node.QuerySelector(TankRoleLocation)!,
            GearsetCategory.Healer   => Node.QuerySelector(HealerRoleLocation)!,
            GearsetCategory.Melee    => Node.QuerySelector(MeleeRoleLocation)!,
            GearsetCategory.Ranged   => Node.QuerySelector(RangedRoleLocation)!,
            GearsetCategory.Caster   => Node.QuerySelector(CasterRoleLocation)!,
            GearsetCategory.Gatherer => Node.QuerySelector(GathererRoleLocation)!,
            GearsetCategory.Crafter  => Node.QuerySelector(CrafterRoleLocation)!,
            GearsetCategory.None     => throw new ArgumentOutOfRangeException(nameof(category), category, null),
            _                        => throw new ArgumentOutOfRangeException(nameof(category), category, null)
        };
    }

    private bool GetVisibilityForRole(GearsetCategory category)
    {
        return category switch {
            GearsetCategory.Tank     => ShowTank,
            GearsetCategory.Healer   => ShowHealer,
            GearsetCategory.Melee    => ShowMelee,
            GearsetCategory.Ranged   => ShowRanged,
            GearsetCategory.Caster   => ShowCaster,
            GearsetCategory.Gatherer => ShowGatherer,
            GearsetCategory.Crafter  => ShowCrafter,
            GearsetCategory.None     => throw new ArgumentOutOfRangeException(nameof(category), category, null),
            _                        => throw new ArgumentOutOfRangeException(nameof(category), category, null)
        };
    }

    private int GetSortIndexForRole(GearsetCategory category)
    {
        return category switch {
            GearsetCategory.Tank     => TankRoleSortIndex,
            GearsetCategory.Healer   => HealerRoleSortIndex,
            GearsetCategory.Melee    => MeleeRoleSortIndex,
            GearsetCategory.Ranged   => RangedRoleSortIndex,
            GearsetCategory.Caster   => CasterRoleSortIndex,
            GearsetCategory.Gatherer => GathererRoleSortIndex,
            GearsetCategory.Crafter  => CrafterRoleSortIndex,
            GearsetCategory.None     => throw new ArgumentOutOfRangeException(nameof(category), category, null),
            _                        => throw new ArgumentOutOfRangeException(nameof(category), category, null)
        };
    }

    private int GetMaxItemsToDisplayForRole(GearsetCategory category)
    {
        return category switch {
            GearsetCategory.Tank     => TankMaxItems,
            GearsetCategory.Healer   => HealerMaxItems,
            GearsetCategory.Melee    => MeleeMaxItems,
            GearsetCategory.Ranged   => RangedMaxItems,
            GearsetCategory.Caster   => CasterMaxItems,
            GearsetCategory.Gatherer => GathererMaxItems,
            GearsetCategory.Crafter  => CrafterMaxItems,
            GearsetCategory.None     => throw new ArgumentOutOfRangeException(nameof(category), category, null),
            _                        => throw new ArgumentOutOfRangeException(nameof(category), category, null)
        };
    }

    private Node GetGearsetListNodeFor(GearsetCategory category)
    {
        return RoleContainers[category].QuerySelector("#GearsetList")!;
    }
}
