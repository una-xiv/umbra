﻿using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Umbra.Common;

namespace Umbra.Windows.Library.IconPicker;

public sealed partial class IconPickerWindow(uint iconId) : Window
{
    public uint LastIconId { get; private set; } = iconId;
    public uint IconId     { get; set; }         = iconId;

    protected override Vector2 MinSize     { get; } = new(752, 600);
    protected override Vector2 MaxSize     { get; } = new(1200, 900);
    protected override Vector2 DefaultSize { get; } = new(752, 600);
    protected override string  Title       { get; } = I18N.Translate("Window.IconPicker.Title");

    private static Dictionary<string, List<(uint, uint)>> Categories { get; } = new() {
        {
            "General", [
                (0, 94), (101, 125), (651, 652), (66001, 66001), (66021, 66023), (66031, 66033),
                (66041, 66043), (66051, 66053), (66061, 66063), (66071, 66073), (66081, 66083), (66101, 66105),
                (66121, 66125), (66141, 66145), (66161, 66171), (66181, 66191), (66301, 66341), (66401, 66423),
                (66452, 66473), (60001, 6004), (60011, 60013), (60026, 60048), (60071, 60074), (61471, 61489),
                (61501, 61548), (61551, 61598), (61751, 61768), (61801, 61850), (61875, 61880)
            ]
        }, {
            "Jobs", [
                (0, 0), (62001, 62042), (62801, 62842), (62226, 62267), (62101, 62142), (62301, 62320), (62401, 62422)
            ]
        }, {
            "Quests", [
                (0, 0),
                (71001, 71006), (71021, 71025), (71041, 71045), (71061, 70165), (71081, 70185), (71101, 71102),
                (71121, 71125), (71141, 71145), (71201, 71205), (71221, 71225), (61721, 61723), (61731, 61733)
            ]
        }, {
            "Avatars", [
                (0, 0), (62145, 62146), (72556, 72608), (72001, 72059), (73001, 73278)
            ]
        }, {
            "Emotes", [
                (0, 0), (64001, 64080), (64082, 64099), (64101, 64133), (64151, 64154), (64326, 64349), (64352, 64455),
                (64457, 64459)
            ]
        }, {
            "Rewards", [
                (0, 0), (65001, 65110)
            ]
        }, {
            "MapMarkers", [
                (0, 0),
                (60401, 60408), (60412, 60482), (60501, 60508), (60511, 60515), (60550, 60565), (60567, 60583),
                (60585, 60611), (60640, 60649), (60651, 60662), (60751, 60792), (60901, 60999)
            ]
        }, {
            "Shapes", [
                (0, 0),
                (82091, 82093), (90001, 90004), (90200, 90263), (90401, 90463), (61901, 61918),
                (230131, 230143), // group pose birthday
                (230201, 230215), // group pose decoration small
                (230301, 230317), // group pose decoration large
                (230401, 230433), // group pose stamp symbol
                (230701, 230715), // group pose stamp nation
            ]
        }, {
            "Minions", [
                (0, 0), (4401, 4521), (4523, 4611), (4613, 4891), (4893, 4928), (4930, 4935), (4938, 4939),
                (59401, 59521), (59523, 59611), (59613, 59891),
                (59893, 59926)
            ]
        }, {
            "Mounts", [
                (0, 0), (4001, 4045), (4047, 4098), (4101, 4276), (4278, 4297), (4301, 4301), (4303, 4304)
            ]
        },
    };

    private static Dictionary<string, string> CategoryLabels { get; } = new() {
        { I18N.Translate("Window.IconPicker.Category.General"), "General" },
        { I18N.Translate("Window.IconPicker.Category.Jobs"), "Jobs" },
        { I18N.Translate("Window.IconPicker.Category.Quests"), "Quests" },
        { I18N.Translate("Window.IconPicker.Category.Avatars"), "Avatars" },
        { I18N.Translate("Window.IconPicker.Category.Emotes"), "Emotes" },
        { I18N.Translate("Window.IconPicker.Category.Rewards"), "Rewards" },
        { I18N.Translate("Window.IconPicker.Category.MapMarkers"), "MapMarkers" },
        { I18N.Translate("Window.IconPicker.Category.Shapes"), "Shapes" },
        { I18N.Translate("Window.IconPicker.Category.Minions"), "Minions" },
        { I18N.Translate("Window.IconPicker.Category.Mounts"), "Mounts" },
    };

    protected override void OnOpen()
    {
        IconIdInputNode.Value             =  (int)IconId;
        IconIdInputNode.OnValueChanged    += OnIconIdInputChanged;
        CategorySelectNode.OnValueChanged += OnCategorySelectChanged;

        CloseButtonNode.OnMouseUp += _ => Close();

        UndoButtonNode.OnMouseUp += _ => {
            IconId                = LastIconId;
            IconIdInputNode.Value = (int)IconId;
            IconGridNode? node                = BodyNode.QuerySelector<IconGridNode>("IconGrid");
            if (node != null) node.SelectedId = IconId;
        };

        OnCategorySelectChanged(CategoryLabels.Keys.First());
    }

    protected override void OnClose()
    {
        IconIdInputNode.OnValueChanged    -= OnIconIdInputChanged;
        CategorySelectNode.OnValueChanged -= OnCategorySelectChanged;
    }

    protected override void OnUpdate(int instanceId)
    {
        UpdateNodeSizes();
    }

    private void OnIconIdInputChanged(int value)
    {
        IconId = (uint)value;

        IconGridNode? node                = BodyNode.QuerySelector<IconGridNode>("IconGrid");
        if (node != null) node.SelectedId = IconId;
    }

    private void OnCategorySelectChanged(string value)
    {
        List<uint> ids = GetIconIds(Categories[CategoryLabels[value]]);

        BodyNode.QuerySelector("IconGrid")?.Dispose();
        BodyNode.AppendChild(new IconGridNode(ids, IconId) { Id = "IconGrid" });
    }

    private static List<uint> GetIconIds(List<(uint, uint)> idRanges)
    {
        List<uint> ids = [];

        foreach (var range in idRanges) {
            for (uint i = range.Item1; i <= range.Item2; i++) {
                ids.Add(i);
            }
        }

        return ids;
    }
}
