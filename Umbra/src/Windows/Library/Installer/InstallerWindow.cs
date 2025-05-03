using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Umbra.Common;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Windows.Library.Installer;

internal sealed class InstallerWindow : Window
{
    protected override string  UdtResourceName => "umbra.windows.installer.window.xml";
    protected override string  Title           => I18N.Translate("Window.Installer.Title");
    protected override Vector2 MinSize         => new(820, 500);
    protected override Vector2 MaxSize         => new(820, 500);
    protected override Vector2 DefaultSize     => new(820, 500);

    private static List<InstallerPage> Pages { get; set; } = [];

    private uint           _currentPageIndex;
    private InstallerPage? _currentPage;

    private ButtonNode PrevButton => RootNode.QuerySelector<ButtonNode>("#btn-prev")!;
    private ButtonNode NextButton => RootNode.QuerySelector<ButtonNode>("#btn-next")!;

    private CheckboxNode DontShowAgain    => RootNode.QuerySelector<CheckboxNode>("#dont-show-again")!;
    private Node         FooterSpacerNode => RootNode.QuerySelector("#footer-spacer")!;

    protected override void OnOpen()
    {
        LoadInstallerPages();
        LoadPage(0);

        PrevButton.OnClick += _ => OnPrevButtonClicked();
        NextButton.OnClick += _ => OnNextButtonClicked();

        DontShowAgain.Value = !UmbraBindings.IsFirstTimeStart;
        DontShowAgain.OnValueChanged += v => ConfigManager.Set("IsFirstTimeStart.V3", !v);
    }

    protected override void OnClose()
    {
        _currentPage?.Deactivate();
        _currentPageIndex = 0;
    }

    protected override void OnDraw()
    {
        NextButton.IsDisabled = !_currentPage?.CanProceed() ?? false;
    }

    private void LoadPage(uint pageIndex)
    {
        if (pageIndex >= Pages.Count) {
            ConfigManager.Set("IsFirstTimeStart.V3", false);
            Close();
            return;
        }

        var page = Pages[(int)pageIndex];
        if (page == _currentPage) return;

        _currentPage?.Deactivate();

        int dir = pageIndex > _currentPageIndex ? 1 : -1;
        
        if (!page.CanActivate()) {
            LoadPage((uint)(pageIndex + dir));
            return;
        }
        
        _currentPage      = page;
        _currentPageIndex = pageIndex;

        RootNode.QuerySelector("#content")!.Clear();
        RootNode.QuerySelector("#content")!.AppendChild(_currentPage.Activate());

        PrevButton.Label = I18N.Translate(IsFirstPage() ? "Cancel" : "Previous");
        NextButton.Label = I18N.Translate(IsLastPage() ? "Finish" : "Next");

        DontShowAgain.Style.IsVisible    = IsFirstPage();
        FooterSpacerNode.Style.IsVisible = !IsFirstPage();
    }

    private bool IsFirstPage() => _currentPageIndex == 0;
    private bool IsLastPage()  => _currentPageIndex == Pages.Count - 1;

    private void OnPrevButtonClicked()
    {
        if (IsFirstPage()) {
            Close();
            return;
        }

        LoadPage(_currentPageIndex - 1);
    }

    private void OnNextButtonClicked()
    {
        LoadPage(_currentPageIndex + 1);
    }

    private static void LoadInstallerPages()
    {
        if (Pages.Count > 0) return;

        foreach (var type in Assembly.GetExecutingAssembly()
                                     .GetTypes()
                                     .Where(t => t is { IsClass: true, IsAbstract: false } && t.IsSubclassOf(typeof(InstallerPage)))
        ) {
            if (Framework.InstantiateWithDependencies(type) is InstallerPage page) {
                Pages.Add(page);
            }
        }

        Pages = Pages.OrderBy(x => x.Order).ToList();
    }
}
