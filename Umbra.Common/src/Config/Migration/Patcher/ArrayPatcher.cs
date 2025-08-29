using System.Text.Json.Nodes;

namespace Umbra.Common.Migration.Patcher;

public class ArrayPatcher : BasePatcher<JsonArray>
{
    public ArrayPatcher(string jsonString) : base(jsonString)
    {
    }

    public ArrayPatcher(JsonArray node) : base(node)
    {
    }
    
    public int Length => Node.Count;
    public bool IsEmpty => Node.Count == 0;
    public bool IsNotEmpty => Node.Count > 0;
    
    public bool IsString(int index) => GetElementOrNull(index) is JsonValue value && value.TryGetValue<string>(out _);
    public bool IsNumber(int index) => GetElementOrNull(index) is JsonValue value && value.TryGetValue<double>(out _);
    public bool IsBoolean(int index) => GetElementOrNull(index) is JsonValue value && value.TryGetValue<bool>(out _);
    public bool IsObject(int index) => GetElementOrNull(index) is JsonObject;
    public bool IsArray(int index) => GetElementOrNull(index) is JsonArray;
    
    /// <summary>
    /// Iterates over each object in the array, executing the provided action.
    /// </summary>
    public void ForEachObject(Action<ObjectPatcher, int> action)
    {
        for (var i = 0; i < Node.Count; i++) {
            if (Node[i] is JsonObject obj) {
                action(new ObjectPatcher(obj), i);
            }
        }
    }
    
    /// <summary>
    /// Iterates over each array in this array, executing the provided action.
    /// </summary>
    public void ForEachArray(Action<ArrayPatcher, int> action)
    {
        for (var i = 0; i < Node.Count; i++) {
            if (Node[i] is JsonArray obj) {
                action(new ArrayPatcher(obj), i);
            }
        }
    }

    /// <summary>
    /// Returns a value of the given type at the specified index, or the
    /// provided default value if the index does not exist or the value cannot
    /// be converted.
    /// </summary>
    /// <param name="index">The index in this array.</param>
    /// <param name="defaultValue">The value to return if the requested value does not exist.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetValue<T>(int index, T? defaultValue = default)
    {
        var node = GetElementOrNull(index);

        if (node is JsonValue value && value.TryGetValue<T>(out var result)) {
            return result;
        }

        return defaultValue!;
    }
    
    /// <summary>
    /// Returns an <see cref="ObjectPatcher"/> instance for the object at the
    /// specified index.
    /// </summary>
    /// <param name="index">The index in this array.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public ObjectPatcher GetObject(int index)
    {
        var node = GetElementOrNull(index);

        if (node is not JsonObject obj) {
            throw new InvalidOperationException($"Element at index '{index}' does not exist or is not an object.");
        }

        return new ObjectPatcher(obj);
    }

    /// <summary>
    /// Returns an <see cref="ArrayPatcher"/> instance for the array at the
    /// specified index.
    /// </summary>
    /// <returns>A referenced <see cref="ArrayPatcher"/>.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public ArrayPatcher GetArray(int index)
    {
        var node = GetElementOrNull(index);

        if (node is not JsonArray arr) {
            throw new InvalidOperationException($"Element at index '{index}' does not exist or is not an array.");
        }

        return new ArrayPatcher(arr);
    }
    
    /// <summary>
    /// Adds the given value to the end of the array.
    /// </summary>
    /// <param name="value">The value to append to the array.</param>
    public void Append(object? value) => Node.Add(JsonValue.Create(value));
    
    /// <summary>
    /// Prepend the given value to the start of the array.
    /// </summary>
    /// <param name="value">The value to prepend to the array.</param>
    public void Prepend(object? value) => Node.Insert(0, JsonValue.Create(value));
    
    /// <summary>
    /// Removes the element at the specified index.
    /// </summary>
    /// <param name="index">The index of the element.</param>
    public void DeleteAt(int index) => Node.RemoveAt(index);
    
    /// <summary>
    /// Clears all elements from the array.
    /// </summary>
    public void Clear() => Node.Clear();

    /// <summary>
    /// Replaces the value at the specified index with the given value.
    /// </summary>
    /// <param name="index">The index of the element to replace.</param>
    /// <param name="value">The replacement value.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void ReplaceAt(int index, object? value)
    {
        if (index < 0 || index >= Node.Count) {
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        }
        
        Node[index] = JsonValue.Create(value);
    }
    
    private JsonNode? GetElementOrNull(int index) => Node[index];
}
