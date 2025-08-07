
using Umbra.Common.Extensions;

namespace Umbra.Windows.Components;

public class ColorInputNode : BoxedInputNode
{
    public event Action<uint>? OnValueChanged;

    public uint Value {
        get => _value;
        set
        {
            if (_value == value) return;
            _value = value;
            OnValueChanged?.Invoke(value);
        }
    }

    private uint _value;

    public ColorInputNode(string id, uint value, string label, string? description = null)
    {
        CreateNode();

        _value      = value;
        Id          = id;
        Label       = label;
        Description = description;
    }

    public ColorInputNode()
    {
        CreateNode();
    }

    protected override void OnDisposed()
    {
        foreach (var handler in OnValueChanged?.GetInvocationList() ?? []) OnValueChanged -= (Action<uint>)handler;

        OnValueChanged = null;

        base.OnDisposed();
    }

    private ColorInputPopupNode? _popupNode;

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        base.OnDraw(drawList);
        
        PreviewNode.Style.BackgroundColor = new(Value);

        var id = $"##{InternalId.Slugify()}";

        ImGui.PushStyleColor(ImGuiCol.ChildBg, 0x00000000);
        ImGui.PushStyleColor(ImGuiCol.PopupBg, 0x00000000);
        
        if (ImGui.BeginPopup(id)) {
            if (null == _popupNode) {
                _popupNode = new ColorInputPopupNode(_value);
                _popupNode.ComputeBoundingSize();
                _popupNode.OnValueChanged += u => Value = u;
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

    private Node PreviewNode => QuerySelector(".preview")!;
    
    private void CreateNode()
    {
        BoxNode.AppendChild(new() { ClassList = ["preview"] });
        OnMouseDown += _ => { ImGui.OpenPopup($"##{InternalId.Slugify()}"); };
    }
}
