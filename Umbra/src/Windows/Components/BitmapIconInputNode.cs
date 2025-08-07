using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;

using Umbra.Windows.FaIconPicker;
using Umbra.Windows.GameGlyphPicker;

namespace Umbra.Windows.Components;

public class BitmapIconInputNode : BoxedInputNode
{
    public event Action<BitmapFontIcon>? OnValueChanged;

    public BitmapFontIcon Value {
        get => _value;
        set
        {
            if (_value == value) return;
            _value = value;
            OnValueChanged?.Invoke(value);
            BoxNode.Style.BitmapFontIcon = Value;
        }
    }

    private BitmapFontIcon _value;

    public BitmapIconInputNode(string id, BitmapFontIcon value, string label, string? description = null)
    {
        _value = value;

        Id          = id;
        Label       = label;
        Description = description;
        
        CreateNode();
    }

    public BitmapIconInputNode()
    {
        _value = BitmapFontIcon.None;
        CreateNode();
    }

    protected override void OnDraw(ImDrawListPtr _)
    {
        BoxNode.Style.BitmapFontIcon = Value;
    }

    protected override void OnDisposed()
    {
        foreach (var handler in OnValueChanged?.GetInvocationList() ?? [])
            OnValueChanged -= (Action<BitmapFontIcon>)handler;

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
                    new BitmapIconPickerWindow(Value),
                    window => {
                        if (null != window.Icon) Value = window.Icon.Value; 
                    }
                );
        };
    }
}
