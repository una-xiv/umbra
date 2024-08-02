using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Linq;
using System.Numerics;
using Umbra.Common;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Widgets.Library.EmoteList.Window;

internal sealed partial class EmotePickerWindow : Windows.Window
{
    public Emote? SelectedEmote { get; private set; }

    protected override Vector2 MinSize     { get; } = new(300, 512);
    protected override Vector2 MaxSize     { get; } = new(600, 1300);
    protected override Vector2 DefaultSize { get; } = new(450, 720);
    protected override string  Title       { get; } = I18N.Translate("Widget.EmoteList.PickerWindow.Title");

    public unsafe EmotePickerWindow()
    {
        var emoteList = Framework.Service<IDataManager>().GetExcelSheet<Emote>()!.ToList();
        emoteList.Sort((a, b) => string.Compare(a.Name.ToString(), b.Name.ToString(), StringComparison.OrdinalIgnoreCase));

        foreach (var emote in emoteList) {
            if (emote.TextCommand.Value == null) continue;
            if (!UIState.Instance()->IsEmoteUnlocked((ushort)emote.RowId)) continue;

            AddEmoteNode(emote);
        }
    }

    /// <inheritdoc/>
    protected override void OnOpen()
    {
        Node.QuerySelector<StringInputNode>("#Search")!.OnValueChanged += OnSearchValueChanged;
    }

    /// <inheritdoc/>
    protected override void OnClose()
    {
        Node.QuerySelector<StringInputNode>("#Search")!.OnValueChanged -= OnSearchValueChanged;
    }

    /// <inheritdoc/>
    protected override void OnUpdate(int instanceId)
    {
        Node.Style.Size                                       = ContentSize;
        Node.QuerySelector("#EmoteList")!.Style.Size          = new(ContentSize.Width, ContentSize.Height - 45);
        Node.QuerySelector("#EmoteListWrapper")!.Style.Size   = new(ContentSize.Width, 0);
        Node.QuerySelector("#SearchPanel")!.Style.Size        = new(ContentSize.Width, 0);
        Node.QuerySelector("#SearchInputWrapper")!.Style.Size = new(ContentSize.Width - 55, 0);

        foreach (Node node in Node.QuerySelectorAll(".emote")) {
            node.Style.Size                               = new(ContentSize.Width - 30, 42);
            node.QuerySelector(".emote-name")!.Style.Size = new(ContentSize.Width - 82, 0);
        }
    }

    private void OnSearchValueChanged(string value)
    {
        foreach (Node node in Node.QuerySelectorAll(".emote")) {
            node.Style.IsVisible = string.IsNullOrEmpty(value) ||
                node.QuerySelector(".emote-name")!.NodeValue!.ToString()!.Contains(value, StringComparison.OrdinalIgnoreCase);
        }
    }
}
