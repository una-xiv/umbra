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

using Dalamud.Interface;
using Umbra.Common;
using Umbra.Game;
using Una.Drawing;

namespace Umbra.Widgets;

internal partial class GearsetNode : Node
{
    public const int NodeWidth  = 200;
    public const int NodeHeight = 40;

    public readonly Gearset Gearset;

    private readonly IGearsetRepository _repository;
    private readonly IPlayer            _player;

    public GearsetNode(IGearsetRepository repository, IPlayer player, Gearset gearset)
    {
        _repository = repository;
        _player     = player;

        Gearset    = gearset;
        Stylesheet = GearsetSwitcherItemStylesheet;
        ClassList  = ["gearset"];

        OnMouseUp += _ => _repository.EquipGearset(gearset.Id);

        ChildNodes = [
            new() {
                Id        = "Icon",
                ClassList = ["gearset--icon"],
                Style     = new() { IconId = gearset.JobId + 62000u },
                ChildNodes = [
                    new() {
                        Id        = "ExclamationMark",
                        ClassList = ["gearset--icon--exclamation-mark"],
                        NodeValue = FontAwesomeIcon.ExclamationTriangle.ToIconString(),
                        Tooltip   = I18N.Translate("Widget.GearsetSwitcher.WarningTooltip.AppearanceDiffers"),
                        Style     = new() { IsVisible = false },
                    }
                ]
            },
            new() {
                ClassList = ["gearset--body"],
                ChildNodes = [
                    new() {
                        Id        = "Name",
                        ClassList = ["gearset--body--name"],
                        NodeValue = gearset.Name
                    },
                    new() {
                        Id        = "Info",
                        ClassList = ["gearset--body--info"],
                        NodeValue = $"Level {gearset.JobLevel} {gearset.JobName}"
                    }
                ]
            },
            new() {
                Id        = "ItemLevel",
                ClassList = ["gearset--ilvl"],
                NodeValue = gearset.Name
            },
            new() {
                Id        = "ExpBar",
                ClassList = ["gearset-exp-bar"],
                ChildNodes = [
                    new() {
                        Id        = "ExpBarFill",
                        ClassList = ["gearset-exp-bar--bar"],
                        Style     = new() { Size = new(25, NodeHeight - 14) },
                    },
                ]
            },
            new() {
                Id        = "ExpBarText",
                ClassList = ["gearset-exp-bar--text"],
                NodeValue = "50%"
            }
        ];
    }

    public int    ButtonIconYOffset   { get; set; }
    public string ButtonIconType      { get; set; } = "Default";
    public bool   ShowGearsetGradient { get; set; } = true;

    public void Update()
    {
        IconNode.Style.IconId      = _player.GetJobInfo(Gearset.JobId).GetIcon(ButtonIconType);
        IconNode.Style.ImageOffset = new(0, ButtonIconYOffset);

        NameNode.NodeValue = Gearset.Name;
        InfoNode.NodeValue = GetCurrentGearsetStatusText();
        IlvlNode.NodeValue = Gearset.ItemLevel.ToString();

        ExpBarNode.Style.IsVisible     = !Gearset.IsMaxLevel;
        ExpBarTextNode.Style.IsVisible = !Gearset.IsMaxLevel;
        ExpBarTextNode.NodeValue       = $"{Gearset.JobXp}%";
        ExpBarFillNode.Style.Size      = new((NodeWidth - 12) * Gearset.JobXp / 100, 1);

        switch (Gearset.IsMaxLevel) {
            case false when !IlvlNode.TagsList.Contains("with-exp-bar"):
                IlvlNode.TagsList.Add("with-exp-bar");
                break;
            case true when IlvlNode.TagsList.Contains("with-exp-bar"):
                IlvlNode.TagsList.Remove("with-exp-bar");
                break;
        }

        if (Gearset == _repository.CurrentGearset && !TagsList.Contains("current")) {
            TagsList.Add("current");
        } else if (Gearset != _repository.CurrentGearset && TagsList.Contains("current")) {
            TagsList.Remove("current");
        }

        SetBackgroundGradientFor(Gearset.Category);

        if (Gearset.IsMainHandMissing) {
            WarnNode.Style.Color     = new(0xE00000DA);
            WarnNode.Style.IsVisible = true;
            WarnNode.Tooltip         = I18N.Translate("Widget.GearsetSwitcher.WarningTooltip.MissingMainHand");
        } else if (Gearset.HasMissingItems) {
            WarnNode.Style.Color     = new(0xE000DADF);
            WarnNode.Style.IsVisible = true;
            WarnNode.Tooltip         = I18N.Translate("Widget.GearsetSwitcher.WarningTooltip.MissingItems");
        } else if (Gearset.AppearanceDiffers) {
            WarnNode.Style.Color     = new(0xC0A0A0A0);
            WarnNode.Style.IsVisible = true;
            WarnNode.Tooltip         = I18N.Translate("Widget.GearsetSwitcher.WarningTooltip.AppearanceDiffers");
        } else {
            WarnNode.Style.IsVisible = false;
        }
    }

    private Node IconNode       => QuerySelector("Icon")!;
    private Node NameNode       => QuerySelector("Name")!;
    private Node InfoNode       => QuerySelector("Info")!;
    private Node IlvlNode       => QuerySelector("ItemLevel")!;
    private Node WarnNode       => QuerySelector("ExclamationMark")!;
    private Node ExpBarNode     => QuerySelector("ExpBar")!;
    private Node ExpBarFillNode => QuerySelector("ExpBarFill")!;
    private Node ExpBarTextNode => QuerySelector("ExpBarText")!;

    private string GetCurrentGearsetStatusText()
    {
        return $"{I18N.Translate("Widget.GearsetSwitcher.JobLevel", Gearset.JobLevel)} {Gearset.JobName}";
    }

    private void SetBackgroundGradientFor(GearsetCategory category)
    {
        if (!ShowGearsetGradient) {
            Style.BackgroundGradient = GradientColor.Vertical(new(0), new(0));
            return;
        }

        switch (category) {
            case GearsetCategory.Tank:
                Style.BackgroundGradient = GradientColor.Vertical(new(0xA0a54a3b), new(0));
                break;
            case GearsetCategory.Healer:
                Style.BackgroundGradient = GradientColor.Vertical(new(0xA02e613b), new(0));
                break;
            case GearsetCategory.Melee:
                Style.BackgroundGradient = GradientColor.Vertical(new(0xA02e3069), new(0));
                break;
            case GearsetCategory.Ranged:
                Style.BackgroundGradient = GradientColor.Vertical(new(0xA02c89a6), new(0));
                break;
            case GearsetCategory.Caster:
                Style.BackgroundGradient = GradientColor.Vertical(new(0xA0a72a5a), new(0));
                break;
            default:
                Style.BackgroundGradient = GradientColor.Vertical(new(0), new(0));
                break;
        }
    }
}
