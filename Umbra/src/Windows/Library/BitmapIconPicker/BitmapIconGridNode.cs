using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Umbra.Common;
using Una.Drawing;
using Una.Drawing.Texture;

namespace Umbra.Windows.FaIconPicker;

public class BitmapIconGridNode : Node
{
    /// <summary>
    /// A <see cref="BitmapFontIcon"/> that the user has selected.
    /// </summary>
    public BitmapFontIcon Selected { get; set; }

    /// <summary>
    /// A <see cref="BitmapFontIcon"/> that the user has confirmed
    /// by double-clicking.
    /// </summary>
    public BitmapFontIcon? Confirmed { get; set; }

    /// <summary>
    /// The search filter string to filter the icon grid.
    /// </summary>
    public string SearchFilter { get; set; } = string.Empty;

    private static List<BitmapFontIcon> AllIcons { get; } =
        Enum.GetValues<BitmapFontIcon>().Where(i => i != BitmapFontIcon.None).ToList();

    private Vector2 IconSize { get; set; } = new(48, 48);

    private List<BitmapFontIcon> Icons { get; set; } = [];

    private Dictionary<string, BitmapFontIcon> IconsByName { get; } = GetIconNames();

    private string         _lastSearch = string.Empty;
    private bool           _isFiltering;
    private double         _lastClickTime;
    private BitmapFontIcon _lastClickedIcon;

    public BitmapIconGridNode(BitmapFontIcon selected)
    {
        Selected  = selected;
        NodeValue = " ";
        ClassList = ["fa-icon-grid"];

        FilterIconListInternal();
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

        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(8 * ScaleFactor));
        ImGui.SetCursorScreenPos(pos);

        if (ImGui.BeginChild("BitmapIconPickerIconGrid", size.ToVector2(), false, ImGuiWindowFlags.NoMove)) {
            ImGuiClip.ClippedDraw(Icons, this.DrawIcon, iconsPerRow, rowHeight);
        }

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
            : SearchIcons(SearchFilter);

        Icons.Sort((a, b) => String.Compare(Enum.GetName(a), Enum.GetName(b), StringComparison.OrdinalIgnoreCase));

        _lastSearch  = SearchFilter;
        _isFiltering = false;
    }

    private void DrawIcon(BitmapFontIcon icon)
    {
        ImDrawListPtr drawList = ImGui.GetWindowDrawList();

        Vector2 p1 = ImGui.GetCursorScreenPos();
        Vector2 p2 = p1 + IconSize;

        ImGui.Dummy(IconSize);

        drawList.AddRectFilled(p1, p2, new Color("Input.Background").ToUInt(), 6, ImDrawFlags.RoundCornersAll);
        drawList.AddRect(p1, p2, new Color("Input.Border").ToUInt(), 6, ImDrawFlags.RoundCornersAll, 1);

        IDalamudTextureWrap? wrap = GfdIconRepository.GetIconWrap(icon);
        if (wrap != null) {
            Vector2 size   = new(wrap.Width, wrap.Height);
            Vector2 center = new(p1.X + (IconSize.X / 2), p1.Y + (IconSize.Y / 2));
            Vector2 s1     = new(center.X - (size.X / 2), center.Y - (size.Y / 2));
            Vector2 s2     = new(center.X + (size.X / 2), center.Y + (size.Y / 2));

            drawList.AddImage(wrap.ImGuiHandle, s1, s2);
        }

        if (icon == Selected) {
            drawList.AddRect(p1, p2, 0xFFFFFFFF, 6, ImDrawFlags.RoundCornersAll, 2);
        }

        if (ImGui.IsItemHovered()) {
            drawList.AddRect(p1, p2, 0xFF00FFFF, 6, ImDrawFlags.RoundCornersAll, 1);
        }

        if (ImGui.IsItemClicked(ImGuiMouseButton.Left)) {
            Selected = icon;
            double now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            if (now - _lastClickTime < 250 && _lastClickedIcon == icon) {
                Confirmed = icon;
            } else {
                _lastClickedIcon = icon;
            }

            _lastClickTime = now;
        }
    }

    private List<BitmapFontIcon> SearchIcons(string search)
    {
        var result = new List<BitmapFontIcon>();

        search = search.ToLowerInvariant().Trim();

        foreach (var (name, icon) in IconsByName) {
            if (name.Contains(search)) {
                result.Add(icon);
            }
        }

        return result;
    }

    private static Dictionary<string, BitmapFontIcon> GetIconNames()
    {
        Dictionary<string, BitmapFontIcon> result = [];

        foreach (var name in Enum.GetNames<BitmapFontIcon>()) {
            BitmapFontIcon icon = Enum.Parse<BitmapFontIcon>(name);
            result.Add(name.ToLowerInvariant(), icon);
        }

        return result;
    }
}
