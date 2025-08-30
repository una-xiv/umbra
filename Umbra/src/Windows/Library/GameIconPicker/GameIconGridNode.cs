﻿using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility;


namespace Umbra.Windows.GameIconPicker;

public class GameIconGridNode : Node
{
    public uint  SelectedId  { get; set; }
    public uint? ConfirmedId { get; set; }

    private ITextureProvider TextureProvider { get; } = Framework.Service<ITextureProvider>();

    private Vector2 IconSize { get; set; } = new(48, 48);

    private List<uint> IconIds { get; }

    private double _lastClickTime;

    public GameIconGridNode(List<uint> iconIds, uint selectedId)
    {
        IconIds    = iconIds;
        SelectedId = selectedId;
        NodeValue  = " ";
    }

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        if (ParentNode == null || ParentNode.Bounds.ContentSize.IsZero) return;

        IconSize = new(48 * ScaleFactor);

        Size    size        = ParentNode.Bounds.ContentSize - new Size((int)(8 * ScaleFactor), (int)(16 * ScaleFactor));
        Vector2 pos         = ParentNode.Bounds.ContentRect.TopLeft + new Vector2(8 * ScaleFactor);
        int     rowHeight   = (int)(IconSize.Y + (8 * ScaleFactor));
        int     iconsPerRow = (int)MathF.Floor(size.Width / (IconSize.X + (8 * ScaleFactor)));

        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(8 * ScaleFactor));
        ImGui.PushStyleColor(ImGuiCol.ScrollbarBg, Color.GetNamedColor("Window.ScrollbarTrack"));
        ImGui.PushStyleColor(ImGuiCol.ScrollbarGrab, Color.GetNamedColor("Window.ScrollbarThumb"));
        ImGui.PushStyleColor(ImGuiCol.ScrollbarGrabHovered, Color.GetNamedColor("Window.ScrollbarThumbHover"));
        ImGui.PushStyleColor(ImGuiCol.ScrollbarGrabActive, Color.GetNamedColor("Window.ScrollbarThumbActive"));
        ImGui.SetCursorScreenPos(pos);

        if (ImGui.BeginChild("IconPickerIconGrid", size.ToVector2(), false, ImGuiWindowFlags.NoMove)) {
            ImGuiClip.ClippedDraw(IconIds, this.DrawIcon, iconsPerRow, rowHeight);
        }

        ImGui.EndChild();
        ImGui.PopStyleVar(3);
        ImGui.PopStyleColor(4);
    }

    private void DrawIcon(uint iconId)
    {
        Vector2       pos      = ImGui.GetCursorScreenPos();
        ImDrawListPtr drawList = ImGui.GetWindowDrawList();

        Vector2 p1 = pos;
        Vector2 p2 = pos + IconSize;
        Vector2 p3 = p1 + new Vector2(2);
        Vector2 p4 = p2 - new Vector2(2);

        ImGui.Dummy(IconSize);

        drawList.AddRectFilled(p1, p2, new Color("Input.Background").ToUInt(), 6, ImDrawFlags.RoundCornersAll);
        drawList.AddRect(p1, p2, new Color("Input.Border").ToUInt(), 6, ImDrawFlags.RoundCornersAll, 1);


        if (TextureProvider.TryGetFromGameIcon(new(iconId), out var texture) && texture.TryGetWrap(out IDalamudTextureWrap? wrap, out Exception? _)) {
            drawList.AddImageRounded(
                wrap.Handle,
                p3,
                p4,
                Vector2.Zero,
                Vector2.One,
                0xFFFFFFFF,
                6,
                ImDrawFlags.RoundCornersAll
            );
        } else {
            drawList.AddRectFilled(p1, p2, 0xA00000FF, 6, ImDrawFlags.RoundCornersAll);
            drawList.AddText(p1 + new Vector2(4), 0xFFFFFFFF, iconId.ToString());
        }

        if (iconId == SelectedId) {
            drawList.AddRect(p1, p2, 0xFFFFFFFF, 6, ImDrawFlags.RoundCornersAll, 2);
        }

        if (ImGui.IsItemHovered()) {
            drawList.AddRect(p1, p2, 0xFF00FFFF, 6, ImDrawFlags.RoundCornersAll, 1);
        }

        if (ImGui.IsItemClicked(ImGuiMouseButton.Left)) {
            SelectedId = iconId;
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            if (_lastClickTime > 0 && now - _lastClickTime < 250) {
                ConfirmedId = iconId;
            }

            _lastClickTime = now;
        }
    }
}
