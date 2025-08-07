using Dalamud.Game.Text;
using Dalamud.Interface.Utility;

using System.Threading.Tasks;

namespace Umbra.Windows.GameGlyphPicker;

public class GameGlyphGridNode : Node
{
    /// <summary>
    /// A <see cref="SeIconChar"/> that the user has selected.
    /// </summary>
    public SeIconChar? Selected { get; set; }

    /// <summary>
    /// A <see cref="SeIconChar"/> that the user has confirmed
    /// by double-clicking.
    /// </summary>
    public SeIconChar? Confirmed { get; set; }

    /// <summary>
    /// The search filter string to filter the icon grid.
    /// </summary>
    public string SearchFilter { get; set; } = string.Empty;

    private static List<SeIconChar> AllIcons { get; } = Enum.GetValues<SeIconChar>().ToList();

    private Vector2 IconSize { get; set; } = new(48, 48);

    private List<SeIconChar> Icons { get; set; } = [];

    private Dictionary<string, SeIconChar> IconsByName { get; } = GetIconNames();

    private string      _lastSearch = string.Empty;
    private bool        _isFiltering;
    private double      _lastClickTime;
    private SeIconChar? _lastClickedIcon;

    public GameGlyphGridNode(SeIconChar? selected)
    {
        Selected  = selected;
        NodeValue = " ";
        ClassList = ["se-icon-grid"];

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

        if (ImGui.BeginChild("SeIconPickerIconGrid", size.ToVector2(), false, ImGuiWindowFlags.NoMove)) {
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

    private void DrawIcon(SeIconChar icon)
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
            double now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            if (now - _lastClickTime < 250 && _lastClickedIcon == icon) {
                Confirmed = icon;
            } else {
                _lastClickedIcon = icon;
            }

            _lastClickTime = now;
        }
    }

    private List<SeIconChar> SearchIcons(string search)
    {
        var result = new List<SeIconChar>();

        search = search.ToLowerInvariant().Trim();
        
        foreach (var (name, icon) in IconsByName) {
            if (name.Contains(search)) {
                result.Add(icon);
            }
        }

        return result;
    }

    private static Dictionary<string, SeIconChar> GetIconNames()
    {
        Dictionary<string, SeIconChar> result = [];

        foreach (var name in Enum.GetNames<SeIconChar>()) {
            SeIconChar icon = Enum.Parse<SeIconChar>(name);
            result.Add(name.ToLowerInvariant(), icon);
        }

        return result;
    }
}
