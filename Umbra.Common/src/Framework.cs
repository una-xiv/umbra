/* Umbra.Common | (c) 2024 by Una       ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Common is free software: you can        \/     \/             \/
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General Public License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Common is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace Umbra.Common;

public static class Framework
{
    public static IDalamudPluginInterface DalamudPlugin    { get; private set; } = null!;
    public static IFramework              DalamudFramework { get; private set; } = null!;
    public static ulong                   LocalCharacterId { get; private set; }

    /// <summary>
    /// List of assemblies registered with the Umbra Framework.
    /// </summary>
    public static readonly List<Assembly> Assemblies = [
        Assembly.GetExecutingAssembly()
    ];

    /// <summary>
    /// Runs all compilation procedures and starts registered services.
    /// </summary>
    /// <remarks>
    /// This method should be called after all assemblies have been registered
    /// with the framework using the <see cref="RegisterAssembly"/> method.
    /// </remarks>
    public static async Task Compile(IFramework dalamudFramework, IDalamudPluginInterface dalamudPlugin, ulong charId)
    {
        DalamudPlugin    = dalamudPlugin;
        DalamudFramework = dalamudFramework;
        LocalCharacterId = charId;

        // Always make sure config is loaded first.
        ConfigManager.Initialize();

        foreach (var initializer in GetMethodInfoListWith<WhenFrameworkAsyncCompilingAttribute>()) {
            await (Task)initializer.Invoke(null, null)!;
        }

        await dalamudFramework.RunOnFrameworkThread(
            () => {
                foreach (var initializer in GetMethodInfoListWith<WhenFrameworkCompilingAttribute>()) {
                    try {
                        initializer.Invoke(null, null);
                    } catch (Exception e) {
                        Logger.Error(
                            $"Failed to run initializer {initializer.DeclaringType?.Name}::{initializer.Name}: {e}"
                        );
                    }
                }
            }
        );

        Scheduler.Start();
    }

    /// <summary>
    /// Disposes of all services and resources registered with the framework.
    /// </summary>
    public static void Dispose()
    {
        Scheduler.Stop();

        GetMethodInfoListWith<WhenFrameworkDisposingAttribute>().ForEach(method => method.Invoke(null, null));

        ConfigManager.Dispose();
    }

    public static async void Restart()
    {
        Dispose();
        await Task.Delay(500);
        await Compile(DalamudFramework, DalamudPlugin, LocalCharacterId);
    }

    /// <summary>
    /// Enables logging of hitch warnings of the scheduler in the console.
    /// </summary>
    public static void SetSchedulerHitchWarnings(bool enabled, float tickThresholdMs, float drawThresholdMs)
    {
        Scheduler.EnableHitchWarnings  = enabled;
        Scheduler.TickHitchThresholdMs = tickThresholdMs;
        Scheduler.DrawHitchThresholdMs = drawThresholdMs;
    }

    /// <summary>
    /// Instantiates the given class with its dependencies injected.
    /// </summary>
    public static T InstantiateWithDependencies<T>()
    {
        return ServiceActivator.CreateInstance<T>();
    }

    /// <summary>
    /// Instantiates the given class with its dependencies injected.
    /// </summary>
    public static object InstantiateWithDependencies(Type type)
    {
        return ServiceActivator.CreateInstance(type, false);
    }

    /// <summary>
    /// Returns an instance of the given service type.
    /// </summary>
    public static T Service<T>()
    {
        return Service<T>(typeof(T));
    }

    public static T Service<T>(Type type)
    {
        var alias = type.GetInterface($"I{type.Name}") ?? type;

        if (ServiceContainer.Instances.TryGetValue(alias, out var service)) {
            return (T)service;
        }

        if (ServiceContainer.Definitions.TryGetValue(alias, out var definition)) {
            return (T)ServiceActivator.CreateInstance(definition.Type);
        }

        throw new InvalidOperationException($"Service \"{type.Name}\" not found.");
    }

    /// <summary>
    /// Register an assembly with the service container.
    /// </summary>
    /// <param name="assembly"></param>
    public static void RegisterAssembly(Assembly assembly)
    {
        if (Assemblies.Contains(assembly)) return;

        Assemblies.Add(assembly);
    }

    /// <summary>
    /// Adds a target to send log messages to that are sent by the
    /// <see cref="Logger"/> instance.
    /// </summary>
    public static void AddLogTarget(ILogTarget logger)
    {
        Logger.AddLogTarget(logger);
    }

    private static List<MethodInfo> GetMethodInfoListWith<T>() where T : Attribute
    {
        List<MethodInfo> methods = Assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .SelectMany(type => type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            .Where(method => method.GetCustomAttributes<T>().Any())
            .ToList();

        if (typeof(T).GetInterfaces().Contains(typeof(IExecutionOrderAware))) {
            methods.Sort(
                (a, b) => {
                    var orderA = a.GetCustomAttribute<T>() as IExecutionOrderAware;
                    var orderB = b.GetCustomAttribute<T>() as IExecutionOrderAware;

                    return orderA!.ExecutionOrder.CompareTo(orderB!.ExecutionOrder);
                }
            );
        }

        return methods;
    }
}
