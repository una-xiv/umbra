using Umbra.Common;
using Umbra.Interface;
using Umbra.Windows.ConfigWindow;

namespace Umbra;

[Service]
public sealed class UmbraDebug
{
    public UmbraDebug(WindowManager wm)
    {
        wm.CreateWindow<ConfigWindow>();
    }
}
