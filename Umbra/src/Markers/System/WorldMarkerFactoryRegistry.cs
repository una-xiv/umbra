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
using System.IO.Compression;
using System.Text;
using Newtonsoft.Json;
using Umbra.Common;

namespace Umbra.Markers.System;

[Service]
internal class WorldMarkerFactoryRegistry
{
    [ConfigVariable("MarkerSettings")] private static string MarkerSettings { get; set; } = "";

    private Dictionary<string, WorldMarkerFactory>         Factories { get; } = [];
    private Dictionary<string, Dictionary<string, object>> CfgValues { get; } = [];

    public WorldMarkerFactoryRegistry(WorldMarkerFactory[] factories)
    {
        try {
            if (MarkerSettings != "") {
                var json   = Decode(MarkerSettings);
                var values = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(json);

                if (null != values) {
                    CfgValues = values;
                }
            }
        } catch {
            Logger.Warning("Failed to restore marker configuration.");
        }

        foreach (WorldMarkerFactory factory in factories) {
            Factories[factory.Id] = factory;

            factory.Setup(CfgValues.GetValueOrDefault(factory.Id, new()));
            factory.OnConfigValueChanged += OnConfigChanged;
        }
    }

    public List<string> GetFactoryIds()
    {
        return [..Factories.Keys];
    }

    public WorldMarkerFactory GetFactory(string id)
    {
        return Factories.TryGetValue(id, out WorldMarkerFactory? factory)
            ? factory
            : throw new KeyNotFoundException($"No factory found for ID: {id}");
    }

    private void OnConfigChanged()
    {
        Dictionary<string, Dictionary<string, object>> config = new();

        foreach ((string id, WorldMarkerFactory factory) in Factories) {
            config[id] = factory.GetUserConfig();
        }

        ConfigManager.Set("MarkerSettings", Encode(JsonConvert.SerializeObject(config)));
    }

    private static string Encode(string text)
    {
        byte[]    bytes  = Encoding.UTF8.GetBytes(text);
        using var output = new MemoryStream();

        using (var deflateStream = new DeflateStream(output, CompressionLevel.SmallestSize)) {
            deflateStream.Write(bytes, 0, bytes.Length);
        }

        bytes = output.ToArray();

        return Convert.ToBase64String(bytes);
    }

    private static string Decode(string text)
    {
        byte[]    bytes  = Convert.FromBase64String(text);
        using var input  = new MemoryStream(bytes);
        using var output = new MemoryStream();

        using (var deflateStream = new DeflateStream(input, CompressionMode.Decompress)) {
            deflateStream.CopyTo(output);
        }

        return Encoding.UTF8.GetString(output.ToArray());
    }
}
