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

using System.Numerics;
using Dalamud.Utility;
using Umbra.Common;
using Umbra.Windows.Oobe;
using Umbra.Windows.Settings.Modules;

namespace Umbra.Windows.Settings;

internal partial class SettingsWindow : Window
{
    protected override Vector2 MinSize     { get; } = new(1100, 680);
    protected override Vector2 MaxSize     { get; } = new(1600, 1300);
    protected override Vector2 DefaultSize { get; } = new(1100, 680);
    protected override string  Title       => I18N.Translate("Settings.Window.Title");

    /// <inheritdoc/>
    protected override void OnOpen()
    {
        AddModule(new WidgetsModule());
        AddModule(new AuxWidgetsModule());
        AddModule(new MarkersModule());

        foreach (string category in ConfigManager.GetCategories()) {
            AddModule(new CvarModule(category));
        }

        AddModule(new AppearanceModule());
        AddModule(new ProfilesModule());
        AddModule(new PluginsModule());

        Node.ParentNode!.Overflow = true;

        Node.QuerySelector("RestartButton")!.OnMouseUp += _ => Framework.Restart();
        Node.QuerySelector("KoFiButton")!.OnMouseUp    += _ => Util.OpenLink("https://ko-fi.com/una_xiv");
        Node.QuerySelector("DiscordButton")!.OnMouseUp += _ => Util.OpenLink("https://discord.gg/xaEnsuAhmm");
        Node.QuerySelector("CloseButton")!.OnMouseUp   += _ => Close();
        Node.QuerySelector("OobeButton")!.OnMouseUp    += _ => {
            Framework.Service<WindowManager>().Present("OOBE", new OobeWindow());
            Close();
        };
    }

    /// <inheritdoc/>
    protected override void OnUpdate(int instanceId)
    {
        Node.Style.Size                = ContentSize;
        NavigationPanelNode.Style.Size = new(220, ContentSize.Height - 40);
        ContentPanelNode.Style.Size    = new(ContentSize.Width - 220, ContentSize.Height - 41);
        FooterPanelNode.Style.Size     = new(ContentSize.Width, 40);

        LogoNode.Style.Size = new(
            NavigationPanelNode.Style.Size.Width - LogoNode.ComputedStyle.Margin.HorizontalSize,
            NavigationPanelNode.Style.Size.Width - LogoNode.ComputedStyle.Margin.VerticalSize
        );

        NavigationListNode.Style.Size = new(
            NavigationPanelNode.Style.Size.Width,
            NavigationPanelNode.Style.Size.Height - LogoNode.Style.Size.Height - 30
        );

        ModuleButtonsNode.Style.Size = new(
            NavigationListNode.Style.Size.Width - 32,
            NavigationListNode.Style.Size.Height
        );

        foreach (var btn in ModuleButtonsNode.QuerySelectorAll(".module-button")) {
            btn.Style.Size = new(ModuleButtonsNode.Style.Size.Width - 30, 28);
        }

        _currentModule?.OnUpdate();
    }

    /// <inheritdoc/>
    protected override void OnClose()
    {
        _currentModule?.OnClose();
    }
}
