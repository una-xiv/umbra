using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility;
using Dalamud.Plugin.Services;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Umbra.Common;
using Una.Drawing;

namespace Umbra.Windows.Library.IconPicker;

public class FaIconGridNode : Node
{
    public FontAwesomeIcon Selected     { get; set; }
    public string          SearchFilter { get; set; } = string.Empty;

    private ITextureProvider TextureProvider { get; } = Framework.Service<ITextureProvider>();

    private Vector2 IconSize { get; set; } = new(48, 48);

    private List<FontAwesomeIcon> AllIcons { get; }      = Enum.GetValues<FontAwesomeIcon>().ToList();
    private List<FontAwesomeIcon> Icons    { get; set; } = Enum.GetValues<FontAwesomeIcon>().ToList();

    private string _lastSearch = string.Empty;
    private bool _isFiltering  = false;

    public FaIconGridNode(FontAwesomeIcon selected)
    {
        Selected  = selected;
        NodeValue = " ";
    }

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        if (ParentNode == null || ParentNode.Bounds.ContentSize.IsZero) return;

        if (SearchFilter != _lastSearch && !_isFiltering) {
            FilterIconList();
        }

        IconSize = new(48 * ScaleFactor);

        Size    size        = ParentNode.Bounds.ContentSize - new Size((int)(8 * ScaleFactor), (int)(16 * ScaleFactor));
        Vector2 pos         = ParentNode.Bounds.ContentRect.TopLeft + new Vector2(8 * ScaleFactor);
        int     rowHeight   = (int)(IconSize.Y + (8 * ScaleFactor));
        int     iconsPerRow = (int)MathF.Floor(size.Width / (IconSize.X + (8 * ScaleFactor)));

        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding,  new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing,   new Vector2(8 * ScaleFactor));
        ImGui.SetCursorScreenPos(pos);

        Framework.DalamudPlugin.UiBuilder.IconFontHandle.Push();

        if (ImGui.BeginChild("FaIconPickerIconGrid", size.ToVector2(), false, ImGuiWindowFlags.NoMove)) {
            ImGuiClip.ClippedDraw(Icons, this.DrawIcon, iconsPerRow, rowHeight);
        }

        Framework.DalamudPlugin.UiBuilder.IconFontHandle.Pop();

        ImGui.EndChild();
        ImGui.PopStyleVar(3);
    }

    private void FilterIconList()
    {
        _isFiltering = true;
        Task.Run(FilterIconListInternal);
    }

    private void FilterIconListInternal()
    {
        Icons = string.IsNullOrEmpty(SearchFilter)
            ? AllIcons
            : FontAwesomeHelpers.SearchIcons(SearchFilter, string.Empty);

        _lastSearch  = SearchFilter;
        _isFiltering = false;
    }

    private void DrawIcon(FontAwesomeIcon icon)
    {
        ImDrawListPtr drawList = ImGui.GetWindowDrawList();

        Vector2 p1 = ImGui.GetCursorScreenPos();
        Vector2 p2 = p1 + IconSize;

        ImGui.Dummy(IconSize);

        Vector2 textSize = ImGui.CalcTextSize(icon.ToIconString());
        Vector2 textPos  = p1 + new Vector2((IconSize.X - textSize.X) / 2, (IconSize.Y / 2) - (textSize.Y / 2));

        drawList.AddRectFilled(p1, p2, new Color("Input.Background").ToUInt(), 6, ImDrawFlags.RoundCornersAll);
        drawList.AddRect(p1, p2, new Color("Input.Border").ToUInt(), 6, ImDrawFlags.RoundCornersAll, 1);
        drawList.AddText(textPos, 0xFFFFFFFF, icon.ToIconString());

        if (icon == Selected) {
            drawList.AddRect(p1, p2, 0xFFFFFFFF, 6, ImDrawFlags.RoundCornersAll, 2);
        }

        if (ImGui.IsItemHovered()) {
            drawList.AddRect(p1, p2, 0xFF00FFFF, 6, ImDrawFlags.RoundCornersAll, 1);
        }

        if (ImGui.IsItemClicked(ImGuiMouseButton.Left)) {
            Selected = icon;
        }
    }
}
