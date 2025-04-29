using ImGuiNET;
using Umbra.Common;
using Una.Drawing;

namespace Umbra.AuxBar;

public class AuxBarNode : UdtNode
{
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

        QuerySelector(".section")!.Style.Gap = config.ItemSpacing;
    }

    protected override void OnDraw(ImDrawListPtr _)
    {
    }

    public int WidgetCount => QuerySelector(".section")!.ChildNodes.Count;
}
