using FFXIVClientStructs.FFXIV.Client.UI;

namespace Umbra.Widgets;

[ToolbarWidget(
    "Currencies",
    "Widget.Currencies.Name",
    "Widget.Currencies.Description",
    ["currency", "gil", "tomestone", "seals", "company", "rewards"]
)]
internal sealed partial class CurrenciesWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    public override MenuPopup Popup { get; } = new();

    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Icon |
        StandardWidgetFeatures.Text |
        StandardWidgetFeatures.SubText |
        StandardWidgetFeatures.ProgressBar;

    private IDataManager DataManager { get; } = Framework.Service<IDataManager>();
    private IPlayer      Player      { get; } = Framework.Service<IPlayer>();

    protected override void OnLoad()
    {
        UpdateWidgetLabel();
        UpdateCustomCurrencyIds();
        InitializeMenu();

        OnConfigValueChanged += ConfigVariableChanged;
        Node.OnClick         += OnNodeClicked;
        Node.OnRightClick    += OpenCurrenciesWindow;
    }

    protected override void OnUnload()
    {
        OnConfigValueChanged -= ConfigVariableChanged;
        Node.OnClick         -= OnNodeClicked;
        Node.OnRightClick    -= OpenCurrenciesWindow;
    }

    protected override void OnDraw()
    {
        UpdateWidgetLabel();

        Popup.IsDisabled = !GetConfigValue<bool>("EnableMouseInteraction");

        foreach (var currency in DefaultCurrencies.Values) ProcessCurrency(currency);
        foreach (var currency in _customCurrencies.Values) ProcessCurrency(currency);
    }

    private void OnNodeClicked(Node node)
    {
        if (!GetConfigValue<bool>("EnableMouseInteraction")) OpenCurrenciesWindow(node);
    }

    private void ProcessCurrency(Currency currency)
    {
        UpdateCurrency(currency);
        AddOrUpdateButtonForCurrency(currency);
    }

    private unsafe void OpenCurrenciesWindow(Node _)
    {
        UIModule* uiModule = UIModule.Instance();
        if (uiModule == null) return;
        uiModule->ExecuteMainCommand(66);
    }

    private void UpdateWidgetLabel()
    {
        Currency? currency = GetTrackedCurrency();

        if (currency != null) {
            SetWidgetLabelFromCurrency(currency);
            return;
        }

        string customLabel = GetConfigValue<string>("CustomLabel").Trim();
        SetText(string.IsNullOrEmpty(customLabel) ? I18N.Translate("Widget.Currencies.Name") : customLabel);
        SetSubText(null);
        SetProgressBarValue(0);
        SetProgressBarConstraint(0, 1);
        ClearIcon();
    }

    private void SetWidgetLabelFromCurrency(Currency currency)
    {
        string capText  = GetCountText(currency, GetConfigValue<bool>("ShowCapOnWidget"));
        bool   showName = GetConfigValue<bool>("ShowName");

        if (IsSubTextEnabled) {
            SetText(showName ? currency.Name : capText);
            SetSubText(showName ? capText : null);
        } else {
            SetText(showName ? $"{capText} {currency.Name}" : capText);
            SetSubText(null);
        }

        SetGameIconId(currency.IconId);

        if (currency.IsCapped) {
            SetProgressBarConstraint(0, 100);
            ProgressBarNode.UseOverflow     = true;
            ProgressBarNode.Value           = 200;
        } else {
            ProgressBarNode.UseOverflow     = false;

            if (currency.WeeklyCapacity > 0) {
                SetProgressBarConstraint(0, currency.WeeklyCapacity);
                SetProgressBarValue(currency.WeeklyCount);
            } else if (currency.Capacity > 0) {
                SetProgressBarConstraint(0, (int)currency.Capacity);
                SetProgressBarValue(currency.Count);
            } else {
                SetProgressBarConstraint(0, 1);
                SetProgressBarValue(0);
            }
        }
    }

    private void ConfigVariableChanged(IWidgetConfigVariable variable)
    {
        if (variable is StringWidgetConfigVariable { Id: "CustomCurrencyIds" }) {
            UpdateCustomCurrencyIds();
        }
    }
}
