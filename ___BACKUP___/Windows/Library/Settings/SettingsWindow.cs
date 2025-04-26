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
    protected override string  Title       { get; } = I18N.Translate("Settings.Window.Title");

    private bool _isDisposed;

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
        if (_isDisposed) return;
        _currentModule?.OnUpdate();
    }

    /// <inheritdoc/>
    protected override void OnClose()
    {
        _currentModule?.OnClose();

        foreach (var module in _modules.Values) {
            module.Dispose();
        }

        _modules.Clear();
    }

    /// <inheritdoc/>
    protected override void OnDisposed()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        base.OnDisposed();
    }
}
