using Umbra.Widgets;

namespace Umbra.AuxBar;

public class AuxBarNode : UdtNode
{
    private bool _isVertical;
    private int _width;
    
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
        
        _width = config.Width;
    }

    protected override void OnDraw(ImDrawListPtr _)
    {
        Style.Size = _isVertical 
            ? new (0, 0) 
            : new(_width, Toolbar.Height);
        
        foreach (var widget in QuerySelectorAll(".widget-instance")) {
            if (_isVertical) {
                widget.Style.AutoSize = (AutoSize.Grow, AutoSize.Fit);
            } else if (widget.GetAttachment<ToolbarWidget>("Widget") is not StandardToolbarWidget) {
                widget.Style.AutoSize = null;
            }
        }
    }

    public int WidgetCount => QuerySelector(".section")!.ChildNodes.Count;
}
