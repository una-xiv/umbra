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

namespace Umbra.Common;

public static class ServiceActivator
{
    private static readonly List<Type> CircularReferenceCheck = [];

    internal static void ClearCircularReferenceCheck()
    {
        CircularReferenceCheck.Clear();
    }
    
    public static T CreateInstance<T>()
    {
        return (T)CreateInstance(typeof(T), false);
    }

    internal static object CreateInstance(Type type, bool registerInstance = true)
    {
        var alias = type.GetInterface($"I{type.Name}") ?? type;

        if (CircularReferenceCheck.Contains(alias)) {
            throw new InvalidOperationException(
                $"Circular reference detected for {alias.Name}. Dependency chain: {string.Join(" -> ", CircularReferenceCheck.Select(t => t.Name))} -> {alias.Name}."
            );
        }

        CircularReferenceCheck.Add(alias);

        var constructor = type.GetConstructors().FirstOrDefault()
         ?? throw new InvalidOperationException($"No constructor found for {type.Name}.");

        object[] parameters = constructor.GetParameters().Select(p => ResolveType(type, p.ParameterType)).ToArray();

        try {
            object instance = Activator.CreateInstance(type, parameters)
               ?? throw new InvalidOperationException($"Failed to create instance of {type.Name}.");

            if (registerInstance) ServiceContainer.Instances[alias] = instance;

            return instance;
        } catch (Exception e) {
            if (e.InnerException != null) {
                throw new InvalidOperationException(
                    $"Failed to create instance of {type.Name}: {e.InnerException.Message} {e.InnerException.StackTrace}"
                );
            }
            throw new InvalidOperationException(
                $"Failed to create instance of {type.Name}: {e.Message} {e.StackTrace}"
            );
        } finally {
            CircularReferenceCheck.Remove(alias);
        }
    }

    private static object ResolveType(MemberInfo sourceType, Type type)
    {
        var alias = type.GetInterface($"I{type.Name}") ?? type;

        if (ServiceContainer.Instances.TryGetValue(alias, out var instance)) {
            return instance;
        }

        if (ServiceContainer.Definitions.TryGetValue(alias, out var definition)) {
            return CreateInstance(definition.Type);
        }

        if (type.IsArray
         && type.GetElementType() is { } elementType) {
            var deps  = ServiceContainer.Definitions.Values.Where(d => elementType.IsAssignableFrom(d.Type)).ToArray();
            var list  = Array.CreateInstance(elementType, deps.Length);
            var index = 0;

            foreach (var dep in deps) {
                list.SetValue(ResolveType(sourceType, dep.Type), index);
                index++;
            }

            return list;
        }

        // Test if the type implements an interface that follows the naming
        // convention of I{TypeName} and is registered as a service.
        var interfaceType = type.GetInterface($"I{type.Name}");

        if (interfaceType != null
         && ServiceContainer.Definitions.TryGetValue(interfaceType, out _)) {
            throw new InvalidOperationException(
                $"Service \"{sourceType.Name}\" depends on \"{type.Name}\", which is not a registered service. Did you mean to use \"{interfaceType.Name}\" instead?"
            );
        }

        throw new InvalidOperationException(
            $"Service {sourceType.Name} depends on {type.Name}, which is not a registered service."
        );
    }
}
