
using Umbra.Common.Extensions;

namespace Umbra.Windows.Components;

public class ColorInputPopupNode : Node
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

    private uint  _value;
    private uint? _originalValue;
    private uint? _copiedValue;

    private ButtonNode RevertButton => QuerySelector<ButtonNode>("#revert")!;
    private ButtonNode CopyButton   => QuerySelector<ButtonNode>("#copy")!;

    public ColorInputPopupNode(uint value)
    {
        _value = value;
        ClassList.Add("color-input-popup");

        UdtDocument doc = UdtLoader.Parse(GetType().Name, UdtSource);

        Stylesheet = doc.Stylesheet;
        AppendChild(doc.RootNode!);

        RevertButton.IsDisabled = true;

        RevertButton.OnMouseUp += _ => {
            Value                   = _originalValue ?? _value;
            RevertButton.IsDisabled = true;
        };

        CopyButton.OnMouseUp += _ => {
            ImGui.SetClipboardText($"#{_value:X8}");
            _copiedValue = _value;
        };
    }

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        string  id    = $"##{InternalId.Slugify()}";
        Vector4 value = ImGui.ColorConvertU32ToFloat4(_value);

        ImGuiColorEditFlags flags = ImGuiColorEditFlags.NoLabel
                                    | ImGuiColorEditFlags.AlphaBar
                                    | ImGuiColorEditFlags.DisplayMask
                                    | ImGuiColorEditFlags.AlphaPreview
                                    | ImGuiColorEditFlags.NoSidePreview
                                    | ImGuiColorEditFlags.NoSmallPreview
                                    | ImGuiColorEditFlags.NoTooltip
                                    | ImGuiColorEditFlags.InputRgb;

        flags &= ~ImGuiColorEditFlags.DisplayHsv;
        flags &= ~ImGuiColorEditFlags.DisplayHex;

        ImGui.SetNextItemWidth(250);
        ImGui.SetCursorPos(new(8, 8));
        if (ImGui.ColorPicker4(id, ref value, flags)) {
            Value = ImGui.ColorConvertFloat4ToU32(value);
        }

        ImGui.SetNextItemWidth(250);
        ImGui.SetCursorPosX(8);
        ImGui.PushStyleColor(ImGuiCol.Border, Color.GetNamedColor("Window.Border"));
        ImGui.PushStyleColor(ImGuiCol.PopupBg, Color.GetNamedColor("Window.Background"));
        if (ImGui.BeginCombo("##ColorPresetPicker", "Pick from...")) {
            foreach (var name in Color.GetAssignedNames()) {
                ImGui.PushStyleColor(ImGuiCol.Text, Color.GetNamedColor(name));
                if (ImGui.Selectable(I18N.Translate($"Color.{name}.Name"))) {
                    Value = Color.GetNamedColor(name);
                }

                ImGui.PopStyleColor();
            }

            ImGui.EndCombo();
        }

        ImGui.PopStyleColor(2);

        if (null == _originalValue && _originalValue != _value) {
            _originalValue          = _value;
            RevertButton.IsDisabled = false;
        } else {
            RevertButton.IsDisabled = _originalValue == _value;
        }

        CopyButton.IsDisabled = _copiedValue == _value;
    }

    protected override void OnDisposed()
    {
        foreach (var handler in OnValueChanged?.GetInvocationList() ?? [])
            OnValueChanged -= (Action<uint>)handler;

        OnValueChanged = null;

        base.OnDisposed();
    }
    
    private const string UdtSource =
        """
        <udt>
            <node id="picker">
                <node class="dummy"/>
                <node class="info">
                    <node class="buttons">
                        <button-node id="copy" label="_L(CopyToClipboard)"/>
                        <button-node id="revert" label="_L(Undo)"/>
                    </node>
                </node>
            </node>
        
            <![CDATA[
                @import "globals";
                
                .color-input-popup {
                    size: 266 0;
                    border-radius: 8;
                    background-color: "Window.Background";
                    stroke-color: "Window.Border";
                    stroke-width: 1;
                    stroke-inset: 1;
                }
                
                #picker {
                    auto-size: grow;
                    flow: vertical;
                    gap: 10;
                    
                    & > .dummy {
                        size: 250;    
                    }
                    
                    & > .info {
                        auto-size: grow fit;
                        padding: 8;
                        
                        & > .buttons {
                            anchor: middle-right;
                            gap: 8;
                        }
                    }
                }
            ]]>
        </udt>
        """;
}
