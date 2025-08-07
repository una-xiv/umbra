
using Una.Drawing.Templating.StyleParser;

namespace Umbra.Windows.Components;

public abstract class ImGuiInputNode : Node
{
    public string? Label {
        get => (string?)LabelNode.NodeValue;
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

    public float? LeftMargin {
        get => Style.Margin?.Left;
        set => Style.Margin = new(0, 0, 0, value ?? 0);
    }

    private string? _description;
    private bool    _showDescription = UmbraBindings.ShowInputControlDescriptions;
    private bool    _isFirstDraw     = true;

    public ImGuiInputNode()
    {
        ClassList  = ["input"];
        Stylesheet = InputStylesheet;

        ChildNodes = [
            new() { ClassList = ["label", "ui-text-default"] },
            new() { ClassList = ["description", "ui-text-muted"] },
            new() {
                ClassList = ["box-row"],
                ChildNodes = [
                    new() { ClassList = ["box"] },
                    new() {
                        ClassList  = ["buttons"],
                        ChildNodes = []
                    },
                ],
            },
        ];
    }

    protected virtual List<Node> GetButtonNodes()
    {
        return [];
    }
    
    protected abstract void DrawImGuiInput(Rect bounds);

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        if (_isFirstDraw) {
            _isFirstDraw = false;
            foreach (var node in GetButtonNodes()) {
                QuerySelector(".buttons")!.AppendChild(node);
            }
        }
        
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

        bool hasLabel       = LabelNode.NodeValue is not null;
        bool hasDescription = _showDescription && DescriptionNode.NodeValue is not null;

        LabelNode.Style.IsVisible       = hasLabel;
        DescriptionNode.Style.IsVisible = hasDescription;

        var bounds = BoxNode.Bounds.ContentRect;
        ImGui.SetCursorScreenPos(bounds.TopLeft);

        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(8, 4));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(8, 4));
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 5);
        ImGui.PushStyleVar(ImGuiStyleVar.PopupRounding, 5);
        ImGui.PushStyleVar(ImGuiStyleVar.PopupBorderSize, 1);

        ImGui.PushStyleColor(ImGuiCol.Text, new Color("Input.Text").ToUInt());
        ImGui.PushStyleColor(ImGuiCol.PopupBg, new Color("Input.Background").ToUInt());
        ImGui.PushStyleColor(ImGuiCol.Border, new Color("Input.Border").ToUInt());
        ImGui.PushStyleColor(ImGuiCol.Button, new Color("Input.Background").ToUInt());
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Color("Input.Background").ToUInt());
        ImGui.PushStyleColor(ImGuiCol.FrameBg, new Color("Input.Background").ToUInt());
        ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, new Color("Input.Background").ToUInt());
        ImGui.PushStyleColor(ImGuiCol.FrameBgActive, new Color("Input.Background").ToUInt());

        try {
            DrawImGuiInput(bounds);
        } catch (Exception e) {
            Logger.Error($"Error while drawing ImGui input node: {e.Message}");
        } finally {
            ImGui.PopStyleColor(8);
            ImGui.PopStyleVar(5);
        }

        base.OnDraw(drawList);
    }

    protected Node BoxNode         => QuerySelector(".box")!;
    protected Node LabelNode       => QuerySelector(".label")!;
    protected Node DescriptionNode => QuerySelector(".description")!;

    private static readonly Stylesheet InputStylesheet = StyleParser.StylesheetFromCode(
        """
        @import "globals";

        .input {
            flow: vertical;
            auto-size: grow fit;
            gap: 1;
            
            & > .box-row {
                auto-size: grow fit;
                gap: 8;
                
                & > .box {
                    auto-size: grow fit;
                    size: 0 24;
                }
                
                & > .buttons {
                    size: 0 24;
                }
            }
            
            & > .label {
                auto-size: grow fit;
                text-overflow: false;
                word-wrap: false;
            }
            
            & > .description {
                auto-size: grow fit;
                text-overflow: false;
                word-wrap: true;
                line-height: 1.2;
            }
        }
        """
    );
}
