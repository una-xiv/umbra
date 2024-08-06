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
using System.Linq;
using Dalamud.Utility;
using Lumina.Excel.GeneratedSheets;
using Umbra.Common;

namespace Umbra.Widgets;

internal partial class CurrenciesWidget
{
    public override MenuPopup Popup { get; } = new() { UseGrayscaleIcons = false };

    private void HydratePopupMenu()
    {
        Framework.DalamudFramework.RunOnTick(
            () => {
                Popup?.Clear();
                HydratePopupMenuInternal();
                HydrateCustomCurrencies();
            },
            delayTicks: 1
        );
    }

    private void HydratePopupMenuInternal()
    {
        IEnumerable<Currency> group0Currencies = Currencies.Values.Where(currency => currency.GroupId == 0);

        byte gcId = Player.GrandCompanyId;

        foreach (Currency currency in group0Currencies) {
            if (currency.Type == CurrencyType.Maelstrom && gcId != 1) continue;
            if (currency.Type == CurrencyType.TwinAdder && gcId != 2) continue;
            if (currency.Type == CurrencyType.ImmortalFlames && gcId != 3) continue;

            Popup.AddButton(
                $"Currency_{currency.Id}",
                currency.Name,
                iconId: currency.Icon,
                altText: GetAmount(currency.Type, GetConfigValue<bool>("ShowCap")),
                onClick: () => {
                    var type = currency.Type.ToString();

                    SetConfigValue("TrackedCurrency", GetConfigValue<string>("TrackedCurrency") != type
                        ? currency.Type.ToString()
                        : ""
                    );
                }
            );

            Popup.SetButtonVisibility($"Currency_{currency.Id}", GetConfigValue<bool>($"EnabledCurrency_{currency.Id}"));
        }

        // Add groups.
        Popup.AddGroup("Group_1", I18N.Translate("Widget.Currencies.Group.TheHunt"));
        Popup.AddGroup("Group_2", I18N.Translate("Widget.Currencies.Group.Tomestones"));
        Popup.AddGroup("Group_3", I18N.Translate("Widget.Currencies.Group.PvP"));
        Popup.AddGroup("Group_4", I18N.Translate("Widget.Currencies.Group.CraftingGathering"));
        Popup.AddGroup("Group_5", I18N.Translate("Widget.Currencies.Group.Miscellaneous"));

        foreach (Currency currency in Currencies.Values) {
            if (currency.GroupId == 0) continue;

            Popup.AddButton(
                $"Currency_{currency.Id}",
                currency.Name,
                iconId: currency.Icon,
                altText: GetAmount(currency.Type, GetConfigValue<bool>("ShowCap")),
                groupId: $"Group_{currency.GroupId}",
                onClick: () => {
                    var type = currency.Type.ToString();

                    SetConfigValue("TrackedCurrency", GetConfigValue<string>("TrackedCurrency") != type
                        ? currency.Type.ToString()
                        : ""
                    );
                }
            );

            Popup.SetButtonVisibility($"Currency_{currency.Id}", GetConfigValue<bool>($"EnabledCurrency_{currency.Id}"));
        }
    }

    private void HydrateCustomCurrencies()
    {
        foreach (Currency currency in CustomCurrencies.Values) {
            if (Popup.HasButton($"CustomCurrency_{currency.Id}")) continue;

            Popup.AddButton(
                $"CustomCurrency_{currency.Id}",
                iconId: currency.Icon,
                label: $"{currency.Name}",
                altText: GetCustomAmount(currency.Id, GetConfigValue<bool>("ShowCap")),
                groupId: "Group_5",
                onClick: () => {
                    var type  = currency.Id.ToString();
                    string newId = GetConfigValue<string>("TrackedCurrency") != type ? type : "";

                    SetConfigValue("TrackedCurrency", newId);
                }
            );
        }
    }
}
