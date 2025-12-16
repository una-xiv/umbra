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

using System.Reflection;
using Dalamud.IoC;
using Dalamud.Plugin;

using System.Diagnostics;
using Logger = Umbra.Common.Logger;

namespace Umbra;

internal sealed class Plugin : IDalamudPlugin
{
    public static string Name => "Umbra";

    [PluginService] private IClientState ClientState      { get; set; } = null!;
    [PluginService] private IPlayerState PlayerState      { get; set; } = null!;
    [PluginService] private IFramework   DalamudFramework { get; set; } = null!;
    [PluginService] private IPluginLog   PluginLog        { get; set; } = null!;
    [PluginService] private IChatGui     ChatGui          { get; set; } = null!;

    private IDalamudPluginInterface PluginInterface { get; init; }

    public Plugin(IDalamudPluginInterface plugin)
    {
        PluginInterface = plugin;
        plugin.Inject(this);

        UmbraDrawing.Initialize(plugin);
        Node.UseThreadedStyleComputation = true;

        RegisterServices();

        ClientState.Login  += OnLogin;
        ClientState.Logout += OnLogout;

        PluginInterface.UiBuilder.Draw += UpdateDevBar;

        if (ClientState.IsLoggedIn) OnLogin();
    }

    public void Dispose()
    {
        if (ClientState.IsLoggedIn) OnLogout(0, 0);

        ClientState.Login  -= OnLogin;
        ClientState.Logout -= OnLogout;

        PluginInterface.UiBuilder.Draw -= UpdateDevBar;

        UmbraDrawing.Dispose();
    }

    private void OnLogin()
    {
        Framework
           .Compile(DalamudFramework, PluginInterface, PlayerState.ContentId)
           .ContinueWith(task => {
                    if (task.IsFaulted) {
                        Logger.Error(
                            $"Umbra failed to initialize: {task.Exception.InnerException?.Message ?? task.Exception.Message}"
                        );
                    }
                }
            );
    }

    private void OnLogout(int type, int code)
    {
        Framework.Dispose();
    }

    private void RegisterServices()
    {
        Framework.AddLogTarget(new DefaultLogTarget(PluginLog, ChatGui));
        Framework.RegisterAssembly(Assembly.GetExecutingAssembly());
        Framework.RegisterAssembly(typeof(Game.EntryPoint).Assembly);
    }

    private void UpdateDevBar()
    {
        if (!PluginInterface.IsDevMenuOpen) return;

        if (ImGui.BeginMainMenuBar()) {
            if (ImGui.BeginMenu("Umbra")) {
                if (ImGui.MenuItem("Show Node Inspector", string.Empty, DrawingLib.ShowDebugWindow)) {
                    DrawingLib.ShowDebugWindow = !DrawingLib.ShowDebugWindow;
                }
                if (ImGui.MenuItem("Draw Bounding Boxes", string.Empty, Node.DrawDebugBoundingBoxes)) {
                    Node.DrawDebugBoundingBoxes = !Node.DrawDebugBoundingBoxes;
                }
                if (ImGui.MenuItem("Draw Reflow & Repaint Boxes", string.Empty, Node.DrawDebugPaintAndReflowBoxes)) {
                    Node.DrawDebugPaintAndReflowBoxes = !Node.DrawDebugPaintAndReflowBoxes;
                }

                ImGui.Separator();
                if (ImGui.MenuItem("Open config directory")) {
                    Process.Start(new ProcessStartInfo {
                        FileName        = PluginInterface.ConfigDirectory.FullName,
                        UseShellExecute = true,
                        Verb            = "open"
                    });
                }

                ImGui.Separator();
                ImGui.TextColored(new Vector4(1, 1, 1, 0.5f), "Umbra v" + Assembly.GetExecutingAssembly().GetName().Version);
                ImGui.SameLine();
                ImGui.Spacing();
                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();
        }
    }
}
