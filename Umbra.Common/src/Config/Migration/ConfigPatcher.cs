using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;

namespace Umbra.Common.Migration;

public class ConfigPatcher
{
    private readonly JsonNode              _node;
    private readonly JsonSerializerOptions _options;

    /// <summary>
    /// Constructs a ConfigPatcher from the given JSON string.
    /// </summary>
    /// <param name="json">The JSON string to patch.</param>
    /// <param name="options"></param>
    public ConfigPatcher(string json, JsonSerializerOptions? options = null)
    {
        _node = JsonNode.Parse(json) ?? new JsonObject();
        _options = options ?? new JsonSerializerOptions {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
            WriteIndented    = true,
        };
    }

    /// <summary>
    /// Constructs a ConfigPatcher from the given JsonNode.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="options"></param>
    private ConfigPatcher(JsonNode node, JsonSerializerOptions options)
    {
        _node    = node;
        _options = options;
    }

    /// <summary>
    /// Returns true if the given JSON path exists in the document.
    /// </summary>
    /// <param name="jsonPath">The JSON path (slash-separated path notation).</param>
    /// <returns>True if the path exists, false otherwise.</returns>
    public bool Has(string jsonPath) => Navigate(jsonPath, false) is not null;

    /// <summary>
    /// Returns true if the node at the given JSON path is an array.
    /// </summary>
    /// <param name="jsonPath">The JSON path (slash-separated path notation).</param>
    /// <returns>True if the node at the given path is an array, false otherwise.</returns>
    public bool IsArray(string jsonPath) => Navigate(jsonPath, false) is JsonArray;

    /// <summary>
    /// Returns true if the node at the given JSON path is an object.
    /// </summary>
    /// <param name="jsonPath">The JSON path (slash-separated path notation).</param>
    /// <returns>True if the node at the given path is an object, false otherwise.</returns>
    public bool IsObject(string jsonPath) => Navigate(jsonPath, false) is JsonObject;

    /// <summary>
    /// Returns true if the node at the given JSON path is a boolean.
    /// </summary>
    /// <param name="jsonPath">The JSON path (slash-separated path notation).</param>
    /// <returns>True if the node at the given path is a boolean, false otherwise.</returns>
    public bool IsBoolean(string jsonPath) => Navigate(jsonPath, false) is JsonValue v && v.TryGetValue(out bool _);

    /// <summary>
    /// Returns true if the node at the given JSON path is a string.
    /// </summary>
    /// <param name="jsonPath">The JSON path (slash-separated path notation).</param>
    /// <returns>True if the node at the given path is a string, false otherwise.</returns>
    public bool IsString(string jsonPath) => Navigate(jsonPath, false) is JsonValue v && v.TryGetValue(out string? _);

    /// <summary>
    /// Returns true if the node at the given JSON path is a number.
    /// </summary>
    /// <param name="jsonPath">The JSON path (slash-separated path notation).</param>
    /// <returns>True if the node at the given path is a number, false otherwise.</returns>
    public bool IsNumber(string jsonPath) => Navigate(jsonPath, false) is JsonValue v && v.TryGetValue(out double _);

    /// <summary>
    /// Returns a ConfigPatcher for the node at the given JSON path.
    /// </summary>
    /// <param name="jsonPath">The JSON path (slash-separated path notation).</param>
    /// <returns>A new <see cref="ConfigPatcher"/> instance.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public ConfigPatcher Get(string jsonPath)
    {
        var node = Navigate(jsonPath, true) ?? throw new InvalidOperationException($"Path {jsonPath} not found.");
        return new ConfigPatcher(node, _options);
    }

    /// <summary>
    /// Sets the value at the given JSON path. Creates intermediate
    /// objects/arrays as needed.
    /// </summary>
    /// <param name="jsonPath">The JSON path (slash-separated path notation).</param>
    /// <param name="value">The value to set. Can be null.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Set(string jsonPath, object? value)
    {
        var parentPath = GetParentPath(jsonPath, out var keyOrIndex);
        var parent     = Navigate(parentPath, true);

        // Create the JsonNode from the input value.
        var nodeToSet = JsonSerializer.SerializeToNode(value);

        switch (parent) {
            case JsonObject obj:
                obj[keyOrIndex] = nodeToSet;
                break;
            case JsonArray arr when int.TryParse(keyOrIndex, out var idx):
                if (arr.Count <= idx) {
                    throw new InvalidOperationException($"Index {idx} out of bounds for array at path '{parentPath}'");
                }

                arr[idx] = nodeToSet;
                break;
            default:
                throw new InvalidOperationException($"Invalid parent node at path '{parentPath}' for target '{jsonPath}'");
        }
    }

    /// <summary>
    /// Deletes a node at the given JSON path.
    /// </summary>
    /// <param name="jsonPath">The JSON path (slash-separated path notation).</param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Delete(string jsonPath)
    {
        var parentPath = GetParentPath(jsonPath, out var keyOrIndex);
        var parent     = Navigate(parentPath, false);

        switch (parent) {
            case JsonObject obj:
                obj.Remove(keyOrIndex);
                break;
            case JsonArray arr when int.TryParse(keyOrIndex, out var idx) && idx >= 0 && idx < arr.Count:
                arr.RemoveAt(idx);
                break;
            default:
                throw new InvalidOperationException($"Invalid delete target for {jsonPath}");
        }
    }

    /// <summary>
    /// Returns the keys of the object at the given JSON path.
    /// </summary>
    /// <param name="jsonPath">The JSON path (slash-separated path notation).</param>
    /// <returns>A list of keys.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public List<string> KeysAt(string jsonPath)
    {
        var node = Navigate(jsonPath, false);

        if (node is JsonObject obj) {
            return obj.Select(kv => kv.Key).ToList();
        }

        throw new InvalidOperationException($"Path {jsonPath} is not an object.");
    }

    /// <summary>
    /// Retrieves the value at the given JSON path and attempts to convert it
    /// to the specified type.
    /// </summary>
    /// <param name="jsonPath">The JSON path (slash-separated path notation).</param>
    /// <typeparam name="T">The type to convert the value to.</typeparam>
    /// <returns>The value at the given path.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public T? GetValue<T>(string jsonPath)
    {
        var node = Navigate(jsonPath, false);

        if (node is null) throw new InvalidOperationException($"Path '{jsonPath}' not found.");
        if (node is JsonValue val && val.TryGetValue<T>(out var value)) return value;

        try {
            return node.Deserialize<T>(_options);
        } catch (Exception ex) {
            throw new InvalidOperationException($"Failed to convert value at path '{jsonPath}' to type {typeof(T).Name}: {ex.Message}");
        }
    }

    /// <summary>
    /// Replaces a key in the object at the given JSON path with a new key.
    /// </summary>
    /// <param name="jsonPath">The JSON path (slash-separated path notation).</param>
    /// <param name="oldKey">The name of the old key.</param>
    /// <param name="newKey">The name of the new key to replace the old one with.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public void ReplaceKey(string jsonPath, string oldKey, string newKey)
    {
        var node = Navigate(jsonPath, false);

        if (node is JsonObject obj) {
            if (obj.TryGetPropertyValue(oldKey, out var value)) {
                obj.Remove(oldKey);
                obj[newKey] = value;
            } else {
                throw new InvalidOperationException($"Key {oldKey} not found at path {jsonPath}.");
            }
        } else {
            throw new InvalidOperationException($"Path {jsonPath} is not an object.");
        }
    }

    /// <summary>
    /// Converts the patched JSON document back to a string.
    /// </summary>
    /// <returns>The patched JSON string.</returns>
    public string ToJsonString() => _node.ToJsonString(_options);

    private JsonNode? Navigate(string path, bool createIfMissing)
    {
        var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return _node;

        JsonNode? current = _node;

        for (int i = 0; i < parts.Length; i++) {
            var part = parts[i];

            if (current is null) return null;

            if (int.TryParse(part, out var idx) && current is JsonArray arr) {
                if (createIfMissing) {
                    while (arr.Count <= idx) {
                        bool nextPartIsIndex = (i + 1 < parts.Length) && int.TryParse(parts[i + 1], out _);
                        arr.Add(nextPartIsIndex ? new JsonArray() : new JsonObject());
                    }
                }

                current = arr.Count > idx ? arr[idx] : null;
            } else if (current is JsonObject obj) {
                if (!obj.TryGetPropertyValue(part, out var child)) {
                    if (!createIfMissing) return null;

                    bool nextPartIsIndex = (i + 1 < parts.Length) && int.TryParse(parts[i + 1], out _);
                    child     = nextPartIsIndex ? new JsonArray() : new JsonObject();
                    obj[part] = child;
                }

                current = child;
            } else {
                return null;
            }
        }

        return current;
    }

    private static string GetParentPath(string fullPath, out string last)
    {
        var idx = fullPath.LastIndexOf('/');
        if (idx == -1) {
            last = fullPath;
            return "";
        }

        last = fullPath[(idx + 1)..];
        return fullPath[..idx];
    }
}
