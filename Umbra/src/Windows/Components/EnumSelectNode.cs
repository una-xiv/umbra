using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Reflection;
using Umbra.Common;
using Umbra.Common.Extensions;
using Una.Drawing;

namespace Umbra.Windows.Components;

public class EnumSelectNode<T> : ImGuiInputNode, IEnumSelectNode where T : struct, Enum
{
    public Action<Enum>? OnValueChanged;

    public T Value {
        get => _value;
        set
        {
            if (_value.Equals(value)) return;
            _value       = value;
            _selectedKey = Enum.GetName(value) ?? string.Empty;
            OnValueChanged?.Invoke(value);
        }
    }

    private readonly Dictionary<string, string> _choices = [];

    private T      _value;
    private string _selectedKey = String.Empty;

    public EnumSelectNode()
    {
        // Grab all TranslationKey attributes from the enum.
        Type enumType = typeof(T);
        foreach (var key in enumType.GetEnumNames()) {
            // Get the field info for the enum value.
            FieldInfo? field = enumType.GetField(key);
            if (field == null) continue;
            
            var attribute = field.GetCustomAttribute<TranslationKeyAttribute>();
            if (attribute != null) {
                _choices[field.Name] = I18N.Has(attribute.Key) ? I18N.Translate(attribute.Key) : attribute.Key;
                continue;
            }

            string translationKey = attribute != null ? attribute.Key : $"Enum.{enumType.Name}.{field.Name}";
            _choices[field.Name] = I18N.Has(translationKey) ? I18N.Translate(translationKey) : field.Name;
        }
    }

    protected override void DrawImGuiInput(Rect bounds)
    {
        string selectedLabel = _selectedKey;
        if (_choices.TryGetValue(Enum.GetName(Value) ?? "", out string? name)) {
            selectedLabel = name;
        }

        if (ImGui.BeginCombo($"##{InternalId.Slugify()}", selectedLabel)) {
            foreach (var (key, label) in _choices) {
                bool isSelected = _selectedKey == key;

                if (ImGui.Selectable(label, isSelected)) {
                    Value = Enum.Parse<T>(key, true);
                }

                if (isSelected) {
                    ImGui.SetItemDefaultFocus();
                }
            }

            ImGui.EndCombo();
        }
    }

    protected override void OnDisposed()
    {
        foreach (var handler in OnValueChanged?.GetInvocationList() ?? [])
            OnValueChanged -= (Action<Enum>)handler;

        OnValueChanged = null;

        base.OnDisposed();
    }
}

public interface IEnumSelectNode
{
    public string? Id          { get; set; }
    public string? Label       { get; set; }
    public string? Description { get; set; }
    public float?  LeftMargin  { get; set; }
}
