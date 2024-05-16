using Umbra.Common;
using Umbra.Widgets.Library.Test1;

namespace Umbra.Widgets.System;

[Service]
internal class WidgetTest
{
    public WidgetTest(WidgetManager wm)
    {
        wm.RegisterWidget<Test1Widget>("Test1");

        wm.CreateWidget("Test1", "Center", 0, "MyGuid", new() {
            { "StringTest", "This is my awesome widget!" }
        });

        wm.CreateWidget("Test1", "Center", 1, "MyGuid2", new() {
            { "StringTest", "Another one!" }
        });

        Logger.Info(wm.DumpConfiguration());
    }
}
