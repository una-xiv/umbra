using FFXIVClientStructs.FFXIV.Client.Game.UI;
using System.Collections.Generic;

namespace Umbra.Widgets.Library.Societies;

internal sealed partial class SocietiesWidgetPopup
{
    private static Dictionary<uint, uint> SocietyToAetheryteId { get; } = new() {
        { 1, 19 },   // Amalj'aa - Little Ala Mhigo
        { 2, 4 },    // Sylphs - The Hawthorne Hut
        { 3, 16 },   // Kobolds - Camp Overlook
        { 4, 14 },   // Sahagin - Aleport
        { 5, 7 },    // Ixal - Fallgourd Float
        { 6, 73 },   // Vanu Vanu - Ok'Zundu
        { 7, 76 },   // Vath - Tailfeather
        { 8, 79 },   // Moogles - Zenith
        { 9, 105 },  // Kojin - Tamamizu
        { 10, 99 },  // Ananta - The Peering Stones
        { 11, 128 }, // Namazu - Dhoro Iloh
        { 12, 144 }, // Pixies - Lydha Lran
        { 13, 143 }, // Qitari - Fanow
        { 14, 136 }, // Dwarves - The Ostall Imperative
        { 15, 169 }, // Arkasodara - Yedlihmad
        { 16, 181 }, // Omicrons - Base Omicron
        { 17, 175 }, // Loporrits - Bestways Burrow
    };

    private uint? _selectedSocietyId;

    private void TeleportToSelectedSociety()
    {
        if (null == _selectedSocietyId || !SocietyToAetheryteId.TryGetValue(_selectedSocietyId.Value, out uint id)) {
            return;
        }

        if (Player.CanUseTeleportAction) {
            unsafe {
                Telepo.Instance()->Teleport(id, 0);
            }
        }
    }
}
