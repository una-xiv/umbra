
using Umbra.Game.Script;
using Umbra.Game.Script.Filters;

namespace Umbra.Windows.Library.ScriptHelp;

public class ScriptHelpWindow : Window
{
    protected override string  Title       { get; } = I18N.Translate("Window.ScriptHelp.Title");
    protected override Vector2 MinSize     { get; } = new(400, 300);
    protected override Vector2 MaxSize     { get; } = new(900, 1000);
    protected override Vector2 DefaultSize { get; } = new(600, 450);

    protected override string UdtResourceName => "umbra.windows.script_help.window.xml";

    protected override void OnOpen()
    {
        RootNode.QuerySelector("#tabBtnPlaceholders")!.OnClick += _ => ActivateTab("Placeholders");
        RootNode.QuerySelector("#tabBtnFunctions")!.OnClick    += _ => ActivateTab("Functions");
        RootNode.QuerySelector("#tabBtnExamples")!.OnClick     += _ => ActivateTab("Examples");

        ActivateTab("Placeholders");

        foreach (var (name, value, desc) in PlaceholderRegistry.All) {
            CreatePlaceholder(name, value, desc);
        }

        foreach (var (name, desc) in FilterFunctionRegistry.All) {
            CreateFunction(name, desc);
        }
    }

    private void ActivateTab(string name)
    {
        Node button  = RootNode.QuerySelector($"#tabBtn{name}")!;
        Node content = RootNode.QuerySelector($"#tab{name}")!;

        foreach (Node btn in RootNode.QuerySelectorAll(".tab-button")) btn.ToggleTag("selected", btn == button);
        foreach (Node tab in RootNode.QuerySelectorAll("#content > .tab")) tab.Style.IsVisible = tab == content;
    }

    private void CreatePlaceholder(string name, string value, string description)
    {
        Node tpl = Document!.CreateNodeFromTemplate("placeholder", new() {
            { "name", $"[{name}]" },
            { "value", value },
            { "description", description }
        });

        tpl.QuerySelector<ButtonNode>(".copy-button")!.OnClick += _ => {
            ImGui.SetClipboardText($"[{name}]");
        };
        
        RootNode.QuerySelector("#tabPlaceholders")!.AppendChild(tpl);
    }

    private void CreateFunction(string name, string description)
    {
        Node tpl = Document!.CreateNodeFromTemplate("placeholder", new() {
            { "name", name },
            { "value", "" },
            { "description", description }
        });
        
        tpl.QuerySelector<ButtonNode>(".copy-button")!.OnClick += _ => {
            ImGui.SetClipboardText(name);
        };

        RootNode.QuerySelector("#tabFunctions")!.AppendChild(tpl);
    }
}
