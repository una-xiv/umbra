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
using Umbra.Common;
using Umbra.Game;
using Umbra.Widgets.System;
using Umbra.Windows.Components;

namespace Umbra.Windows.Library.WidgetConfig;

internal partial class ToolbarProfilesWindow : Window
{
    protected override string  Title       { get; }
    protected override Vector2 MinSize     { get; } = new(512, 300);
    protected override Vector2 MaxSize     { get; } = new(720, 600);
    protected override Vector2 DefaultSize { get; } = new(512, 300);

    private WidgetManager Manager { get; }
    private IPlayer       Player  { get; }

    public ToolbarProfilesWindow()
    {
        Manager = Framework.Service<WidgetManager>();
        Player  = Framework.Service<IPlayer>();
        Title   = I18N.Translate("ToolbarProfilesWindow.Title");
        MinSize = new(640, 600);
        MaxSize = new(800, 1200);

        Node.QuerySelector("#CloseButton")!.OnMouseUp += _ => Close();
    }

    /// <inheritdoc/>
    protected override void OnOpen()
    {
        Node.QuerySelector<CheckboxNode>("#UseJobAssociatedProfiles")!.Value = WidgetManager.UseJobAssociatedProfiles;

        Manager.ActiveProfileChanged += OnActiveProfileChanged;
        Manager.ProfileCreated       += OnProfileListChanged;
        Manager.ProfileRemoved       += OnProfileListChanged;
    }

    /// <inheritdoc/>
    protected override void OnClose()
    {
        Manager.ActiveProfileChanged -= OnActiveProfileChanged;
        Manager.ProfileCreated       -= OnProfileListChanged;
        Manager.ProfileRemoved       -= OnProfileListChanged;
    }

    /// <inheritdoc/>
    protected override void OnUpdate(int instanceId)
    {
        UpdateNodeSizes();
    }

    private void OnActiveProfileChanged(string name)
    {
        Logger.Info($"Active profile changed to: {name}");
        Node.QuerySelector<SelectNode>("#ActiveProfile")!.Value = name;
        Node.QuerySelector<SelectNode>("#DeleteProfile")!.Value = Manager.GetActiveProfileName();

        if (!WidgetManager.UseJobAssociatedProfiles) return;

        Node.QuerySelector<SelectNode>($"JobProfile_{Player.JobId}")!.Value = name;
    }

    private void OnProfileListChanged(string _)
    {
        Node.QuerySelector<SelectNode>("#ActiveProfile")!.Choices = Manager.GetProfileNames();
        Node.QuerySelector<SelectNode>("#DeleteProfile")!.Choices = Manager.GetProfileNames();
        Node.QuerySelector<SelectNode>("#DeleteProfile")!.Value   = Manager.GetActiveProfileName();

        foreach (var node in Node.QuerySelectorAll<SelectNode>(".job-profile-select--selector")) {
            node.Choices = Manager.GetProfileNames();
        }
    }
}
