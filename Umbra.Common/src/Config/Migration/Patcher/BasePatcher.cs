using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;

namespace Umbra.Common.Migration.Patcher;

public abstract class BasePatcher<T> where T : JsonNode
{
    protected T Node { get; }

    protected BasePatcher(string jsonString)
    {
        var node = JsonNode.Parse(jsonString);

        if (node is not T obj) {
            throw new InvalidOperationException($"Failed to parse JSON string into {typeof(T)}.");
        }

        Node = obj;
    }

    protected BasePatcher(T node)
    {
        Node = node;
    }

    /// <summary>
    /// Outputs the current JSON structure as a string.
    /// </summary>
    /// <param name="indented">Whether to indent the output for readability.</param>
    /// <returns>The generarted JSON string.</returns>
    public string ToJsonString(bool indented = false)
    {
        return Node.ToJsonString(new JsonSerializerOptions(JsonSerializerDefaults.General) {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
            WriteIndented    = indented
        });
    }

    /// <summary>
    /// Returns an <see cref="ObjectPatcher"/> instance for the node at the
    /// specified path.
    /// </summary>
    /// <param name="path">The path to the nods in slash-notation.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public ObjectPatcher GetObject(string path)
    {
        JsonNode? node = Navigate(path);

        if (node is not JsonObject obj) {
            throw new InvalidOperationException($"Node at path '{path}' does not exist or is not an object.");
        }

        return new ObjectPatcher(obj);
    }

    /// <summary>
    /// Returns an <see cref="ArrayPatcher"/> instance for the node at the
    /// specified path.
    /// </summary>
    /// <param name="path">The path to the node in slash-notation.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public ArrayPatcher GetArray(string path)
    {
        JsonNode? node = Navigate(path);

        if (node is not JsonArray arr) {
            throw new InvalidOperationException($"Node at path '{path}' does not exist or is not an array.");
        }

        return new ArrayPatcher(arr);
    }

    protected JsonNode? NavigateOrNull(string   path) => Navigate(path, false);
    protected JsonNode  NavigateOrCreate(string path) => Navigate(path, true) ?? throw new InvalidOperationException($"Failed to create missing path: {path}");

    protected static string Deflate64(string text, string? prefix = null)
    {
        byte[]    bytes  = Encoding.UTF8.GetBytes(text);
        using var output = new MemoryStream();

        using (var deflateStream = new DeflateStream(output, CompressionLevel.SmallestSize)) {
            deflateStream.Write(bytes, 0, bytes.Length);
        }

        bytes = output.ToArray();

        string result = Convert.ToBase64String(bytes);

        return !string.IsNullOrEmpty(prefix) ? prefix + result : result;
    }

    protected static string? Inflate64(string text, string? prefix = null)
    {
        if (!string.IsNullOrEmpty(prefix)) {
            if (!text.StartsWith(prefix)) {
                return null;
            }

            text = text[prefix.Length..];
        }

        byte[]    bytes  = Convert.FromBase64String(text);
        using var input  = new MemoryStream(bytes);
        using var output = new MemoryStream();

        using (var deflateStream = new DeflateStream(input, CompressionMode.Decompress)) {
            deflateStream.CopyTo(output);
        }

        return Encoding.UTF8.GetString(output.ToArray());
    }

    private JsonNode? Navigate(string path, bool createIfMissing = false)
    {
        var       segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        JsonNode? current  = Node;

        foreach (var segment in segments) {
            switch (current) {
                case null:
                    if (createIfMissing) {
                        var newObj = new JsonObject();
                        current = newObj;
                    } else {
                        return null;
                    }

                    break;

                case JsonObject obj: {
                    if (!obj.TryGetPropertyValue(segment, out var next)) {
                        if (createIfMissing) {
                            var newObj = new JsonObject();
                            obj[segment] = newObj;
                            current      = newObj;
                        } else {
                            return null;
                        }
                    } else {
                        current = next;
                    }

                    break;
                }
                case JsonArray arr when int.TryParse(segment, out int index): {
                    if (index >= 0 && index < arr.Count) {
                        current = arr[index];
                    } else {
                        if (createIfMissing) {
                            var newObj = new JsonObject();
                            while (arr.Count <= index) arr.Add(null);
                            arr[index] = newObj;
                            current    = newObj;
                        } else {
                            return null;
                        }
                    }

                    break;
                }
                case JsonArray:
                    throw new InvalidOperationException($"Invalid array index '{segment}' in path '{path}'.");
                default:
                    throw new InvalidOperationException($"Cannot navigate through non-container node at segment '{segment}' in path '{path}'.");
            }
        }

        return current;
    }

    protected static string GetParentPath(string fullPath, out string last)
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
