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
using Dalamud.Plugin.Services;
using System.Diagnostics;

namespace Umbra.Common;

internal static class Scheduler
{
    internal static bool  EnableHitchWarnings  { get; set; } = false;
    internal static float TickHitchThresholdMs { get; set; } = 50.0f;
    internal static float DrawHitchThresholdMs { get; set; } = 16.0f;

    private static List<TickHandler> _onTickHandlers = [];
    private static List<DrawHandler> _onDrawHandlers = [];

    private static bool _isRunning;

    [WhenFrameworkCompiling]
    internal static void OnFrameworkCompiling()
    {
        _onTickHandlers = GetOnTickHandlers();
        _onDrawHandlers = GetOnDrawHandlers();
    }

    [WhenFrameworkDisposing]
    internal static void OnFrameworkDisposing()
    {
        _onTickHandlers.Clear();
        _onDrawHandlers.Clear();
    }

    /// <summary>
    /// Start the scheduler.
    /// </summary>
    /// <remarks>
    /// Tick and Draw handlers rely on registered services. Therefore, this method
    /// must be invoked after the service container has been compiled.
    /// </remarks>
    internal static void Start()
    {
        Framework.DalamudFramework.Update      += OnUmbraTick;
        Framework.DalamudPlugin.UiBuilder.Draw += OnUmbraDraw;
        _isRunning                             =  true;
    }

    /// <summary>
    /// Stop the scheduler.
    /// </summary>
    internal static void Stop()
    {
        Framework.DalamudFramework.Update      -= OnUmbraTick;
        Framework.DalamudPlugin.UiBuilder.Draw -= OnUmbraDraw;
        _isRunning                             =  false;
    }

    private static void OnUmbraTick(IFramework fw)
    {
        fw.RunOnFrameworkThread(
            () => {
                foreach (var handler in _onTickHandlers) {
                    handler.Invoke(fw.UpdateDelta.TotalMilliseconds);
                    if (!_isRunning) return;
                }
            }
        );
    }

    private static void OnUmbraDraw()
    {
        foreach (var handler in _onDrawHandlers) {
            handler.Invoke();
            if (!_isRunning) return;
        }
    }

    private static List<TickHandler> GetOnTickHandlers()
    {
        return Framework
            .Assemblies.SelectMany(assembly => assembly.GetTypes())
            .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            .Where(method => method.GetCustomAttribute<OnTickAttribute>() != null)
            .Select(
                method => {
                    var type     = method.DeclaringType!;
                    var interval = method.GetCustomAttribute<OnTickAttribute>()!.Interval;

                    return new TickHandler(method, type.GetInterface($"I{type.Name}") ?? type, interval);
                }
            )
            .ToList();
    }

    private static List<DrawHandler> GetOnDrawHandlers()
    {
        return [
            .. Framework
                .Assemblies.SelectMany(assembly => assembly.GetTypes())
                .SelectMany(
                    type => type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                )
                .Where(method => method.GetCustomAttribute<OnDrawAttribute>() != null)
                .Select(
                    method => {
                        var type = method.DeclaringType!;
                        var conf = method.GetCustomAttribute<OnDrawAttribute>()!;

                        return new DrawHandler(method, type.GetInterface($"I{type.Name}") ?? type, conf.ExecutionOrder);
                    }
                )
                .OrderBy(handler => handler.ExecutionOrder)
        ];
    }

    private class TickHandler(MethodBase method, Type serviceType, long interval) : IDisposable
    {
        private bool       _isRunning = true;
        private long       _elapsedTime;
        private object?    _instance;
        private Stopwatch? _stopwatch = new();

        public void Dispose()
        {
            _stopwatch?.Stop();
            _stopwatch = null;
            _isRunning = false;
        }

        public void Invoke(double deltaTime)
        {
            if (!_isRunning) return;

            if (_instance == null) {
                if (!ServiceContainer.Definitions.TryGetValue(serviceType, out var definition)) {
                    _isRunning = false;

                    throw new InvalidOperationException(
                        $"Class {serviceType.Name} has an OnTick method, but the class is not a registered service."
                    );
                }

                _instance = ServiceContainer.GetInstance(definition.Type);
            }

            _elapsedTime += (long)deltaTime;
            if (_elapsedTime < interval) return;

            _stopwatch?.Start();
            method.Invoke(_instance, null);

            if (EnableHitchWarnings && _stopwatch?.ElapsedMilliseconds > TickHitchThresholdMs) {
                Logger.Warning(
                    $"Tick hitch detected of {_stopwatch.ElapsedMilliseconds}ms in \"{serviceType.Name}.{method.Name}\"."
                );
            }

            _stopwatch?.Reset();
            _elapsedTime = 0;
        }
    }

    private class DrawHandler(MethodBase method, Type serviceType, int executionOrder) : IDisposable
    {
        public readonly int ExecutionOrder = executionOrder;

        private bool       _isRunning = true;
        private object?    _instance;
        private Stopwatch? _stopwatch = new();

        public void Dispose()
        {
            _stopwatch?.Stop();
            _stopwatch = null;
            _isRunning = false;
        }

        public void Invoke()
        {
            if (!_isRunning) return;

            if (_instance == null) {
                if (!ServiceContainer.Definitions.TryGetValue(serviceType, out var definition)) {
                    _isRunning = false;

                    throw new InvalidOperationException(
                        $"Class {serviceType.Name} has an OnDraw method, but the class is not a registered service."
                    );
                }

                _instance = ServiceContainer.GetInstance(definition.Type);
            }

            _stopwatch?.Start();
            method.Invoke(_instance, null);

            if (EnableHitchWarnings && _stopwatch?.ElapsedMilliseconds > DrawHitchThresholdMs) {
                Logger.Warning(
                    $"Draw hitch detected of {_stopwatch.ElapsedMilliseconds}ms in \"{serviceType.Name}.{method.Name}\"."
                );
            }

            _stopwatch?.Reset();
        }
    }
}
