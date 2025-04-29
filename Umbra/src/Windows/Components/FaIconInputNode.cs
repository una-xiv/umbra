using Dalamud.Interface;
using System;
using ImGuiNET;
using Umbra.Common;
using Umbra.Windows.FaIconPicker;
// using Umbra.Windows.Library.IconPicker;
using Una.Drawing;
using Una.Drawing.Templating.StyleParser;

namespace Umbra.Windows.Components;

public class FaIconInputNode : BoxedInputNode
{
    public event Action<FontAwesomeIcon>? OnValueChanged;

    public FontAwesomeIcon Value {
        get => _value;
        set
        {
            if (_value == value) return;
            _value = value;
            OnValueChanged?.Invoke(value);
            BoxNode.NodeValue = Value.ToIconString();
        }
    }

    private FontAwesomeIcon _value;

    public FaIconInputNode(string id, FontAwesomeIcon value, string label, string? description = null)
    {
        _value = value;

        Id          = id;
        Label       = label;
        Description = description;
        
        CreateNode();
    }

    public FaIconInputNode()
    {
        _value = FontAwesomeIcon.None;
        CreateNode();
    }

    protected override void OnDraw(ImDrawListPtr _)
    {
        BoxNode.NodeValue = Value.ToIconString();
    }

    protected override void OnDisposed()
    {
        foreach (var handler in OnValueChanged?.GetInvocationList() ?? [])
            OnValueChanged -= (Action<FontAwesomeIcon>)handler;

        OnValueChanged = null;

        base.OnDisposed();
    }

    private void CreateNode()
    {
        OnClick += _ => {
            Framework
               .Service<WindowManager>()
               .Present(
                    "IconPicker",
                    new FaIconPickerWindow(Value),
                    window => {
                        if (null != window.Icon) Value = window.Icon.Value; 
                    }
                );
        };
    }
}
