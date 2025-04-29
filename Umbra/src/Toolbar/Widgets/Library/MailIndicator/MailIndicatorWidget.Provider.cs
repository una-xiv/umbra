using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.UI.Info;

namespace Umbra.Widgets;

internal unsafe partial class MailIndicatorWidget
{
    private static uint GetUnreadMailCount()
    {
        var ipl = (InfoProxyLetterCount*)InfoModule.Instance()->GetInfoProxyById(InfoProxyId.Letter);

        return ipl->NumLetters;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct InfoProxyLetterCount
    {
        [FieldOffset(0x26)] public byte NumLetters;
    }
}
