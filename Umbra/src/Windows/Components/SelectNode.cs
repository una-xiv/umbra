using System;
using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;
using System.Collections.Immutable;
using System.Linq;
using Umbra.Common;
using Umbra.Common.Extensions;
using Una.Drawing;

namespace Umbra.Windows.Components;

public class SelectNode : ImGuiInputNode
{
    public event Action<string>? OnValueChanged;

    public string Value {
        get => _value;
        set
        {
            if (_value == value) return;
            _value = value;
            OnValueChanged?.Invoke(value);
        }
    }

    public List<string> Choices {
        get => _choices;
        set
        {
            _isLocked = true;
            _choices.Clear();
            _choices.AddRange(value);
            _isLocked = false;
        }
    }

    public void SetValueInternal(string c) => _value = c;

    private readonly List<string> _choices;
    private          string       _value;
    private          bool         _isLocked;

    public SelectNode(
        string id, string value, List<string> choices, string? label = null, string? description = null,
        int    leftMargin = 36
    )
    {
        Id          = id;
        LeftMargin  = leftMargin;
        Label       = label;
        Description = description;
        _value      = value;
        _choices    = [..choices];
    }

    public SelectNode()
    {
        _value   = "None";
        _choices = ["None"];
    }

    protected override void DrawImGuiInput(Rect bounds)
    {
        ImGui.SetNextItemWidth(bounds.Width);

        // Ensure the current value is amongst the choices.
        if (!_isLocked && !_choices.Contains(Value)) {
            Logger.Warning(
                $"Selected value [{Value}] is not amongst children in {Id}. Possible values: {string.Join(", ", _choices)}");
            Value = _choices.Count > 0 ? _choices[0] : "";
        }

        if (ImGui.BeginCombo($"##{InternalId.Slugify()}", Value)) {
            foreach (string choice in _choices.ToImmutableArray()) {
                bool isSelected = Value == choice;

                if (ImGui.Selectable(choice, isSelected)) {
                    Value = choice;
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
        foreach (var handler in OnValueChanged?.GetInvocationList() ?? []) OnValueChanged -= (Action<string>)handler;

        OnValueChanged = null;
        _choices.Clear();

        base.OnDisposed();
    }
}
