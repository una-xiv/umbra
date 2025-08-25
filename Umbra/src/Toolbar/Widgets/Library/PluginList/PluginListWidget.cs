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

using Dalamud.Plugin;

using Lumina.Misc;
using System.Collections.Immutable;

namespace Umbra.Widgets;

[ToolbarWidget(
    "PluginList",
    "Widget.PluginList.Name",
    "Widget.PluginList.Description",
    ["plugin", "addon", "dalamud", "list", "menu"]
)]
internal sealed partial class PluginListWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Icon |
        StandardWidgetFeatures.CustomizableIcon;

    protected override string          DefaultIconType        => IconTypeFontAwesome;
    protected override FontAwesomeIcon DefaultFontAwesomeIcon => FontAwesomeIcon.Plug;

    public override MenuPopup Popup { get; } = new();

    private readonly Dictionary<string, MenuPopup.Button> _buttons = [];

    protected override void OnLoad()
    {
        Popup.OnPopupOpen  += UpdatePluginList;
        Popup.OnPopupClose += ClearPluginList;

        Node.OnRightClick += _ => Framework.Service<IChatSender>().Send("/xlplugins");
    }

    protected override void OnUnload()
    {
        Popup.OnPopupOpen  -= UpdatePluginList;
        Popup.OnPopupClose -= ClearPluginList;

        _buttons.Clear();
    }

    protected override void OnDraw()
    {
        Node.Tooltip = GetConfigValue<bool>("ShowTooltip") ? I18N.Translate("Widget.PluginList.Tooltip") : null;
    }

    private void UpdatePluginList()
    {
        Framework.DalamudFramework.Run(UpdatePluginListInternal);
    }

    private void UpdatePluginListInternal()
    {
        List<IExposedPlugin> pluginList = [..Framework.DalamudPlugin.InstalledPlugins];
        pluginList.Sort((a, b) => String.Compare(a.Name, b.Name, StringComparison.InvariantCultureIgnoreCase));

        List<string> usedPluginIds = [];

        foreach (var plugin in pluginList) {
            if (!plugin.IsLoaded || plugin is { HasMainUi: false, HasConfigUi: false }) continue;

            string id = $"Plugin_{Crc32.Get(plugin.InternalName)}";

            if (HasConfigVariable($"Enabled{id}") && !GetConfigValue<bool>($"Enabled{id}")) continue;
            if (_buttons.ContainsKey(id)) continue;

            usedPluginIds.Add(id);
            MenuPopup.Button button = CreatePluginEntry(id, plugin);

            Popup.Add(button);
            _buttons.Add(id, button);
        }
 
        foreach (var (id, button) in _buttons.ToImmutableArray()) {
            if (usedPluginIds.Contains(id)) continue;
         
            Popup.Remove(button, true);
            _buttons.Remove(id);
        }
    }

    private void ClearPluginList()
    {
        Framework.DalamudFramework.Run(
            () => {
                Popup.Clear(true);
                _buttons.Clear();
            }
        );
    }

    private MenuPopup.Button CreatePluginEntry(string id, IExposedPlugin plugin)
    {
        MenuPopup.Button button = new(id) {
            Label = plugin.Name,
            OnClick = () => {
                if ((ImGui.GetIO().KeyShift || ImGui.GetIO().KeyCtrl) && plugin.HasConfigUi) {
                    plugin.OpenConfigUi();
                    return;
                }

                if (plugin.HasMainUi) {
                    plugin.OpenMainUi();
                } else {
                    plugin.OpenConfigUi();
                }
            }
        };

        if (plugin is { HasMainUi: true, HasConfigUi: true }) {
            var configButton = new ButtonNode($"{id}_config", null, FontAwesomeIcon.Cog, isGhost: true, isSmall: true);
            configButton.OnClick += _ => plugin.OpenConfigUi();
            button.Node.QuerySelector(".alt-text")!.AppendChild(configButton);
        }

        return button;
    }
}
