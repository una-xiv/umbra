
using Una.Drawing.Templating.StyleParser;

namespace Umbra.Windows.Components;

public abstract class BoxedInputNode : Node
{
    public string? Label {
        get => (string)(LabelNode.NodeValue ?? "");
        set => LabelNode.NodeValue = value;
    }

    public string? Description {
        get => _description;
        set
        {
            _description = value;
            if (UmbraBindings.ShowInputControlDescriptions) {
                LabelNode.Tooltip         = null;
                DescriptionNode.NodeValue = _description;
            } else {
                LabelNode.Tooltip         = _description;
                DescriptionNode.NodeValue = null;
            }
        }
    }

    public bool IsFit {
        get => ClassList.Contains("fit");
        set => ToggleClass("fit", value);
    }

    public bool IsLabelLeft {
        get => ClassList.Contains("label-left");
        set => ToggleClass("label-left", value);
    }

    public BoxedInputNode()
    {
        ClassList  = ["boxed-input"];
        Stylesheet = BoxedInputStylesheet;

        ChildNodes = [
            new() {
                ClassList   = ["box", "ui-frame-input"],
                InheritTags = true,
            },
            new() {
                Id          = "Text",
                ClassList   = ["text"],
                InheritTags = true,
                ChildNodes = [
                    new() {
                        Id          = "Label",
                        ClassList   = ["label", "ui-text-default"],
                        InheritTags = true,
                    },
                    new() {
                        Id        = "Description",
                        ClassList = ["description", "ui-text-muted"],
                    },
                ],
            },
        ];
    }

    protected Node TextNode        => QuerySelector(".text")!;
    protected Node BoxNode         => QuerySelector(".box")!;
    protected Node LabelNode       => QuerySelector(".text > .label")!;
    protected Node DescriptionNode => QuerySelector(".text > .description")!;

    private string? _description;
    private bool    _showDescription = UmbraBindings.ShowInputControlDescriptions;
    
    protected override void OnDraw(ImDrawListPtr drawList)
    {
        if (_showDescription != UmbraBindings.ShowInputControlDescriptions) {
            _showDescription = UmbraBindings.ShowInputControlDescriptions;
            switch (_showDescription) {
                case true:
                    LabelNode.Tooltip         = null;
                    DescriptionNode.NodeValue = _description;
                    break;
                case false:
                    LabelNode.Tooltip         = _description;
                    DescriptionNode.NodeValue = null;
                    break;
            }
        }
        
        LabelNode.Style.IsVisible       = LabelNode.NodeValue is not null;
        DescriptionNode.Style.IsVisible = _showDescription && !string.IsNullOrEmpty((string?)DescriptionNode.NodeValue);
        TextNode.ToggleClass("has-description", DescriptionNode.Style.IsVisible);
        
        base.OnDraw(drawList);
    }

    private static readonly Stylesheet BoxedInputStylesheet = StyleParser.StylesheetFromCode(
        """
        @import "globals";

        .boxed-input {
            flow: horizontal;
            auto-size: grow fit;
            gap: 8;
            
            &.label-left { flow-order: reverse; }
            &.fit { auto-size: fit; }
            
            & > .box {
                flow: horizontal;
                anchor: top-left;
                size: 28;
                font: 2;
                font-size: 14;
                text-align: middle-center;
                text-offset: 0 1;
                
                & > .preview {
                    auto-size: grow;
                    margin: 4;
                    border-radius: 4;
                }
            }
            
            & > .text {
                flow: vertical;
                anchor: top-left;
                auto-size: grow fit;
                gap: 1;
                padding: 4 0 0 0;
                
                &.has-description { 
                    padding: 0 0 0 0;
                }
                
                & > .label {
                    anchor: top-left;
                    auto-size: grow fit;
                    text-align: middle-left;
                    text-overflow: false;
                    word-wrap: false;
                }
                
                & > .description {
                    anchor: top-left; 
                    auto-size: grow fit; 
                    text-overflow: false;
                    word-wrap: true;
                    line-height: 1.1;
                }
            }
        }
        """
    );
}
