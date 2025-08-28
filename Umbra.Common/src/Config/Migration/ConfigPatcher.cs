using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Umbra.Common.Migration;

public class ConfigPatcher
{
    private readonly JToken _node;

    /// <summary>
    /// Constructs a ConfigPatcher from the given JSON string.
    /// </summary>
    /// <param name="json">The JSON string to patch.</param>
    public ConfigPatcher(string json)
    {
        _node = JToken.Parse(json);
    }

    /// <summary>
    /// Constructs a ConfigPatcher from the given JsonNode.
    /// </summary>
    /// <param name="node"></param>
    private ConfigPatcher(JToken node)
    {
        _node = node;
    }

    /// <summary>
    /// Returns true if the given JSON path exists in the document.
    /// </summary>
    /// <param name="jsonPath">The JSON path (RFC 9535).</param>
    /// <returns>True if the path exists, false otherwise.</returns>
    public bool Has(string jsonPath) => QueryToken(jsonPath) is not null;

    /// <summary>
    /// Returns true if the node at the given JSON path is an array.
    /// </summary>
    /// <param name="jsonPath">The JSON path (RFC 9535).</param>
    /// <returns>True if the node at the given path is an array, false otherwise.</returns>
    public bool IsArray(string jsonPath) => QueryToken(jsonPath) is JArray;

    /// <summary>
    /// Returns true if the node at the given JSON path is an object.
    /// </summary>
    /// <param name="jsonPath">The JSON path (RFC 9535).</param>
    /// <returns>True if the node at the given path is an object, false otherwise.</returns>
    public bool IsObject(string jsonPath) => QueryToken(jsonPath) is JObject;

    /// <summary>
    /// Returns true if the node at the given JSON path is a boolean.
    /// </summary>
    /// <param name="jsonPath">The JSON path (RFC 9535).</param>
    /// <returns>True if the node at the given path is a boolean, false otherwise.</returns>
    public bool IsBoolean(string jsonPath) => QueryToken(jsonPath) is JValue { Value: bool };

    /// <summary>
    /// Returns true if the node at the given JSON path is a string.
    /// </summary>
    /// <param name="jsonPath">The JSON path (RFC 9535).</param>
    /// <returns>True if the node at the given path is a string, false otherwise.</returns>
    public bool IsString(string jsonPath) => QueryToken(jsonPath) is JValue { Value: string };

    /// <summary>
    /// Returns true if the node at the given JSON path is a number.
    /// </summary>
    /// <param name="jsonPath">The JSON path (RFC 9535).</param>
    /// <returns>True if the node at the given path is a number, false otherwise.</returns>
    public bool IsNumber(string jsonPath) => QueryToken(jsonPath) is JValue { Value: int or uint or long or float or double or decimal };

    /// <summary>
    /// Returns a ConfigPatcher for the node at the given JSON path.
    /// </summary>
    /// <param name="jsonPath">The JSON path (RFC 9535).</param>
    /// <returns>A new <see cref="ConfigPatcher"/> instance.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public ConfigPatcher Get(string jsonPath)
    {
        var node = QueryToken(jsonPath) ?? throw new InvalidOperationException($"Path {jsonPath} not found.");
        return new ConfigPatcher(node);
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
        var parent     = FindOrCreate(parentPath, true);
        var nodeToSet  = value is not null ? JToken.FromObject(value) : JValue.CreateNull();

        switch (parent) {
            case JObject obj:
                obj[keyOrIndex] = nodeToSet;
                break;
            case JArray arr when int.TryParse(keyOrIndex, out var idx):
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
        var parent     = FindOrCreate(parentPath, false);

        switch (parent) {
            case JObject obj:
                obj.Remove(keyOrIndex);
                break;
            case JArray arr when int.TryParse(keyOrIndex, out var idx) && idx >= 0 && idx < arr.Count:
                arr.RemoveAt(idx);
                break;
            default:
                throw new InvalidOperationException($"Invalid delete target for {jsonPath}");
        }
    }

    /// <summary>
    /// Returns a list of keys of the root object.
    /// </summary>
    public List<string> Keys => KeysAt("");

    /// <summary>
    /// Returns the keys of the object at the given JSON path.
    /// </summary>
    /// <param name="jsonPath">The JSON path (RFC 9535).</param>
    /// <returns>A list of keys.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public List<string> KeysAt(string jsonPath)
    {
        var node = QueryToken(jsonPath);

        return node switch {
            null        => throw new InvalidOperationException($"Path '{jsonPath}' not found."),
            JObject obj => obj.Properties().Select(p => p.Name).ToList(),
            _           => throw new InvalidOperationException($"The node at path '{jsonPath}' is a {node.Type}, not an object.")
        };
    }

    /// <summary>
    /// Retrieves the value at the given JSON path and attempts to convert it
    /// to the specified type.
    /// </summary>
    /// <param name="jsonPath">The JSON path (RFC 9535).</param>
    /// <returns>The value at the given path.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public T? GetValue<T>(string jsonPath)
    {
        var node = QueryToken(jsonPath);

        if (node is null) throw new InvalidOperationException($"Path '{jsonPath}' not found.");

        try {
            return node.ToObject<T>();
        } catch (JsonException ex) {
            throw new InvalidOperationException($"Failed to convert value at path '{jsonPath}' to type {typeof(T).Name}: {ex.Message}");
        }
    }

    /// <summary>
    /// Replaces a key in the object at the given JSON path with a new key.
    /// </summary>
    /// <param name="jsonPath">The JSON path (RFC 9535).</param>
    /// <param name="oldKey">The name of the old key.</param>
    /// <param name="newKey">The name of the new key to replace the old one with.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public void ReplaceKey(string jsonPath, string oldKey, string newKey)
    {
        var node = QueryToken(jsonPath);

        if (node is JObject obj) {
            if (obj.TryGetValue(oldKey, out var value)) {
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
    public string ToJsonString() => _node.ToString(Formatting.Indented);

    private JToken? QueryToken(string jsonPath)
    {
        try
        {
            return _node.SelectToken(jsonPath);
        }
        catch (JsonException ex)
        {
            Logger.Error($"[Migration] Error querying JSON with path '{jsonPath}': {ex.Message}");
            return null;
        }
    }

    private JToken? FindOrCreate(string path, bool createIfMissing)
    {
        var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return _node;

        JToken? current = _node;

        for (int i = 0; i < parts.Length; i++) {
            var part = parts[i];

            if (current is null) return null;

            if (int.TryParse(part, out var idx) && current is JArray arr) {
                if (createIfMissing) {
                    while (arr.Count <= idx) {
                        bool nextPartIsIndex = (i + 1 < parts.Length) && int.TryParse(parts[i + 1], out _);
                        arr.Add(nextPartIsIndex ? new JArray() : new JObject());
                    }
                }

                current = arr.Count > idx ? arr[idx] : null;
            } else if (current is JObject obj) {
                if (!obj.TryGetValue(part, out var child)) {
                    if (!createIfMissing) return null;

                    bool nextPartIsIndex = (i + 1 < parts.Length) && int.TryParse(parts[i + 1], out _);
                    child     = nextPartIsIndex ? new JArray() : new JObject();
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
