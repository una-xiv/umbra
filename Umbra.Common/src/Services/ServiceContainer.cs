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
using Umbra.Common.Services;

namespace Umbra.Common;

internal static class ServiceContainer
{
    internal static readonly Dictionary<Type, Definition> Definitions = [];
    internal static readonly Dictionary<Type, object> Instances = [];

    internal static void SetInstance(Type type, object instance)
    {
        Instances[type] = instance;
    }

    /// <summary>
    /// Gets the instance of the service.
    /// </summary>
    /// <remarks>
    /// An instance will be created if it does not already exist. The given type
    /// must be a valid registered service definition.
    /// </remarks>
    /// <exception cref="ArgumentException"></exception>
    internal static object GetInstance(Type type)
    {
        Type t = type.GetInterface($"I{type.Name}") ?? type;

        if (!Instances.TryGetValue(t, out object? instance)) {
            if (!Definitions.TryGetValue(t, out Definition? definition))
                throw new ArgumentException($"Service \"{type.Name}\" not found.");

            Instances[t] = instance = ServiceActivator.CreateInstance(definition.Type);
        }

        return instance;
    }

    /// <summary>
    /// Compiles the services into instances.
    /// </summary>
    [WhenFrameworkCompiling(executionOrder: int.MaxValue - 1)]
    internal static void Compile()
    {
        foreach (Type alias in Definitions.Keys) {
            if (Instances.ContainsKey(alias))
                continue;

            ServiceActivator.CreateInstance(Definitions[alias].Type);
        }
    }

    /// <summary>
    /// Registers all service definitions from the assemblies.
    /// </summary>
    /// <exception cref="Exception"></exception>
    [WhenFrameworkCompiling]
    internal static void RegisterDefinitionsFromAssemblies()
    {
        var types = Framework.Assemblies.SelectMany(assembly => assembly.GetTypes()).Where(type => type.GetCustomAttribute<ServiceAttribute>() != null).ToList();

        foreach (Type type in types)
        {
            var alias = type.GetInterface($"I{type.Name}") ?? type;

            if (Definitions.ContainsKey(alias))
                throw new Exception($"Another service with name \"{type.Name}\" already exists.");

            if (type.GetConstructors().Length == 0)
                throw new Exception($"Service \"{type.Name}\" does not have a constructor.");

            Definitions[alias] = new Definition(type);
        }
    }

    [WhenFrameworkDisposing]
    internal static void Dispose()
    {
        foreach (var instance in Instances.Values)
        {
            if (instance is IDisposable disposable)
                disposable.Dispose();
        }

        Definitions.Clear();
        Instances.Clear();
    }
}
