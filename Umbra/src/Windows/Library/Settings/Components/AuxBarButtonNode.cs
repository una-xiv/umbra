
using Umbra.AuxBar;

namespace Umbra.Windows.Settings.Components;

public class AuxBarButtonNode : UdtNode
{
    public AuxBarConfig Config { get; }
    
    public AuxBarButtonNode(AuxBarConfig config) : base("umbra.windows.settings.components.aux_button.xml")
    {
        Id        = $"btn-{config.Id}";
        ClassList = ["bar-button"];
        Config    = config;
        
        QuerySelector("#delete-button")!.OnClick += _ => {
            if (config.Id == "aux") return;
            
            var auxBarManager = Framework.Service<AuxBarManager>();
            auxBarManager.DeleteBar(config.Id);
        };
    }
    
    protected override void OnDraw(ImDrawListPtr _)
    {
        QuerySelector(".name")!.NodeValue = Config.Name;
        ToggleClass("hidden", !Config.IsEnabled);
    }
}
