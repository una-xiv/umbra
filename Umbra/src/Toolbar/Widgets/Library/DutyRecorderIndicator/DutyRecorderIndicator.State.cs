using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using System.Collections.Generic;
using Umbra.Common;

namespace Umbra.Widgets.Library.DutyRecorderIndicator;

internal sealed unsafe partial class DutyRecorderIndicator
{
    private delegate bool DisplayRecordingIndicatorDelegate(nint agent);

    private Hook<DisplayRecordingIndicatorDelegate>? DisplayRecordingIndicatorHook { get; set; }

    private void SetupHook()
    {
        DisplayRecordingIndicatorHook = Framework
            .Service<IGameInteropProvider>()
            .HookFromSignature<DisplayRecordingIndicatorDelegate>(
                "E8 ?? ?? ?? ?? 44 0F B6 C0 BA 4F 00 00 00",
                DisplayRecordingIndicatorDetour
            );

        DisplayRecordingIndicatorHook?.Enable();
    }

    private void DisposeHook()
    {
        DisplayRecordingIndicatorHook?.Dispose();
    }

    private bool DisplayRecordingIndicatorDetour(nint agent)
    {
        return DisplayRecordingIndicatorHook?.Original(agent) ?? false;
    }

    private bool IsRecordingDuty => DisplayRecordingIndicatorHook?.Original(0) ?? false;

}
