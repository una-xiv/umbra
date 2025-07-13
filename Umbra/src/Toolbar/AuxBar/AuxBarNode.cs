using ImGuiNET;
using Umbra.Common;
using Una.Drawing;

namespace Umbra.AuxBar;

public class AuxBarNode : UdtNode
{
    private bool _isVertical;
    
    public AuxBarNode(AuxBarConfig config) : base("umbra.auxbar.xml")
    {
        QuerySelector(".section")!.Id = config.Id;
        Update(config);
    }
    
    public void Update(AuxBarConfig config)
    {
        ToggleClass("toolbar", config.Decorate);
        ToggleClass("shadow", config.EnableShadow);
        ToggleClass("rounded", config.RoundedCorners);
        ToggleClass("vertical", _isVertical = config.IsVertical);

        QuerySelector(".section")!.Style.Gap = config.ItemSpacing;
    }

    protected override void OnDraw(ImDrawListPtr _)
    {
        Style.Size = _isVertical 
            ? new (0, 0) 
            : new(0, Toolbar.Height);
        
        foreach (var widget in QuerySelectorAll(".widget-instance")) {
            widget.Style.AutoSize = _isVertical ? (AutoSize.Grow, AutoSize.Fit) : null;
        }
    }

    public int WidgetCount => QuerySelector(".section")!.ChildNodes.Count;
}
