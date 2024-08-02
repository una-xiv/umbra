using System.Numerics;
using Una.Drawing;

namespace Umbra.Widgets.Library.EmoteList.Window;

internal sealed partial class EmotePickerWindow : Windows.Window
{
    protected override Vector2 MinSize     { get; } = new(300, 512);
    protected override Vector2 MaxSize     { get; } = new(600, 1300);
    protected override Vector2 DefaultSize { get; } = new(450, 720);
    protected override Node    Node        { get; } = new();
    protected override string  Title       { get; } = "Emote Picker";

    /// <inheritdoc/>
    protected override void OnOpen() { }

    /// <inheritdoc/>
    protected override void OnUpdate(int instanceId) { }

    /// <inheritdoc/>
    protected override void OnClose() { }
}
