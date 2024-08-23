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

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using Dalamud.Plugin;
using ImGuiNET;
using Umbra.Common;
using Umbra.Game;
using Una.Drawing;

namespace Umbra.Plugins;

internal class PluginLoadContext(DirectoryInfo directoryInfo) : AssemblyLoadContext(true)
{
    private static Dictionary<string, Assembly> KnownAssemblies { get; } = new() {
        ["Umbra"]              = typeof(Plugin).Assembly,
        ["Umbra.Common"]       = typeof(Framework).Assembly,
        ["Umbra.Game"]         = typeof(IPlayer).Assembly,
        ["Una.Drawing"]        = typeof(Node).Assembly,
        ["Dalamud"]            = typeof(IDalamudPluginInterface).Assembly,
        ["Lumina"]             = typeof(Lumina.GameData).Assembly,
        ["Lumina.Excel"]       = typeof(Lumina.Excel.GeneratedSheets.Action).Assembly,
        ["ImGuiNET"]           = typeof(ImGui).Assembly,
        ["FFXIVClientStructs"] = typeof(FFXIVClientStructs.FFXIV.Client.System.Framework.Framework).Assembly,
    };

    internal Assembly LoadFromFile(string filePath)
    {
        Assembly assembly = LoadFromFileInternal(filePath);

        bool hasUmbraReference = false;

        foreach (var name in assembly.GetReferencedAssemblies()) {
            if (null == name.Name || !KnownAssemblies.TryGetValue(name.Name, out var referencedKnownAssembly)) continue;

            if (name.Name == "Umbra") hasUmbraReference = true;

            ValidateReferencedAssembly(name, referencedKnownAssembly.GetName());
        }

        if (!hasUmbraReference) {
            throw new ("This is not an Umbra plugin.");
        }

        return assembly;
    }

    private Assembly LoadFromFileInternal(string filePath)
    {
        using var file    = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        string    pdbPath = Path.ChangeExtension(filePath, ".pdb");

        if (!File.Exists(pdbPath)) return LoadFromStream(file);

        using var pdbFile = File.Open(pdbPath, FileMode.Open, FileAccess.Read, FileShare.Read);

        return LoadFromStream(file, pdbFile);
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        if (assemblyName.Name != null && KnownAssemblies.TryGetValue(assemblyName.Name, out Assembly? forwardedRef)) {
            return forwardedRef;
        }

        string   filePath = Path.Join(directoryInfo.FullName, $"{assemblyName.Name}.dll");
        FileInfo file     = new(filePath);

        if (!file.Exists) return base.Load(assemblyName);

        try {
            return LoadFromFileInternal(file.FullName);
        } catch (Exception e) {
            Logger.Error($"Failed to load {assemblyName.Name} from {file.FullName}: {e.Message}");
        }

        return base.Load(assemblyName);
    }

    private static void ValidateReferencedAssembly(AssemblyName usedAsm, AssemblyName refAsm)
    {
        // We're only interested in Umbra assemblies.
        if (!usedAsm.Name?.StartsWith("Umbra") ?? false) return;

        int usedMa = usedAsm.Version?.Major ?? 0;
        int refMa  = refAsm.Version?.Major ?? 0;
        int usedMi = usedAsm.Version?.Minor ?? 0;
        int refMi  = refAsm.Version?.Minor ?? 0;

        if (usedMa < refMa || usedMi < refMi) {
            // I18N isn't available here.
            throw new ($"This plugin cannot be loaded because it was made using {refAsm.Name} version {usedMa}.{usedMi}, but {refMa}.{refMi} is required. This plugin should be updated by the author.");
        }
    }
}
