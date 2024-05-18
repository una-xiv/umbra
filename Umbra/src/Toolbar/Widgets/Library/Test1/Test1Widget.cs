using System;
using System.Collections.Generic;
using Umbra.Common;
using Umbra.Windows;

namespace Umbra.Widgets.Library.Test1;

public class Test1Widget(string? guid = null, Dictionary<string, object>? configValues = null) : DefaultToolbarWidget(
    guid,
    configValues
)
{
    public override string Name => "Test1";

    public override MenuPopup? Popup { get; } = new MenuPopup();

    /// <inheritdoc/>
    protected override void Initialize()
    {
        DoRandomStuff();
        SetLabel(GetConfigValue<string>("StringTest"));

        Popup!.AddButton("Button1", "A",       0, 14, "[F1]", () => { Logger.Info("Button1 clicked!"); });
        Popup!.AddButton("Button2", "B",                 1, 15, "[F3]", () => { Logger.Info("Button2 clicked!"); });
        Popup!.AddButton("Button3", "C", 2, null, "[CTRL-SHIFT-ALT-DEL-F2]",
            () => {
                Popup!.AddButton("Button4", "Group Button #1", 0, 16, null, () => { Logger.Info("Button4 clicked!"); }, "Group1");
                Popup!.AddButton("Button5", "Group Button #2", 1, null, "[F3]", () => { Logger.Info("Button5 clicked!"); }, "Group1");
            });

        Popup!.AddGroup("Group1", "This is a group", 3);
        Popup!.AddGroup("Group2", "This is another group", 4);

        Popup!.AddButton("Button6", "A grouped button with a very long name",        altText: "A", groupId: "Group2", onClick: () => { Logger.Info("Button6 clicked!"); });
        Popup!.AddButton("Button7", "Another disabled", altText: "A",groupId: "Group2", onClick: () => { Logger.Info("Button7 clicked!"); });
        Popup!.AddButton("Button8", "A hidden button",         altText: "A",groupId: "Group2", onClick: () => { Logger.Info("Button8 clicked!"); });
        Popup!.AddButton("Button9", "Boooooyeah!",             groupId: "Group2", onClick: () => {
            Framework.Service<WindowManager>().Present("Booyah", new TestWindow(),
                window => {
                    Logger.Info("Window is closed!");
                });
        });

        // Popup!.SetButtonDisabled("Button7", true);
        // Popup!.SetButtonVisibility("Button8", false);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        // Do stuff.
    }

    /// <inheritdoc/>
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new BooleanWidgetConfigVariable("BooleanTest", "This is a boolean", true),
            new IntegerWidgetConfigVariable("IntegerTest", "This is an integer", 42, 0, 100),
            new FloatWidgetConfigVariable("FloatTest", "This is a float", 3.14f, 0, 10),
            new StringWidgetConfigVariable("StringTest", "This is a string", "Hello, world!"),
        ];
    }

    private void DoRandomStuff()
    {
        string[] str = [
            "A button",
            "Some widget",
            "Something with a long name",
            "Yes.",
            "No."
        ];

        SetLeftIcon(Random.Shared.Next(0,  10) < 5 ? null : (uint)(1 + Random.Shared.Next(0, 20)));
        SetRightIcon(Random.Shared.Next(0, 10) < 5 ? null : (uint)(1 + Random.Shared.Next(0, 20)));
        SetGhost(Random.Shared.Next(0, 10) < 5);
        SetLabel(str[Random.Shared.Next(0, str.Length)]);

        if (Random.Shared.Next(0, 10) < 2) {
            SetTwoLabels(str[Random.Shared.Next(0, str.Length)], str[Random.Shared.Next(0, str.Length)]);
        }
    }
}
