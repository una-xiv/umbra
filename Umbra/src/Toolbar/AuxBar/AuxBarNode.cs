using Umbra.Widgets;

namespace Umbra.AuxBar;

public class AuxBarNode : UdtNode
{
    private bool   _isVertical;
    private int    _width;
    private string _widgetContentAlignment;

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

        _width                  = config.Width;
        _widgetContentAlignment = config.WidgetContentAlignment;
    }

    protected override void OnDraw(ImDrawListPtr _)
    {
        Style.Size = _isVertical
            ? new(0, 0)
            : new(_width, Toolbar.Height);

        Anchor anchor = _widgetContentAlignment switch {
            "Left"  => Anchor.MiddleLeft,
            "Right" => Anchor.MiddleRight,
            _       => Anchor.MiddleCenter,
        };

        foreach (var widget in QuerySelectorAll(".widget-instance")) {
            if (widget.GetAttachment<ToolbarWidget>("Widget") is StandardToolbarWidget instance) {
                if (_isVertical) {
                    widget.Style.AutoSize = (AutoSize.Grow, AutoSize.Fit);
                }

                instance.SetWidgetAnchor(anchor);
            } else {
                widget.Style.AutoSize = null;
            }
        }
    }

    public int WidgetCount => QuerySelector(".section")!.ChildNodes.Count;
}
