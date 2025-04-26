using Dalamud.Interface;
using ImGuiNET;
using Una.Drawing;
using Una.Drawing.Templating.StyleParser;

namespace Umbra.Windows.Components;

public class ButtonNode : Node
{
    public string? Label {
        get => _label;
        set
        {
            _label                    = value?.Trim();
            LabelNode.NodeValue       = _label;
            ToggleClass("has-label", null != value);
        }
    }

    public FontAwesomeIcon? Icon {
        get => _icon;
        set
        {
            _icon                    = value;
            IconNode.NodeValue       = _icon?.ToIconString();
            ToggleClass("has-icon", null != value);
        }
    }

    public bool IsGhost {
        get => ClassList.Contains("ghost");
        set => ToggleClass("ghost", value);
    }

    public bool IsSmall {
        get => ClassList.Contains("small");
        set => ToggleClass("small", value);
    }

    private string?          _label;
    private FontAwesomeIcon? _icon;

    private Node IconNode  => QuerySelector("#Icon")!;
    private Node LabelNode => QuerySelector("#Label")!;

    public ButtonNode(
        string id, string? label, FontAwesomeIcon? icon = null, bool isGhost = false, bool isSmall = false
    )
    {
        CreateNode();

        Id      = id;
        IsGhost = isGhost;
        Label   = label;
        Icon    = icon;
        IsSmall = isSmall;
    }

    public ButtonNode()
    {
        CreateNode();

        Label = null;
        Icon  = null;
    }

    private void CreateNode()
    {
        IsGhost    = false;
        ClassList  = ["button", "ui-frame-input"];
        Stylesheet = ButtonStylesheet;
        ChildNodes = [
            new() { Id = "Icon", ClassList  = ["icon", "ui-text-default"], InheritTags  = true },
            new() { Id = "Label", ClassList = ["label", "ui-text-default"], InheritTags = true },
        ];
    }

    protected override void OnDraw(ImDrawListPtr _)
    {
    }

    private static readonly Stylesheet ButtonStylesheet = StyleParser.StylesheetFromCode(
        """
        @import "globals";

        .button {
            size: 0 28;
            padding: 0 8;
            gap: 6;
            
            &:disabled {
                color: "Input.TextDisabled";
                outline-color: "Input.TextOutlineDisabled";
                background-color: "Input.BackgroundDisabled";
                stroke-color: "Input.BorderDisabled";
            }
            
            &.ghost {
                background-color: 0;
                stroke-color: 0;
            }
            
            & > .icon {
                anchor: middle-left;
                auto-size: fit grow;
                size: 16 0;
                text-align: middle-center;
                font-size: 13;
                text-offset: 0 1;
                font: 2; // FontAwesome
                is-visible: false;
            }
            
            & > .label {
                anchor: middle-left;
                text-align: middle-center;
                size: 0 28;
                is-visible: false;
            }
        }
        
        .button.has-label > .label { is-visible: true; }
        .button.has-icon  > .icon  { is-visible: true; }
        
        .button.small {
            size: 0 20;
            padding: 0 5;
            
            &.has-label > .label { padding: 0 8 0 0; }
            
            & > .icon { size: 13 0; font-size: 11; }
            & > .label { size: 0 20; font-size: 10; }
        }
        """
    );
}
