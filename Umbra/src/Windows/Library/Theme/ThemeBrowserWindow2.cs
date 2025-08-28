using Umbra.Windows.Library.Theme.Components;

namespace Umbra.Windows.Library.Theme;

public class ThemeBrowserWindow2 : Window
{
    protected override string  UdtResourceName => "umbra.windows.theme.browse.xml";
    protected override string  Title           => I18N.Translate("Window.ThemeBrowser.Title");
    protected override Vector2 MinSize         => new(800, 480);
    protected override Vector2 MaxSize         => new(1200, 900);
    protected override Vector2 DefaultSize     => new(800, 480);

    private string                          _searchAuthor   = string.Empty;
    private string                          _searchName     = string.Empty;
    private int                             _searchPageNum  = 1;
    private int                             _searchPageSize = 20;
    private int                             _totalThemes    = 0;
    private int                             _totalPages     = 1;
    private UmbraThemesApi.Theme?           _selectedTheme  = null;
    private List<UmbraThemesApi.Theme>      _themes         = [];
    private ButtonNode                      _prevButton     = null!;
    private ButtonNode                      _nextButton     = null!;
    private SelectNode                      _pagerSelect    = null!;
    private ButtonNode                      _applyButton    = null!;
    private ButtonNode                      _undoButton     = null!;
    private Stack<Dictionary<string, uint>> _undoStack      = [];

    protected override void OnOpen()
    {
        _applyButton = RootNode.QuerySelector<ButtonNode>("#btn-apply")!;
        _undoButton  = RootNode.QuerySelector<ButtonNode>("#btn-undo")!;

        RootNode.QuerySelector<ButtonNode>("#btn-close")!.OnMouseUp += _ => Close();

        _applyButton.OnMouseUp += _ => ApplySelectedTheme();
        _undoButton.OnMouseUp  += _ => UndoSelectedTheme();

        BindHeaderButtons();
        BindPagerButtons();
        FetchThemes();
    }

    private void ApplySelectedTheme()
    {
        if (this._selectedTheme == null) return;

        Dictionary<string, uint> current = [];

        foreach (string colorName in Color.GetAssignedNames()) {
            current[colorName] = Color.GetNamedColor(colorName);
        }

        _undoStack.Push(current);

        foreach (var (colorName, colorValue) in this._selectedTheme.Colors) {
            Color.AssignByName(colorName, colorValue);
        }

        _applyButton.IsDisabled = true;
        _undoButton.IsDisabled  = false;
    }

    private void UndoSelectedTheme()
    {
        if (_undoStack.Count == 0) {
            _undoButton.IsDisabled = true;
            return;
        }

        var last = _undoStack.Pop();

        foreach (var (colorName, colorValue) in last) {
            Color.AssignByName(colorName, colorValue);
        }

        if (_undoStack.Count == 0) {
            _undoButton.IsDisabled = true;
        }
    }

    private void BindHeaderButtons()
    {
        RootNode.QuerySelector<ButtonNode>("#btn-search")!.OnClick += _ => FetchThemes();
        RootNode.QuerySelector<StringInputNode>("#search-author")!.OnValueChanged += v => {
            this._searchAuthor = v;
            this.FetchThemes(true);
        };
        RootNode.QuerySelector<StringInputNode>("#search-name")!.OnValueChanged += v => {
            this._searchName = v;
            this.FetchThemes(true);
        };
    }

    private void BindPagerButtons()
    {
        _prevButton  = RootNode.QuerySelector<ButtonNode>("#pager-prev")!;
        _nextButton  = RootNode.QuerySelector<ButtonNode>("#pager-next")!;
        _pagerSelect = RootNode.QuerySelector<SelectNode>("#pager-select")!;

        _prevButton.OnMouseUp += _ => {
            if (this._searchPageNum > 1) {
                this._searchPageNum--;
                FetchThemes();
            }
        };

        _nextButton.OnMouseUp += _ => {
            if (this._searchPageNum < this._totalPages) {
                this._searchPageNum++;
                FetchThemes();
            }
        };

        _pagerSelect.OnValueChanged += v => {
            if (int.TryParse(v, out int pageNum) && pageNum != this._searchPageNum) {
                this._searchPageNum = pageNum;
                FetchThemes();
            }
        };
    }

    private void FetchThemes(bool resetPageNumber = false)
    {
        if (resetPageNumber) {
            this._searchPageNum = 1;
        }

        DisableSearchButtons();

        Framework.Service<UmbraThemesApi>().GetThemeListAsync(this._searchName, this._searchAuthor, this._searchPageNum, this._searchPageSize).ContinueWith(task => {
            if (task.Result == null) {
                EnableSearchButtons();
                return;
            }

            this._totalThemes = task.Result.Total;
            this._themes      = task.Result.Themes;
            this._totalPages  = Math.Clamp((int)Math.Ceiling((double)this._totalThemes / this._searchPageSize), 1, 1000);

            if (this._searchPageNum > this._totalPages) {
                this._searchPageNum = this._totalPages;
            }

            Framework.DalamudFramework.RunOnFrameworkThread(() => {
                EnableSearchButtons();
                UpdatePager();
                UpdateThemes();
            });
        });
    }

    private void UpdateThemes()
    {
        Node body = RootNode.QuerySelector("#theme-list")!;
        body.Clear();

        if (this._themes.Count == 0) {
            body.AppendChild(new Node() {
                ClassList = ["ui-text-default"],
                NodeValue = I18N.Translate("Window.ThemeBrowser.NoResults")
            });

            return;
        }

        int  index  = 0;
        Node target = new() { ClassList = ["theme-row"] };
        body.AppendChild(target);

        foreach (var theme in this._themes) {
            ThemeCardNode card = new(theme);
            target.AppendChild(card);

            card.OnClick       += _ => ActivateTheme(card, theme);
            card.OnDoubleClick += _ => ActivateTheme(card, theme);

            index++;
            if (index > 1) {
                target = new() { ClassList = ["theme-row"] };
                body.AppendChild(target);
                index = 0;
            }
        }
    }

    private void ActivateTheme(ThemeCardNode card, UmbraThemesApi.Theme theme)
    {
        foreach (var c in RootNode.QuerySelector("#theme-list")!.QuerySelectorAll(".theme-card")) {
            c.ToggleClass("selected", c == card);
        }

        this._selectedTheme     = theme;
        _applyButton.IsDisabled = false;
    }

    private void UpdatePager()
    {
        this._prevButton.IsDisabled  = this._searchPageNum <= 1;
        this._nextButton.IsDisabled  = this._searchPageNum >= this._totalPages;
        this._pagerSelect.IsDisabled = this._totalPages > 1;

        this._pagerSelect.Choices = Enumerable.Range(1, this._totalPages).Select(i => i.ToString()).ToList();
        this._pagerSelect.SetValueInternal(this._searchPageNum.ToString());
    }

    private void DisableSearchButtons()
    {
        _pagerSelect.IsDisabled = true;
        _nextButton.IsDisabled  = true;
        _prevButton.IsDisabled  = true;
    }

    private void EnableSearchButtons()
    {
        _pagerSelect.IsDisabled = false;
        _nextButton.IsDisabled  = false;
        _prevButton.IsDisabled  = false;
    }
}
