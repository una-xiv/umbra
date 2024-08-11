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

using Dalamud.Game.Text;
using Dalamud.Interface;
using Una.Drawing;

namespace Umbra.Widgets;

internal partial class ExperienceBarWidget
{
    public override Node Node { get; } = new() {
        Stylesheet = Stylesheet,
        ClassList  = ["experience-bar"],
        ChildNodes = [
            new() {
                ClassList = ["bar", "normal"],
            },
            new() {
                ClassList = ["bar", "rested"],
            },
            new() {
                ClassList = ["sanctuary-icon"],
                NodeValue = FontAwesomeIcon.Moon.ToIconString(),
            },
            new() {
                ClassList = ["sync-icon"],
                NodeValue = SeIconChar.Experience.ToIconString(),
            },
            new() {
                ClassList = ["label", "left"],
            },
            new() {
                ClassList = ["label", "right"],
            }
        ]
    };

    private Node NormalXpBarNode   => Node.QuerySelector(".bar.normal")!;
    private Node RestedXpBarNode   => Node.QuerySelector(".bar.rested")!;
    private Node LeftLabelNode     => Node.QuerySelector(".label.left")!;
    private Node RightLabelNode    => Node.QuerySelector(".label.right")!;
    private Node SanctuaryIconNode => Node.QuerySelector(".sanctuary-icon")!;
    private Node SyncIconNode      => Node.QuerySelector(".sync-icon")!;
}
