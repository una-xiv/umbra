using Dalamud.Game.Text;
using ImGuiNET;
using System;
using Umbra.Common;
using Umbra.Windows.GameGlyphPicker;

namespace Umbra.Windows.Components;

public class GameGlyphInputNode : BoxedInputNode
{
    public event Action<SeIconChar>? OnValueChanged;

    public SeIconChar Value {
        get => _value;
        set
        {
            if (_value == value) return;
            _value = value;
            OnValueChanged?.Invoke(value);
            BoxNode.NodeValue = Value.ToIconString();
        }
    }

    private SeIconChar _value;

    public GameGlyphInputNode(string id, SeIconChar value, string label, string? description = null)
    {
        _value = value;

        Id          = id;
        Label       = label;
        Description = description;
        
        CreateNode();
    }

    public GameGlyphInputNode()
    {
        _value = SeIconChar.Cross;
        CreateNode();
    }

    protected override void OnDraw(ImDrawListPtr _)
    {
        BoxNode.NodeValue = Value.ToIconString();
    }

    protected override void OnDisposed()
    {
        foreach (var handler in OnValueChanged?.GetInvocationList() ?? [])
            OnValueChanged -= (Action<SeIconChar>)handler;

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
                    new GameGlyphPickerWindow(Value),
                    window => {
                        if (null != window.Icon) Value = window.Icon.Value; 
                    }
                );
        };
    }
}
