using System;
using Umbra.Common;
using Umbra.Windows.GameIconPicker;

namespace Umbra.Windows.Components;

public class GameIconInputNode : BoxedInputNode
{
    public event Action<uint>? OnValueChanged;

    public uint Value {
        get => _value;
        set
        {
            if (_value == value) return;
            _value = value;
            OnValueChanged?.Invoke(value);
            if (IsDisposed) return;
            
            var boxNode = BoxNode.QuerySelector(".preview");
            if (boxNode != null) { // May be NULL during plugin unload.
                boxNode.Style.IconId = value;
            }
        }
    }

    private uint _value;

    public GameIconInputNode(string id, uint value, string label, string? description = null)
    {
        CreateNode();

        Id          = id;
        Label       = label;
        Description = description;
        Value       = value;
    }

    public GameIconInputNode()
    {
        CreateNode();
    }

    private void CreateNode()
    {
        BoxNode.AppendChild(new() { ClassList = ["preview"] });

        OnClick += _ => {
            Framework
               .Service<WindowManager>()
               .Present(
                    "IconPicker",
                    new GameIconPickerWindow(Value),
                    window => {
                        if (null != window.SelectedId) Value = window.SelectedId.Value;
                    }
                );
        };
    }

    protected override void OnDisposed()
    {
        foreach (var handler in OnValueChanged?.GetInvocationList() ?? []) OnValueChanged -= (Action<uint>)handler;

        OnValueChanged = null;

        base.OnDisposed();
    }
}
