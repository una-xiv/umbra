namespace Umbra.Windows.Library.Theme;

public class ThemePublishWindow : Window
{
    protected override string  UdtResourceName => "umbra.windows.theme.publish.xml";
    protected override string  Title           => I18N.Translate("Settings.AppearanceModule.PublishTheme");
    protected override Vector2 MinSize         => new(600, 400);
    protected override Vector2 MaxSize         => new(800, 600);
    protected override Vector2 DefaultSize     => new(600, 400);

    protected override void OnOpen()
    {
        RootNode.QuerySelector("#header-title")!.NodeValue =  UmbraColors.GetCurrentProfileName();
        RootNode.QuerySelector("#btn-cancel")!.OnClick     += _ => Close();
        RootNode.QuerySelector("#btn-publish")!.OnClick    += _ => PublishTheme();

        SetStage(Stage.Intro);
    }

    private void PublishTheme()
    {
        SetStage(Stage.Preparing);

        UmbraThemesApi api = Framework.Service<UmbraThemesApi>();

        api.UploadThemeAsync().ContinueWith(task => {
            if (!task.IsCompletedSuccessfully || task.Result == null) {
                SetStage(Stage.Failed);
                return;
            }

            SetStage(Stage.Site);
            api.OpenPublishSite(task.Result);
        });
    }

    private void SetStage(Stage stage)
    {
        string id = stage switch {
            Stage.Intro     => "step-intro",
            Stage.Preparing => "step-preparing",
            Stage.Failed    => "step-failed",
            Stage.Site      => "step-site",
            _               => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };

        foreach (var node in RootNode.QuerySelectorAll(".step")) {
            node.ToggleClass("visible", node.Id == id);
        }

        RootNode.QuerySelector("#btn-publish")!.Style.IsVisible = stage == Stage.Intro;
        RootNode.QuerySelector("#btn-cancel")!.IsDisabled       = stage == Stage.Preparing;
        RootNode.QuerySelector<ButtonNode>("#btn-cancel")!.Label = stage == Stage.Intro
            ? I18N.Translate("Cancel")
            : I18N.Translate("Close");
    }

    private enum Stage
    {
        Intro,
        Preparing,
        Failed,
        Site
    }
}
