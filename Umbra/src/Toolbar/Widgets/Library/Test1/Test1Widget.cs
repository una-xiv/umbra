using System;
using System.Timers;
using Una.Drawing;

namespace Umbra.Widgets.Library.Test1;

public class Test1Widget : DefaultToolbarWidget
{
    public override string Name => "Test1";

    public override Node? PopupNode => null;

    public Test1Widget()
    {
        // Execute a function on an interval of 500ms.
        Timer timer = new();
        timer.Interval = 1000;

        string[] str = [
            "A button",
            "Some widget",
            "Something with a long name",
            "Yes.",
            "No."
        ];

        timer.Elapsed += (sender, args) => {
            Node.Style.IsVisible = Random.Shared.Next(0, 10) < 8;
            SetLeftIcon(Random.Shared.Next(0,  10) < 5 ? null : (uint)(1 + Random.Shared.Next(0, 20)));
            SetRightIcon(Random.Shared.Next(0, 10) < 5 ? null : (uint)(1 + Random.Shared.Next(0, 20)));
            SetGhost(Random.Shared.Next(0,    10) < 5);
            SetDisabled(Random.Shared.Next(0, 10) < 5);
            SetLabel(str[Random.Shared.Next(0, str.Length)]);

            if (Random.Shared.Next(0, 10) < 2) {
                SetTwoLabels(str[Random.Shared.Next(0, str.Length)], str[Random.Shared.Next(0, str.Length)]);
            }
        };

        timer.Start();
    }
}
