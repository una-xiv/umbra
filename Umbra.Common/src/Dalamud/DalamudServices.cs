/* Umbra.Common | (c) 2024 by Una       ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Common is free software: you can        \/     \/             \/
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General private License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Common is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General private License for more details.
 */

using Dalamud.Game;
using Dalamud.Game.ClientState.Objects;
using Dalamud.IoC;

// ReSharper disable UnusedMember.Local

namespace Umbra.Common;

#pragma warning disable SeStringEvaluator

internal sealed class DalamudServices
{
    [PluginService] private IPluginLog Log { get; init; } = null!;

    [PluginService] private IAetheryteList AetheryteList { get; init; } = null!;

    [PluginService] private IChatGui ChatGui { get; init; } = null!;

    [PluginService] private IClientState ClientState { get; init; } = null!;

    [PluginService] private ICommandManager CommandManager { get; init; } = null!;

    [PluginService] private ICondition Condition { get; init; } = null!;

    [PluginService] private IDataManager DataManager { get; init; } = null!;

    [PluginService] private IDtrBar DtrBar { get; init; } = null!;

    [PluginService] private IDutyState DutyState { get; init; } = null!;

    [PluginService] private IFateTable FateTable { get; init; } = null!;

    [PluginService] private IFlyTextGui FlyTextGui { get; init; } = null!;

    [PluginService] private IGameConfig GameConfig { get; init; } = null!;

    [PluginService] private IGameGui GameGui { get; init; } = null!;

    [PluginService] private IGameInteropProvider GameInteropProvider { get; init; } = null!;

    [PluginService] private IGameLifecycle GameLifecycle { get; init; } = null!;

    [PluginService] private IJobGauges JobGauges { get; init; } = null!;

    [PluginService] private IKeyState KeyState { get; init; } = null!;

    [PluginService] private INotificationManager NotificationManager { get; init; } = null!;

    [PluginService] private IObjectTable ObjectTable { get; init; } = null!;

    [PluginService] private IPartyFinderGui PartyFinderGui { get; init; } = null!;

    [PluginService] private IPartyList PartyList { get; init; } = null!;

    [PluginService] private ITargetManager TargetManager { get; init; } = null!;

    [PluginService] private ITextureProvider TextureProvider { get; init; } = null!;

    [PluginService] private ITextureSubstitutionProvider TextureSubstitutionProvider { get; init; } = null!;

    [PluginService] private ITitleScreenMenu TitleScreenMenu { get; init; } = null!;

    [PluginService] private IToastGui ToastGui { get; init; } = null!;

    [PluginService] private ISeStringEvaluator SeStringEvaluator { get; init; } = null!;

    [PluginService] private ISigScanner SigScanner { get; init; } = null!;

    [WhenFrameworkCompiling]
    internal static void Initialize()
    {
        DalamudServices container = new();
        Framework.DalamudPlugin.Inject(container);

        typeof(DalamudServices)
            .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic)
            .ToList()
            .ForEach(
                property => {
                    if (property.GetCustomAttribute<PluginServiceAttribute>() == null) return;

                    var inst = property.GetValue(container)
                        ?? throw new InvalidOperationException($"Service {property.Name} is null.");

                    ServiceContainer.SetInstance(property.PropertyType, inst);
                }
            );
    }
}
