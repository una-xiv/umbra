using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;

namespace Umbra.Common.Migration.Patcher;

public class ObjectPatcher : BasePatcher<JsonObject>
{
    public ObjectPatcher(string jsonString) : base(jsonString)
    {
    }

    public ObjectPatcher(JsonObject node) : base(node)
    {
    }

    public bool ContainsKey(string path) => NavigateOrNull(path) is not null;
    public bool IsString(string    path) => NavigateOrNull(path) is JsonValue value && value.TryGetValue<string>(out _);
    public bool IsNumber(string    path) => NavigateOrNull(path) is JsonValue value && value.TryGetValue<double>(out _);
    public bool IsBoolean(string   path) => NavigateOrNull(path) is JsonValue value && value.TryGetValue<bool>(out _);
    public bool IsObject(string    path) => NavigateOrNull(path) is JsonObject;
    public bool IsArray(string     path) => NavigateOrNull(path) is JsonArray;

    public IEnumerable<string> Keys { get => Node.Select(kvp => kvp.Key); }

    /// <summary>
    /// Returns a value of the given type at the specified path, or the
    /// provided default value if the path does not exist or the value cannot
    /// be converted.
    /// </summary>
    /// <param name="path">The path to the value in slash-notation.</param>
    /// <param name="defaultValue">An optional default value if the requested element does not exist.</param>
    /// <typeparam name="T">The type of the returned object.</typeparam>
    [return: NotNullIfNotNull(nameof(defaultValue))]
    public T? GetValue<T>(string path, T? defaultValue = default)
    {
        JsonNode? node = NavigateOrNull(path);

        if (node is JsonValue value && value.TryGetValue<T>(out var result)) {
            return result;
        }

        return defaultValue;
    }

    /// <summary>
    /// Stores the given value at the specified path, creating any necessary
    /// parent objects along the way.
    /// </summary>
    /// <param name="path">The path that specifies where to store the value.</param>
    /// <param name="value">The value to store.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Set(string path, object? value)
    {
        var parent = NavigateOrCreate(GetParentPath(path, out var key));

        if (parent is not JsonObject obj) {
            throw new InvalidOperationException($"Parent node of '{path}' is not an object.");
        }

        obj[key] = JsonValue.Create(value);
    }

    /// <summary>
    /// Deletes a key from this object at the given path.
    /// </summary>
    /// <param name="path">The path to the element to remove.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Delete(string path)
    {
        var parent = NavigateOrCreate(GetParentPath(path, out var key));

        if (parent is not JsonObject obj) {
            throw new InvalidOperationException($"Parent node of '{path}' is not an object.");
        }

        obj.Remove(key);
    }

    /// <summary>
    /// Iterates over all keys in the object at the given path and invokes the
    /// specified action for each key that contains an object.
    /// </summary>
    /// <param name="action">The action to invoke.</param>
    public void ForEachObject(Action<ObjectPatcher, string> action)
    {
        foreach (var key in Keys) {
            if (IsObject(key)) {
                action(GetObject(key), key);
            }
        }
    }

    /// <summary>
    /// Returns a dereferenced <see cref="ObjectPatcher"/> for the JSON object
    /// stored at the given path. Changes made to the returned object must be
    /// saved back using <see cref="SetObjectAsJson"/>.
    /// </summary>
    /// <param name="path">The path to the object in slash-notation.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public ObjectPatcher GetObjectFromJson(string path)
    {
        if (! IsString(path)) {
            throw new InvalidOperationException($"Value at path '{path}' is not a string.");
        }
        
        string? json = GetValue<string>(path);
        if (string.IsNullOrEmpty(json)) {
            return new ObjectPatcher("{}");
        }
        
        if (!json.StartsWith('{')) {
            throw new InvalidOperationException($"Value at path '{path}' is null, empty or not a valid JSON object.");
        }
        
        return new ObjectPatcher(json);
    }
    
    /// <summary>
    /// Sets the object at the given path to the JSON representation of the
    /// given <see cref="ObjectPatcher"/> instance. Changes made to the given
    /// object are not automatically saved; you must call this method to persist them.
    /// </summary>
    /// <param name="path">The path to the object in slash-notation.</param>
    /// <param name="obj">The patcher to save.</param>
    public void SetObjectAsJson(string path, ObjectPatcher obj)
    {
        Set(path, obj.ToJsonString());
    }

    /// <summary>
    /// Returns a dereferenced <see cref="ArrayPatcher"/> for the JSON array
    /// stored at the given path. Changes made to the returned object must be
    /// saved back using <see cref="SetArrayAsJson"/>.
    /// </summary>
    /// <param name="path">The path to the array in slash-notation.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public ArrayPatcher GetArrayFromJson(string path)
    {
        if (!IsString(path)) {
            throw new InvalidOperationException($"Value at path '{path}' is not a string.");
        }
        
        string? json = GetValue<string>(path);
        if (string.IsNullOrEmpty(json)) {
            return new ArrayPatcher("[]");
        }
        
        if (!json.StartsWith('[')) {
            throw new InvalidOperationException($"Value at path '{path}' is null, empty or not a valid JSON array.");
        }
        
        return new ArrayPatcher(json);
    }
    
    /// <summary>
    /// Sts the array at the given path to the JSON representation of the
    /// given <see cref="ArrayPatcher"/> instance. Changes made to the given
    /// object are not automatically saved; you must call this method to persist them.
    /// </summary>
    /// <param name="path">The path to the array in slash-notation.</param>
    /// <param name="arr">The patcher to save.</param>
    public void SetArrayAsJson(string path, ArrayPatcher arr)
    {
        Set(path, arr.ToJsonString());
    }

    /// <summary>
    /// Returns a dereferenced <see cref="ObjectPatcher"/> for the Deflate64-compressed
    /// JSON object stored at the given path. Changes made to the returned object must
    /// be saved back using <see cref="SetObjectDeflate64"/>.
    /// </summary>
    /// <param name="path">The path to the object in slash-notation.</param>
    /// <param name="prefix">An optional prefix that is used in the encoded data.</param>
    /// <returns>A new <see cref="ObjectPatcher"/> instance.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public ObjectPatcher GetObjectDeflate64(string path, string? prefix = null)
    {
        string? data = GetValue<string>(path);

        if (string.IsNullOrEmpty(data)) {
            throw new InvalidOperationException($"Data at path '{path}' is null or empty.");
        }

        string? json = Inflate64(data, prefix);

        if (string.IsNullOrEmpty(json) || !json.StartsWith('{')) {
            throw new InvalidOperationException($"Data at path '{path}' could not be inflated to a valid JSON object.");
        }

        return new ObjectPatcher(json);
    }

    /// <summary>
    /// Sets the object at the given path to the Deflate64-compressed
    /// representation of the given <see cref="ObjectPatcher"/> instance. Changes
    /// made to the given object are not automatically saved; you must call this
    /// method to persist them.
    /// </summary>
    /// <param name="path">The path to the object in slash-notation.</param>
    /// <param name="obj">The patcher to save.</param>
    /// <param name="prefix">An optional prefix to prepend to the encoded data.</param>
    public void SetObjectDeflate64(string path, ObjectPatcher obj, string? prefix = null)
    {
        string json = obj.ToJsonString();
        string data = Deflate64(json, prefix);

        Set(path, data);
    }
}
