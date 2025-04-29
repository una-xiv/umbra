using ImGuiNET;
using System;
using System.Numerics;
using Umbra.Common.Extensions;
using Una.Drawing;

namespace Umbra.Windows.Components;

public class SimpleColorPickerNode : Node
{
    public event Action<uint>? OnValueChanged;

    public uint Value {
        get => _value;
        set
        {
            if (_value == value) return;
            _value = value;
            Style.BackgroundColor = new(_value);
        }
    }

    private uint                 _value;
    private ColorInputPopupNode? _popupNode;

    public SimpleColorPickerNode()
    {
        Style.AutoSize =  (AutoSize.Grow, AutoSize.Grow);
        OnMouseDown    += _ => { ImGui.OpenPopup($"##{InternalId.Slugify()}"); };
    }

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        var id = $"##{InternalId.Slugify()}";

        ImGui.PushStyleColor(ImGuiCol.ChildBg, 0x00000000);
        ImGui.PushStyleColor(ImGuiCol.PopupBg, 0x00000000);

        if (ImGui.BeginPopup(id)) {
            if (null == _popupNode) {
                _popupNode = new ColorInputPopupNode(_value);
                _popupNode.ComputeBoundingSize();
                _popupNode.OnValueChanged += u => {
                    Value = u;
                    OnValueChanged?.Invoke(Value);
                };
            }

            ImGui.PushStyleVar(ImGuiStyleVar.PopupBorderSize, 1);
            ImGui.PushStyleVar(ImGuiStyleVar.PopupRounding, 8);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(4, 4));
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(4, 4));

            ImGui.BeginChild(id, _popupNode.Bounds.MarginSize.ToVector2(), true);
            _popupNode.Render(ImGui.GetWindowDrawList(), new(0, 0));
            ImGui.EndChild();

            ImGui.PopStyleVar(4);
            ImGui.EndPopup();
        } else {
            if (null != _popupNode) {
                _popupNode.Dispose();
                _popupNode = null;
            }
        }

        ImGui.PopStyleColor(2);
    }

    protected override void OnDisposed()
    {
        foreach (var handler in OnValueChanged?.GetInvocationList() ?? []) OnValueChanged -= (Action<uint>)handler;

        OnValueChanged = null;

        base.OnDisposed();
    }
}
