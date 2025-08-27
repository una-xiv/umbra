using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;

namespace Umbra.Widgets;

public abstract class StandardToolbarWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : ToolbarWidget(info, guid, configValues)
{
    public sealed override Node Node { get; } = UmbraDrawing.DocumentFrom("umbra.widgets._standard.xml").RootNode!;

    public override WidgetPopup? Popup => null;

    protected abstract StandardWidgetFeatures Features { get; }

    protected Node BodyNode                 => Node.QuerySelector(".wrapper")!;
    protected Node IconNode                 => Node.QuerySelector(".icon")!;
    protected Node SingleLabelNode          => Node.QuerySelector(".single-label")!;
    protected Node MultiLabelNode           => Node.QuerySelector(".multi-label")!;
    protected Node SingleLabelTextNode      => Node.QuerySelector(".single-label > .text")!;
    protected Node MultiLabelTextTopNode    => Node.QuerySelector(".multi-label > .text-top")!;
    protected Node MultiLabelTextBottomNode => Node.QuerySelector(".multi-label > .text-bottom")!;

    protected ProgressBarNode ProgressBarNode => Node.QuerySelector<ProgressBarNode>("#progress-bar")!;

    private bool _isUpdating;

    protected virtual void OnLoad() { }

    protected virtual void OnUnload() { }

    protected virtual void OnDraw() { }

    protected sealed override void Initialize()
    {
        OnLoad();
    }

    protected sealed override void OnUpdate()
    {
        if (_isUpdating) {
            Logger.Warning("OnUpdate called while already updating. Please remove any calls to OnUpdate from your code.");
            return;
        }

        _isUpdating = true;

        Node.ToggleClass("decorated", GetConfigValue<bool>(CvarNameDecorate));

        bool isVertical = IsMemberOfVerticalBar;
        Node? barNode    = GetBarNode;

        switch (CvarSizingMode()) {
            case SizingModeGrow:
                Node.Style.Size = new(0, SafeHeight);
                if (!isVertical) {
                    Node.Style.AutoSize = (AutoSize.Grow, AutoSize.Grow);
                }
                break;
            case SizingModeFixed:
                if (!isVertical) {
                    Node.Style.Size     = new(CvarWidth(), SafeHeight);
                    Node.Style.AutoSize = (AutoSize.Fit, AutoSize.Grow);
                } else {
                    Node.Style.Size = new(0, SafeHeight);
                }

                break;
            default: // Default is Fit
                Node.Style.Size = new(0, SafeHeight);
                if (!isVertical) {
                    Node.Style.AutoSize = (AutoSize.Fit, AutoSize.Grow);
                }

                break;
        }

        if (barNode != null) {
            if (barNode.ClassList.Contains("align-content-left")) {
                BodyNode.Style.Anchor = Anchor.MiddleLeft;
            } else if (barNode.ClassList.Contains("align-content-right")) {
                BodyNode.Style.Anchor = Anchor.MiddleRight;
            } else {
                BodyNode.Style.Anchor = Anchor.MiddleCenter;
            }
        }

        if (HasIconAndText()) {
            Node.ToggleClass("reverse", GetConfigValue<string>(CvarNameIconLocation) == "Right");
        }

        if (IsSingleLabelFeature() || IsMultiLabelFeature()) {
            string value = GetConfigValue<string>(CvarNameTextAlign);
            Node.ToggleClass("text-align-left", value == "Left");
            Node.ToggleClass("text-align-center", value == "Center");
            Node.ToggleClass("text-align-right", value == "Right");
        }

        if (HasBothLabelTypes()) {
            bool showSubText = GetConfigValue<bool>(CvarNameShowSubText);
            SingleLabelNode.Style.IsVisible = (!showSubText && HasMainText()) || (showSubText && HasMainText() && !HasSubText());
            MultiLabelNode.Style.IsVisible  = showSubText && HasMainText() && HasSubText();
        } else if (IsSingleLabelFeature()) {
            MultiLabelNode.Style.IsVisible  = false;
            SingleLabelNode.Style.IsVisible = HasMainText();
        } else if (IsMultiLabelFeature()) {
            SingleLabelNode.Style.IsVisible = !HasSubText() && HasMainText();
            MultiLabelNode.Style.IsVisible  = HasSubText();
        } else {
            SingleLabelNode.Style.IsVisible = false;
            MultiLabelNode.Style.IsVisible  = false;
        }

        if (IsSingleLabelFeature() && HasMainText()) {
            SingleLabelNode.Style.Size           = new(GetConfigValue<int>(CvarNameMinTextWidth), 0);
            SingleLabelTextNode.Style.FontSize   = GetConfigValue<int>(CvarNameSingleLabelTextSize);
            SingleLabelTextNode.Style.TextOffset = new(0, GetConfigValue<int>(CvarNameSingleLabelTextYOffset));
            SingleLabelTextNode.Style.MaxWidth   = GetConfigValue<int>(CvarNameMaxTextWidth);
        }

        if (IsMultiLabelFeature() && HasSubText()) {
            MultiLabelNode.Style.Size                 = new(GetConfigValue<int>(CvarNameMinTextWidth), 0);
            MultiLabelTextTopNode.Style.FontSize      = GetConfigValue<int>(CvarNameMultiLabelTextSizeTop);
            MultiLabelTextTopNode.Style.TextOffset    = new(0, GetConfigValue<int>(CvarNameMultiLabelTextYOffsetTop));
            MultiLabelTextTopNode.Style.MaxWidth      = GetConfigValue<int>(CvarNameMaxTextWidth);
            MultiLabelTextBottomNode.Style.FontSize   = GetConfigValue<int>(CvarNameMultiLabelTextSizeBottom);
            MultiLabelTextBottomNode.Style.TextOffset = new(0, GetConfigValue<int>(CvarNameMultiLabelTextYOffsetBottom));
            MultiLabelTextBottomNode.Style.MaxWidth   = GetConfigValue<int>(CvarNameMaxTextWidth);
        }

        if (HasIconFeature()) {
            if (IsIconCustomizable()) {
                switch (GetConfigValue<string>(CvarNameIconType)) {
                    case "Game":
                        SetGameIconId(GetConfigValue<uint>(CvarNameGameIconId));
                        break;
                    case "FA":
                        SetFontAwesomeIcon(GetConfigValue<FontAwesomeIcon>(CvarNameFontAwesomeIcon));
                        break;
                    case "Glyph":
                        SetGameGlyphIcon(GetConfigValue<SeIconChar>(CvarNameGameGlyphIcon));
                        break;
                    case IconTypeBitmapIcon:
                        SetGfdIcon(GetConfigValue<BitmapFontIcon>(CvarNameBitmapFontIcon));
                        break;
                }
            }

            SetIconColor(new(GetConfigValue<uint>(CvarNameIconColor)));
            SetIconDesaturated(GetConfigValue<bool>(CvarNameDesaturateIcon));
            SetIconSize(GetConfigValue<int>(CvarNameIconSize));
            SetIconOffset(GetConfigValue<int>(CvarNameIconYOffset));
        }

        if (IsMultiLabelFeature()) {
            if (GetConfigValue<bool>(CvarNameUseCustomTextColor)) {
                SetTextColor(
                    new(GetConfigValue<uint>(CvarNameTextColor)),
                    new(GetConfigValue<uint>(CvarNameTextOutlineColor))
                );
                SetSubTextColor(
                    new(GetConfigValue<uint>(CvarNameSubTextColor)),
                    new(GetConfigValue<uint>(CvarNameSubTextOutlineColor))
                );
            } else {
                SetTextColor(null, null);
                SetSubTextColor(null, null);
            }
        } else if (IsSingleLabelFeature()) {
            if (GetConfigValue<bool>(CvarNameUseCustomTextColor)) {
                SetTextColor(
                    new(GetConfigValue<uint>(CvarNameTextColor)),
                    new(GetConfigValue<uint>(CvarNameTextOutlineColor))
                );
            } else {
                SetTextColor(null, null);
            }
        }

        if (HasIconAndTextFeature()) {
            string displayMode = GetConfigValue<string>(CvarNameDisplayMode);
            Node.ToggleClass("text-only", displayMode == DisplayModeTextOnly);
            Node.ToggleClass("icon-only", displayMode == DisplayModeIconOnly);
        }

        int padding = GetConfigValue<int>(CvarNameHorizontalPadding);

        BodyNode.Style.Padding = new(0, padding);

        BodyNode.QuerySelector(".body")!.Style.IsVisible = ShouldShowBody();
        IconNode.Style.IsVisible                         = ShouldShowIcon();

        if (!HasProgressBarFeature() || !GetConfigValue<bool>(CvarNameShowProgressBar)) {
            ProgressBarNode.Style.IsVisible = false;
        } else {
            ProgressBarNode.Style.IsVisible = GetConfigValue<bool>(CvarNameShowProgressBar) && ProgressBarVisibility;
            ProgressBarNode.Direction = GetConfigValue<string>(CvarNameProgressBarFillDirection) switch {
                "LeftToRight" => ProgressBarNode.FillDirection.LeftToRight,
                "RightToLeft" => ProgressBarNode.FillDirection.RightToLeft,
                _             => ProgressBarNode.FillDirection.LeftToRight,
            };

            bool useCustomColor = GetConfigValue<bool>(CvarNameUseCustomProgressBarColor);
            ProgressBarNode.BarNode.Style.BackgroundColor      = useCustomColor ? new(GetConfigValue<uint>(CvarNameCustomProgressBarColor)) : ProgressBarColorOverride;
            ProgressBarNode.OverflowNode.Style.BackgroundColor = useCustomColor ? new(GetConfigValue<uint>(CvarNameCustomProgressBarOverflowColor)) : ProgressBarOverflowColorOverride;
        }

        try {
            OnDraw();
        } catch (Exception e) {
            Logger.Error(e.Message);
            Logger.Error(e.StackTrace);
        }

        _isUpdating = false;
    }

    protected sealed override void OnDisposed()
    {
        base.OnDisposed();

        OnUnload();
    }

    private bool ShouldShowIcon() => HasIconFeature() && HasIcon() && (!HasTextFeature() || (HasIconAndTextFeature() && GetConfigValue<string>(CvarNameDisplayMode) != "TextOnly"));
    private bool ShouldShowBody() => HasTextFeature() && HasText() && (!HasIconFeature() || (HasIconAndTextFeature() && GetConfigValue<string>(CvarNameDisplayMode) != "IconOnly"));

    #region Widget API

    protected bool IsSubTextEnabled => IsMultiLabelFeature() && GetConfigValue<bool>(CvarNameShowSubText);

    protected void SetDisabled(bool disabled)
    {
        Node.IsDisabled = disabled;
    }

    protected void SetText(object? text)
    {
        SingleLabelTextNode.NodeValue   = text;
        MultiLabelTextTopNode.NodeValue = text;
    }

    protected void SetSubText(object? text)
    {
        MultiLabelTextBottomNode.NodeValue = text;
    }

    protected void SetTextColor(Color? textColor, Color? outlineColor)
    {
        SingleLabelTextNode.Style.Color          = textColor;
        SingleLabelTextNode.Style.OutlineColor   = outlineColor;
        MultiLabelTextTopNode.Style.Color        = textColor;
        MultiLabelTextTopNode.Style.OutlineColor = outlineColor;
    }

    protected void SetSubTextColor(Color? textColor, Color? outlineColor)
    {
        MultiLabelTextBottomNode.Style.Color        = textColor;
        MultiLabelTextBottomNode.Style.OutlineColor = outlineColor;
    }

    protected void SetTooltip(string? text)
    {
        Node.Tooltip = text;
    }

    protected void SetGameIconId(uint iconId)
    {
        IconNode.NodeValue            = null;
        IconNode.Style.IconId         = iconId;
        IconNode.Style.BitmapFontIcon = null;
        IconNode.Style.UldPartId      = null;
        IconNode.Style.UldPartsId     = null;
        IconNode.Style.UldResource    = null;

        IconNode.ToggleClass("fa-icon", false);
        IconNode.ToggleClass("glyph-icon", false);
        IconNode.ToggleClass("game-icon", true);
        IconNode.ToggleClass("uld", false);
    }

    protected void SetFontAwesomeIcon(FontAwesomeIcon icon)
    {
        IconNode.NodeValue            = icon.ToIconString();
        IconNode.Style.IconId         = null;
        IconNode.Style.BitmapFontIcon = null;
        IconNode.Style.UldPartId      = null;
        IconNode.Style.UldPartsId     = null;
        IconNode.Style.UldResource    = null;

        IconNode.ToggleClass("fa-icon", true);
        IconNode.ToggleClass("glyph-icon", false);
        IconNode.ToggleClass("game-icon", false);
        IconNode.ToggleClass("uld", false);
    }

    protected void SetGameGlyphIcon(SeIconChar iconChar)
    {
        IconNode.NodeValue            = iconChar.ToIconString();
        IconNode.Style.IconId         = null;
        IconNode.Style.BitmapFontIcon = null;
        IconNode.Style.UldPartId      = null;
        IconNode.Style.UldPartsId     = null;
        IconNode.Style.UldResource    = null;

        IconNode.ToggleClass("fa-icon", false);
        IconNode.ToggleClass("glyph-icon", true);
        IconNode.ToggleClass("game-icon", false);
        IconNode.ToggleClass("uld", false);
    }

    protected void SetGfdIcon(BitmapFontIcon icon)
    {
        IconNode.NodeValue            = "";
        IconNode.Style.IconId         = null;
        IconNode.Style.BitmapFontIcon = icon;
        IconNode.Style.UldPartId      = null;
        IconNode.Style.UldPartsId     = null;
        IconNode.Style.UldResource    = null;

        IconNode.ToggleClass("fa-icon", false);
        IconNode.ToggleClass("glyph-icon", false);
        IconNode.ToggleClass("game-icon", true);
        IconNode.ToggleClass("uld", false);
    }

    protected void SetUldIcon(int uldPartId, string uldResource, int uldPartsId)
    {
        IconNode.NodeValue            = null;
        IconNode.Style.IconId         = null;
        IconNode.Style.BitmapFontIcon = null;
        IconNode.Style.UldPartId      = uldPartId;
        IconNode.Style.UldResource    = uldResource;
        IconNode.Style.UldPartsId     = uldPartsId;

        IconNode.ToggleClass("fa-icon", false);
        IconNode.ToggleClass("glyph-icon", false);
        IconNode.ToggleClass("game-icon", true);
        IconNode.ToggleClass("uld", true);
    }

    protected void ClearIcon()
    {
        IconNode.NodeValue            = null;
        IconNode.Style.IconId         = null;
        IconNode.Style.BitmapFontIcon = null;
        IconNode.Style.UldPartId      = null;
        IconNode.Style.UldResource    = null;
        IconNode.Style.UldPartsId     = null;
        IconNode.ToggleClass("fa-icon", false);
        IconNode.ToggleClass("glyph-icon", false);
        IconNode.ToggleClass("game-icon", false);
        IconNode.ToggleClass("uld", false);
    }

    protected void SetIconColor(Color color)
    {
        IconNode.Style.ImageColor = color;
    }

    protected void SetIconSize(int size)
    {
        if (size == 0) size = SafeHeight - 6;

        if (IconNode.ClassList.Contains("fa-icon") || IconNode.ClassList.Contains("glyph-icon")) {
            IconNode.Style.Size     = null;
            IconNode.Style.FontSize = Math.Clamp(size - 8, 6, 32);
        } else {
            IconNode.Style.Size     = new(Math.Clamp(size, 6, 32));
            IconNode.Style.FontSize = null;
        }
    }

    protected void SetIconOffset(int offset)
    {
        if (IconNode.ClassList.Contains("fa-icon") || IconNode.ClassList.Contains("glyph-icon")) {
            IconNode.Style.TextOffset  = new(0, offset);
            IconNode.Style.ImageOffset = null;
        } else {
            IconNode.Style.TextOffset  = null;
            IconNode.Style.ImageOffset = new(0, offset);
        }
    }

    protected void SetIconDesaturated(bool desaturated)
    {
        IconNode.ToggleClass("desaturated", desaturated);
    }

    /// <summary>
    /// Sets the minimum and maximum values that the progress bar can take.
    /// </summary>
    protected void SetProgressBarConstraint(int min, int max)
    {
        ProgressBarNode.Minimum = min;
        ProgressBarNode.Maximum = max;
    }

    /// <summary>
    /// Sets the value of the progress bar. Note that the given value is
    /// clamped to the configured minimum and maximum values based on what
    /// is given to the <see cref="SetProgressBarConstraint"/> method.
    /// </summary>
    /// <param name="value">The value to set.</param>
    protected void SetProgressBarValue(int value)
    {
        ProgressBarNode.Value = Math.Clamp(value, ProgressBarNode.Minimum, ProgressBarNode.Maximum);
    }

    protected const string IconTypeGameIcon       = "Game";
    protected const string IconTypeFontAwesome    = "FA";
    protected const string IconTypeGlyphIcon      = "Glyph";
    protected const string IconTypeBitmapIcon     = "Bitmap";
    protected const string DisplayModeTextAndIcon = "TextAndIcon";
    protected const string DisplayModeTextOnly    = "TextOnly";
    protected const string DisplayModeIconOnly    = "IconOnly";
    protected const string SizingModeFit          = "Fit";
    protected const string SizingModeGrow         = "Grow";
    protected const string SizingModeFixed        = "Fixed";

    protected bool   ProgressBarVisibility            = true;
    protected Color? ProgressBarColorOverride         = null;
    protected Color? ProgressBarOverflowColorOverride = null;

    #endregion

    #region Feature Flags

    private bool IsSingleLabelFeature()  => Features.HasFlag(StandardWidgetFeatures.Text);
    private bool IsMultiLabelFeature()   => Features.HasFlag(StandardWidgetFeatures.SubText);
    private bool IsIconCustomizable()    => Features.HasFlag(StandardWidgetFeatures.CustomizableIcon);
    private bool IsSizeCustomizable()    => Features.HasFlag(StandardWidgetFeatures.CustomizableSize);
    private bool HasTextFeature()        => IsSingleLabelFeature() || IsMultiLabelFeature();
    private bool HasBothLabelTypes()     => IsSingleLabelFeature() && IsMultiLabelFeature();
    private bool HasIconFeature()        => Features.HasFlag(StandardWidgetFeatures.Icon);
    private bool HasProgressBarFeature() => Features.HasFlag(StandardWidgetFeatures.ProgressBar);
    private bool HasIconAndTextFeature() => HasIconFeature() && (IsSingleLabelFeature() || IsMultiLabelFeature());

    private bool HasIcon() => IconNode.NodeValue != null || IconNode.Style.IconId > 0 || IconNode.Style.BitmapFontIcon != null || HasUldIcon();

    private bool HasIconAndText() => HasIcon() && HasText();

    private bool HasUldIcon() => IconNode.Style.UldPartId != null && IconNode.Style.UldPartsId != null && IconNode.Style.UldResource != null;

    private bool HasMainText() =>
        (IsSingleLabelFeature() && !string.IsNullOrWhiteSpace(SingleLabelTextNode.NodeValue?.ToString())) ||
        (IsMultiLabelFeature() && !string.IsNullOrWhiteSpace(MultiLabelTextTopNode.NodeValue?.ToString()));

    private bool HasSubText() =>
        IsMultiLabelFeature() && !string.IsNullOrWhiteSpace(MultiLabelTextBottomNode.NodeValue?.ToString());

    private bool HasText() => HasMainText() || HasSubText();

    #endregion

    #region Configuration Variables

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        List<IWidgetConfigVariable> variables = [ConfigVariableDecorate()];

        if (HasIconAndTextFeature()) {
            variables.Add(ConfigVariableDisplayMode());
        }

        variables.Add(ConfigVariableHorizontalPadding());

        variables.AddRange([
            ConfigVariableSizingMode(),
            ConfigVariableWidth(),
        ]);

        if (HasIconFeature()) {
            if (Features.HasFlag(StandardWidgetFeatures.CustomizableIcon)) {
                variables.AddRange([
                    ConfigVariableIconType(),
                    ConfigVariableGameIconId(),
                    ConfigVariableFaIcon(),
                    ConfigVariableGameGlyph(),
                    ConfigVariableBitmapIcon(),
                ]);
            }

            variables.AddRange([
                ConfigVariableDesaturateIcon(),
                ConfigVariableIconLocation(),
                ConfigVariableIconColor(),
                ConfigVariableIconSize(),
                ConfigVariableIconYOffset(),
            ]);
        }

        if (HasBothLabelTypes()) {
            variables.Add(ConfigVariableShowSubText());
        }

        if (IsSingleLabelFeature() || IsMultiLabelFeature()) {
            variables.AddRange([
                ConfigVariableTextAlign(),
                ConfigVariableMinTextWidth(),
                ConfigVariableMaxTextWidth(),
            ]);
        }

        if (IsSingleLabelFeature()) {
            variables.AddRange([
                ConfigVariableSingleLabelTextSize(),
                ConfigVariableSingleLabelTextYOffset(),
            ]);
        }

        if (IsMultiLabelFeature()) {
            variables.AddRange([
                ConfigVariableMultiLabelTextSizeTop(),
                ConfigVariableMultiLabelTextSizeBottom(),
                ConfigVariableMultiLabelTextYOffsetTop(),
                ConfigVariableMultiLabelTextYOffsetBottom(),
            ]);
        }

        if (IsSingleLabelFeature() || IsMultiLabelFeature()) {
            variables.Add(ConfigVariableUseCustomTextColor());

            if (IsSingleLabelFeature()) {
                variables.AddRange([
                    ConfigVariableTextColor(),
                    ConfigVariableTextOutlineColor(),
                ]);
            }

            if (IsMultiLabelFeature()) {
                variables.AddRange([
                    ConfigVariableSubTextColor(),
                    ConfigVariableSubTextOutlineColor(),
                ]);
            }
        }

        if (HasProgressBarFeature()) {
            variables.AddRange([
                ConfigVariableShowProgressBar(),
                ConfigVariableProgressBarFillDirection(),
                ConfigVariableUseCustomProgressBarColor(),
                ConfigVariableCustomProgressBarColor(),
                ConfigVariableCustomProgressBarOverflowColor(),
            ]);
        }

        return variables;
    }

    private const string CvarNameDecorate                       = "Decorate";
    private const string CvarNameDesaturateIcon                 = "DesaturateIcon";
    private const string CvarNameDisplayMode                    = "DisplayMode";
    private const string CvarNameSizingMode                     = "SizingMode";
    private const string CvarNameWidth                          = "Width";
    private const string CvarNameIconType                       = "IconType";
    private const string CvarNameGameIconId                     = "GameIconId";
    private const string CvarNameFontAwesomeIcon                = "FaIcon";
    private const string CvarNameGameGlyphIcon                  = "GlyphIcon";
    private const string CvarNameBitmapFontIcon                 = "BitmapIcon";
    private const string CvarNameIconLocation                   = "IconLocation";
    private const string CvarNameIconColor                      = "IconColor";
    private const string CvarNameTextAlign                      = "TextAlign";
    private const string CvarNameIconSize                       = "IconSize";
    private const string CvarNameIconYOffset                    = "IconYOffset";
    private const string CvarNameHorizontalPadding              = "HorizontalPadding"; // new (prev = ButtonPadding)
    private const string CvarNameMinTextWidth                   = "MinTextWidth";      // new (prev = MaxLabelWidth)
    private const string CvarNameMaxTextWidth                   = "MaxTextWidth";
    private const string CvarNameShowSubText                    = "ShowSubText"; // new
    private const string CvarNameSingleLabelTextSize            = "TextSize";
    private const string CvarNameSingleLabelTextYOffset         = "TextYOffset";
    private const string CvarNameMultiLabelTextSizeTop          = "TextSizeTop";
    private const string CvarNameMultiLabelTextSizeBottom       = "TextSizeBottom";
    private const string CvarNameMultiLabelTextYOffsetTop       = "TextYOffsetTop";
    private const string CvarNameMultiLabelTextYOffsetBottom    = "TextYOffsetBottom";
    private const string CvarNameUseCustomTextColor             = "UseCustomTextColor";
    private const string CvarNameTextColor                      = "TextColor";
    private const string CvarNameTextOutlineColor               = "TextOutlineColor";
    private const string CvarNameSubTextColor                   = "SubTextColor";
    private const string CvarNameSubTextOutlineColor            = "SubTextOutlineColor";
    private const string CvarNameShowProgressBar                = "ShowProgressBar";
    private const string CvarNameProgressBarFillDirection       = "ProgressBarFillDirection";
    private const string CvarNameUseCustomProgressBarColor      = "UseCustomProgressBarColor";
    private const string CvarNameCustomProgressBarColor         = "CustomProgressBarColor";
    private const string CvarNameCustomProgressBarOverflowColor = "CustomProgressBarOverflowColor";

    protected bool            CvarDecorate()                    => GetConfigValue<bool>(CvarNameDecorate);
    protected bool            CvarDesaturateIcon()              => GetConfigValue<bool>(CvarNameDesaturateIcon);
    protected string          CvarDisplayMode()                 => GetConfigValue<string>(CvarNameDisplayMode);
    protected string          CvarSizingMode()                  => GetConfigValue<string>(CvarNameSizingMode);
    protected int             CvarWidth()                       => GetConfigValue<int>(CvarNameWidth);
    protected string          CvarIconType()                    => GetConfigValue<string>(CvarNameIconType);
    protected uint            CvarGameIconId()                  => GetConfigValue<uint>(CvarNameGameIconId);
    protected FontAwesomeIcon CvarFontAwesomeIcon()             => GetConfigValue<FontAwesomeIcon>(CvarNameFontAwesomeIcon);
    protected SeIconChar      CvarGameGlyphIcon()               => GetConfigValue<SeIconChar>(CvarNameGameGlyphIcon);
    protected BitmapFontIcon  CvarBitmapFontIcon()              => GetConfigValue<BitmapFontIcon>(CvarNameBitmapFontIcon);
    protected string          CvarIconLocation()                => GetConfigValue<string>(CvarNameIconLocation);
    protected uint            CvarIconColor()                   => GetConfigValue<uint>(CvarNameIconColor);
    protected int             CvarIconSize()                    => GetConfigValue<int>(CvarNameIconSize);
    protected int             CvarIconYOffset()                 => GetConfigValue<int>(CvarNameIconYOffset);
    protected int             CvarHorizontalPadding()           => GetConfigValue<int>(CvarNameHorizontalPadding);
    protected int             CvarMinTextWidth()                => GetConfigValue<int>(CvarNameMinTextWidth);
    protected int             CvarMaxTextWidth()                => GetConfigValue<int>(CvarNameMaxTextWidth);
    protected bool            CvarShowSubText()                 => GetConfigValue<bool>(CvarNameShowSubText);
    protected int             CvarSingleLabelTextSize()         => GetConfigValue<int>(CvarNameSingleLabelTextSize);
    protected int             CvarSingleLabelTextYOffset()      => GetConfigValue<int>(CvarNameSingleLabelTextYOffset);
    protected int             CvarMultiLabelTextSizeTop()       => GetConfigValue<int>(CvarNameMultiLabelTextSizeTop);
    protected int             CvarMultiLabelTextSizeBottom()    => GetConfigValue<int>(CvarNameMultiLabelTextSizeBottom);
    protected int             CvarMultiLabelTextYOffsetTop()    => GetConfigValue<int>(CvarNameMultiLabelTextYOffsetTop);
    protected int             CvarMultiLabelTextYOffsetBottom() => GetConfigValue<int>(CvarNameMultiLabelTextYOffsetBottom);
    protected bool            CvarUseCustomTextColor()          => GetConfigValue<bool>(CvarNameUseCustomTextColor);
    protected uint            CvarTextColor()                   => GetConfigValue<uint>(CvarNameTextColor);
    protected uint            CvarTextOutlineColor()            => GetConfigValue<uint>(CvarNameTextOutlineColor);
    protected uint            CvarSubTextColor()                => GetConfigValue<uint>(CvarNameSubTextColor);
    protected uint            CvarSubTextOutlineColor()         => GetConfigValue<uint>(CvarNameSubTextOutlineColor);
    protected string          CvarTextAlign()                   => GetConfigValue<string>(CvarNameTextAlign);

    protected virtual bool            DefaultDecorate                    => true;
    protected virtual bool            DefaultDesaturateIcon              => false;
    protected virtual string          DefaultDisplayMode                 => DisplayModeTextAndIcon;
    protected virtual string          DefaultSizingMode                  => SizingModeFit;
    protected virtual int             DefaultWidth                       => 0;
    protected virtual string          DefaultIconType                    => IconTypeGameIcon;
    protected virtual uint            DefaultGameIconId                  => 113;
    protected virtual FontAwesomeIcon DefaultFontAwesomeIcon             => FontAwesomeIcon.Home;
    protected virtual SeIconChar      DefaultGlyphIcon                   => SeIconChar.BoxedStar;
    protected virtual BitmapFontIcon  DefaultBitmapIcon                  => BitmapFontIcon.None;
    protected virtual string          DefaultIconLocation                => "Left";
    protected virtual uint            DefaultIconColor                   => 0xFFFFFFFF;
    protected virtual int             DefaultIconSize                    => 0;
    protected virtual int             DefaultIconYOffset                 => 0;
    protected virtual string          DefaultTextAlign                   => "Left";
    protected virtual int             DefaultMinTextWidth                => 0;
    protected virtual int             DefaultMaxTextWidth                => 0;
    protected virtual bool            DefaultShowSubText                 => true;
    protected virtual int             DefaultSingleLabelTextSize         => 13;
    protected virtual int             DefaultSingleLabelTextYOffset      => 0;
    protected virtual int             DefaultMultiLabelTextSizeTop       => 12;
    protected virtual int             DefaultMultiLabelTextSizeBottom    => 9;
    protected virtual int             DefaultMultiLabelTextYOffsetTop    => 1;
    protected virtual int             DefaultMultiLabelTextYOffsetBottom => -1;
    protected virtual bool            DefaultUseCustomTextColor          => false;
    protected virtual bool            DefaultShowProgressBar             => true;

    private BooleanWidgetConfigVariable ConfigVariableDecorate() => new(
        CvarNameDecorate,
        I18N.Translate("Widgets.Standard.Config.Decorate.Name"),
        I18N.Translate("Widgets.Standard.Config.Decorate.Description"),
        DefaultDecorate
    ) { Category = I18N.Translate("Widgets.Standard.Config.Category.General") };

    private static IntegerWidgetConfigVariable ConfigVariableHorizontalPadding() => new(
        CvarNameHorizontalPadding,
        I18N.Translate("Widgets.Standard.Config.HorizontalPadding.Name"),
        I18N.Translate("Widgets.Standard.Config.HorizontalPadding.Description"),
        0,
        0,
        500
    ) { Category = I18N.Translate("Widgets.Standard.Config.Category.General") };

    private BooleanWidgetConfigVariable ConfigVariableDesaturateIcon() => new(
        CvarNameDesaturateIcon,
        I18N.Translate("Widgets.Standard.Config.DesaturateIcon.Name"),
        I18N.Translate("Widgets.Standard.Config.DesaturateIcon.Description"),
        DefaultDesaturateIcon
    ) {
        Category  = I18N.Translate("Widgets.Standard.Config.Category.Icon"),
        DisplayIf = () => IsIconCustomizable() && GetConfigValue<string>(CvarNameIconType) == "Game",
    };

    private SelectWidgetConfigVariable ConfigVariableDisplayMode() => new(
        CvarNameDisplayMode,
        I18N.Translate("Widgets.Standard.Config.DisplayMode.Name"),
        I18N.Translate("Widgets.Standard.Config.DisplayMode.Description"),
        DefaultDisplayMode,
        new() {
            { DisplayModeTextAndIcon, I18N.Translate("Widgets.Standard.Config.DisplayMode.Option.TextAndIcon") },
            { DisplayModeTextOnly, I18N.Translate("Widgets.Standard.Config.DisplayMode.Option.TextOnly") },
            { DisplayModeIconOnly, I18N.Translate("Widgets.Standard.Config.DisplayMode.Option.IconOnly") }
        }
    ) { Category = I18N.Translate("Widgets.Standard.Config.Category.General") };

    private SelectWidgetConfigVariable ConfigVariableSizingMode() => new(
        CvarNameSizingMode,
        I18N.Translate("Widgets.Standard.Config.SizingMode.Name"),
        I18N.Translate("Widgets.Standard.Config.SizingMode.Description"),
        DefaultSizingMode,
        new() {
            { SizingModeFit, I18N.Translate("Widgets.Standard.Config.SizingMode.Option.Fit") },
            { SizingModeGrow, I18N.Translate("Widgets.Standard.Config.SizingMode.Option.Grow") },
            { SizingModeFixed, I18N.Translate("Widgets.Standard.Config.SizingMode.Option.Fixed") },
        }
    ) {
        Category  = I18N.Translate("Widgets.Standard.Config.Category.General"),
        DisplayIf = IsSizeCustomizable
    };

    private IntegerWidgetConfigVariable ConfigVariableWidth() => new(
        CvarNameWidth,
        I18N.Translate("Widgets.Standard.Config.Width.Name"),
        I18N.Translate("Widgets.Standard.Config.Width.Description"),
        DefaultWidth,
        0
    ) {
        Category  = I18N.Translate("Widgets.Standard.Config.Category.General"),
        DisplayIf = () => IsSizeCustomizable() && GetConfigValue<string>(CvarNameSizingMode) == "Fixed",
    };

    private BooleanWidgetConfigVariable ConfigVariableShowSubText() => new(
        CvarNameShowSubText,
        I18N.Translate("Widgets.Standard.Config.ShowSubText.Name"),
        I18N.Translate("Widgets.Standard.Config.ShowSubText.Description"),
        DefaultShowSubText
    ) { Category = I18N.Translate("Widgets.Standard.Config.Category.Text") };

    private SelectWidgetConfigVariable ConfigVariableIconType() => new(
        CvarNameIconType,
        I18N.Translate("Widgets.Standard.Config.IconType.Name"),
        I18N.Translate("Widgets.Standard.Config.IconType.Description"),
        DefaultIconType,
        new() {
            { IconTypeGameIcon, I18N.Translate("Widgets.Standard.Config.IconType.Option.Game") },
            { IconTypeFontAwesome, I18N.Translate("Widgets.Standard.Config.IconType.Option.FA") },
            { IconTypeGlyphIcon, I18N.Translate("Widgets.Standard.Config.IconType.Option.Glyph") },
            { IconTypeBitmapIcon, I18N.Translate("Widgets.Standard.Config.IconType.Option.Bitmap") }
        }
    ) { Category = I18N.Translate("Widgets.Standard.Config.Category.Icon") };

    private IconIdWidgetConfigVariable ConfigVariableGameIconId() => new(
        CvarNameGameIconId,
        I18N.Translate("Widgets.Standard.Config.GameIconId.Name"),
        I18N.Translate("Widgets.Standard.Config.GameIconId.Description"),
        DefaultGameIconId
    ) {
        Category  = I18N.Translate("Widgets.Standard.Config.Category.Icon"),
        DisplayIf = () => IsIconCustomizable() && GetConfigValue<string>(CvarNameIconType) == IconTypeGameIcon,
    };

    private FaIconWidgetConfigVariable ConfigVariableFaIcon() => new(
        CvarNameFontAwesomeIcon,
        I18N.Translate("Widgets.Standard.Config.FaIcon.Name"),
        I18N.Translate("Widgets.Standard.Config.FaIcon.Description"),
        DefaultFontAwesomeIcon
    ) {
        Category  = I18N.Translate("Widgets.Standard.Config.Category.Icon"),
        DisplayIf = () => IsIconCustomizable() && GetConfigValue<string>(CvarNameIconType) == IconTypeFontAwesome,
    };

    private GameGlyphWidgetConfigVariable ConfigVariableGameGlyph() => new(
        CvarNameGameGlyphIcon,
        I18N.Translate("Widgets.Standard.Config.GameGlyph.Name"),
        I18N.Translate("Widgets.Standard.Config.GameGlyph.Description"),
        DefaultGlyphIcon
    ) {
        Category  = I18N.Translate("Widgets.Standard.Config.Category.Icon"),
        DisplayIf = () => IsIconCustomizable() && GetConfigValue<string>(CvarNameIconType) == IconTypeGlyphIcon,
    };

    private BitmapIconWidgetConfigVariable ConfigVariableBitmapIcon() => new(
        CvarNameBitmapFontIcon,
        I18N.Translate("Widgets.Standard.Config.BitmapIcon.Name"),
        I18N.Translate("Widgets.Standard.Config.BitmapIcon.Description"),
        DefaultBitmapIcon
    ) {
        Category  = I18N.Translate("Widgets.Standard.Config.Category.Icon"),
        DisplayIf = () => IsIconCustomizable() && GetConfigValue<string>(CvarNameIconType) == IconTypeBitmapIcon,
    };

    private SelectWidgetConfigVariable ConfigVariableIconLocation() => new(
        CvarNameIconLocation,
        I18N.Translate("Widgets.Standard.Config.IconLocation.Name"),
        I18N.Translate("Widgets.Standard.Config.IconLocation.Description"),
        DefaultIconLocation,
        new() {
            { "Left", I18N.Translate("Widgets.Standard.Config.IconLocation.Option.Left") },
            { "Right", I18N.Translate("Widgets.Standard.Config.IconLocation.Option.Right") }
        }
    ) {
        Category  = I18N.Translate("Widgets.Standard.Config.Category.Icon"),
        DisplayIf = HasIconAndTextFeature
    };

    private ColorWidgetConfigVariable ConfigVariableIconColor() => new(
        CvarNameIconColor,
        I18N.Translate("Widgets.Standard.Config.IconColor.Name"),
        I18N.Translate("Widgets.Standard.Config.IconColor.Description"),
        DefaultIconColor
    ) {
        Category  = I18N.Translate("Widgets.Standard.Config.Category.Icon"),
        DisplayIf = () => IsIconCustomizable() && GetConfigValue<string>(CvarNameIconType) == IconTypeGameIcon,
    };

    private IntegerWidgetConfigVariable ConfigVariableIconSize() => new(
        CvarNameIconSize,
        I18N.Translate("Widgets.Standard.Config.IconSize.Name"),
        I18N.Translate("Widgets.Standard.Config.IconSize.Description"),
        DefaultIconSize,
        0,
        42
    ) { Category = I18N.Translate("Widgets.Standard.Config.Category.Icon") };

    private IntegerWidgetConfigVariable ConfigVariableIconYOffset() => new(
        CvarNameIconYOffset,
        I18N.Translate("Widgets.Standard.Config.IconYOffset.Name"),
        I18N.Translate("Widgets.Standard.Config.IconYOffset.Description"),
        DefaultIconYOffset,
        -5,
        5
    ) { Category = I18N.Translate("Widgets.Standard.Config.Category.Icon") };


    private SelectWidgetConfigVariable ConfigVariableTextAlign() => new(
        CvarNameTextAlign,
        I18N.Translate("Widgets.Standard.Config.TextAlign.Name"),
        I18N.Translate("Widgets.Standard.Config.TextAlign.Description"),
        DefaultTextAlign,
        new() {
            { "Left", I18N.Translate("Widgets.Standard.Config.TextAlign.Option.Left") },
            { "Center", I18N.Translate("Widgets.Standard.Config.TextAlign.Option.Center") },
            { "Right", I18N.Translate("Widgets.Standard.Config.TextAlign.Option.Right") }
        }
    ) { Category = I18N.Translate("Widgets.Standard.Config.Category.Text") };

    private IntegerWidgetConfigVariable ConfigVariableMinTextWidth() => new(
        CvarNameMinTextWidth,
        I18N.Translate("Widgets.Standard.Config.MinTextWidth.Name"),
        I18N.Translate("Widgets.Standard.Config.MinTextWidth.Description"),
        DefaultMinTextWidth,
        0,
        500
    ) {
        Category = I18N.Translate("Widgets.Standard.Config.Category.Text"),
        Group    = I18N.Translate("Widgets.Standard.Config.Group.Boundary"),
    };

    private IntegerWidgetConfigVariable ConfigVariableMaxTextWidth() => new(
        CvarNameMaxTextWidth,
        I18N.Translate("Widgets.Standard.Config.MaxTextWidth.Name"),
        I18N.Translate("Widgets.Standard.Config.MaxTextWidth.Description"),
        DefaultMaxTextWidth,
        0,
        1000
    ) {
        Category = I18N.Translate("Widgets.Standard.Config.Category.Text"),
        Group    = I18N.Translate("Widgets.Standard.Config.Group.Boundary"),
    };

    private IntegerWidgetConfigVariable ConfigVariableSingleLabelTextSize() => new(
        CvarNameSingleLabelTextSize,
        I18N.Translate("Widgets.Standard.Config.TextSize.Name"),
        I18N.Translate("Widgets.Standard.Config.TextSize.Description"),
        DefaultSingleLabelTextSize,
        8,
        24
    ) {
        Category  = I18N.Translate("Widgets.Standard.Config.Category.Text"),
        Group     = I18N.Translate("Widgets.Standard.Config.Group.TextSize"),
        DisplayIf = () => IsSingleLabelFeature() || (IsMultiLabelFeature() && !GetConfigValue<bool>(CvarNameShowSubText)),
    };

    private IntegerWidgetConfigVariable ConfigVariableSingleLabelTextYOffset() => new(
        CvarNameSingleLabelTextYOffset,
        I18N.Translate("Widgets.Standard.Config.TextYOffset.Name"),
        I18N.Translate("Widgets.Standard.Config.TextYOffset.Description"),
        DefaultSingleLabelTextYOffset,
        -5,
        5
    ) {
        Category  = I18N.Translate("Widgets.Standard.Config.Category.Text"),
        Group     = I18N.Translate("Widgets.Standard.Config.Group.TextOffset"),
        DisplayIf = () => IsSingleLabelFeature() || (IsMultiLabelFeature() && !GetConfigValue<bool>(CvarNameShowSubText)),
    };

    private IntegerWidgetConfigVariable ConfigVariableMultiLabelTextSizeTop() => new(
        CvarNameMultiLabelTextSizeTop,
        I18N.Translate("Widgets.Standard.Config.TextSizeTop.Name"),
        I18N.Translate("Widgets.Standard.Config.TextSizeTop.Description"),
        DefaultMultiLabelTextSizeTop,
        8,
        24
    ) {
        Category  = I18N.Translate("Widgets.Standard.Config.Category.Text"),
        Group     = I18N.Translate("Widgets.Standard.Config.Group.TextSize"),
        DisplayIf = () => IsMultiLabelFeature() && GetConfigValue<bool>(CvarNameShowSubText),
    };

    private IntegerWidgetConfigVariable ConfigVariableMultiLabelTextSizeBottom() => new(
        CvarNameMultiLabelTextSizeBottom,
        I18N.Translate("Widgets.Standard.Config.TextSizeBottom.Name"),
        I18N.Translate("Widgets.Standard.Config.TextSizeBottom.Description"),
        DefaultMultiLabelTextSizeBottom,
        8,
        24
    ) {
        Category  = I18N.Translate("Widgets.Standard.Config.Category.Text"),
        Group     = I18N.Translate("Widgets.Standard.Config.Group.TextSize"),
        DisplayIf = () => IsMultiLabelFeature() && GetConfigValue<bool>(CvarNameShowSubText),
    };

    private IntegerWidgetConfigVariable ConfigVariableMultiLabelTextYOffsetTop() => new(
        CvarNameMultiLabelTextYOffsetTop,
        I18N.Translate("Widgets.Standard.Config.TextYOffsetTop.Name"),
        I18N.Translate("Widgets.Standard.Config.TextYOffsetTop.Description"),
        DefaultMultiLabelTextYOffsetTop,
        -5,
        5
    ) {
        Category  = I18N.Translate("Widgets.Standard.Config.Category.Text"),
        Group     = I18N.Translate("Widgets.Standard.Config.Group.TextOffset"),
        DisplayIf = () => IsMultiLabelFeature() && GetConfigValue<bool>(CvarNameShowSubText),
    };

    private IntegerWidgetConfigVariable ConfigVariableMultiLabelTextYOffsetBottom() => new(
        CvarNameMultiLabelTextYOffsetBottom,
        I18N.Translate("Widgets.Standard.Config.TextYOffsetBottom.Name"),
        I18N.Translate("Widgets.Standard.Config.TextYOffsetBottom.Description"),
        DefaultMultiLabelTextYOffsetBottom,
        -5,
        5
    ) {
        Category  = I18N.Translate("Widgets.Standard.Config.Category.Text"),
        Group     = I18N.Translate("Widgets.Standard.Config.Group.TextOffset"),
        DisplayIf = () => IsMultiLabelFeature() && GetConfigValue<bool>(CvarNameShowSubText),
    };

    private BooleanWidgetConfigVariable ConfigVariableUseCustomTextColor() => new(
        CvarNameUseCustomTextColor,
        I18N.Translate("Widgets.Standard.Config.UseCustomTextColor.Name"),
        I18N.Translate("Widgets.Standard.Config.UseCustomTextColor.Description"),
        DefaultUseCustomTextColor
    ) {
        Category  = I18N.Translate("Widgets.Standard.Config.Category.Text"),
        Group     = I18N.Translate("Widgets.Standard.Config.Group.TextColor"),
        DisplayIf = () => IsSingleLabelFeature() || IsMultiLabelFeature(),
    };

    private ColorWidgetConfigVariable ConfigVariableTextColor() => new(
        CvarNameTextColor,
        I18N.Translate("Widgets.Standard.Config.TextColor.Name"),
        I18N.Translate("Widgets.Standard.Config.TextColor.Description"),
        Color.GetNamedColor("Widget.Text")
    ) {
        Category  = I18N.Translate("Widgets.Standard.Config.Category.Text"),
        Group     = I18N.Translate("Widgets.Standard.Config.Group.TextColor"),
        DisplayIf = () => (IsSingleLabelFeature() || IsMultiLabelFeature()) && GetConfigValue<bool>(CvarNameUseCustomTextColor),
    };

    private ColorWidgetConfigVariable ConfigVariableTextOutlineColor() => new(
        CvarNameTextOutlineColor,
        I18N.Translate("Widgets.Standard.Config.TextOutlineColor.Name"),
        I18N.Translate("Widgets.Standard.Config.TextOutlineColor.Description"),
        Color.GetNamedColor("Widget.TextOutline")
    ) {
        Category  = I18N.Translate("Widgets.Standard.Config.Category.Text"),
        Group     = I18N.Translate("Widgets.Standard.Config.Group.TextColor"),
        DisplayIf = () => (IsSingleLabelFeature() || IsMultiLabelFeature()) && GetConfigValue<bool>(CvarNameUseCustomTextColor),
    };

    private ColorWidgetConfigVariable ConfigVariableSubTextColor() => new(
        CvarNameSubTextColor,
        I18N.Translate("Widgets.Standard.Config.SubTextColor.Name"),
        I18N.Translate("Widgets.Standard.Config.SubTextColor.Description"),
        Color.GetNamedColor("Widget.TextMuted")
    ) {
        Category = I18N.Translate("Widgets.Standard.Config.Category.Text"),
        Group    = I18N.Translate("Widgets.Standard.Config.Group.TextColor"),
        DisplayIf = () => IsMultiLabelFeature()
                          && GetConfigValue<bool>(CvarNameUseCustomTextColor)
                          && GetConfigValue<bool>(CvarNameShowSubText),
    };

    private ColorWidgetConfigVariable ConfigVariableSubTextOutlineColor() => new(
        CvarNameSubTextOutlineColor,
        I18N.Translate("Widgets.Standard.Config.SubTextOutlineColor.Name"),
        I18N.Translate("Widgets.Standard.Config.SubTextOutlineColor.Description"),
        Color.GetNamedColor("Widget.TextOutline")
    ) {
        Category = I18N.Translate("Widgets.Standard.Config.Category.Text"),
        Group    = I18N.Translate("Widgets.Standard.Config.Group.TextColor"),
        DisplayIf = () => IsMultiLabelFeature()
                          && GetConfigValue<bool>(CvarNameUseCustomTextColor)
                          && GetConfigValue<bool>(CvarNameShowSubText),
    };

    private BooleanWidgetConfigVariable ConfigVariableShowProgressBar() => new(
        CvarNameShowProgressBar,
        I18N.Translate("Widgets.Standard.Config.ShowProgressBar.Name"),
        I18N.Translate("Widgets.Standard.Config.ShowProgressBar.Description"),
        DefaultShowProgressBar
    ) {
        Category = I18N.Translate("Widgets.Standard.Config.Category.ProgressBar"),
    };

    private SelectWidgetConfigVariable ConfigVariableProgressBarFillDirection() => new(
        CvarNameProgressBarFillDirection,
        I18N.Translate("Widgets.Standard.Config.ProgressBarFillDirection.Name"),
        I18N.Translate("Widgets.Standard.Config.ProgressBarFillDirection.Description"),
        "LeftToRight",
        new() {
            { "LeftToRight", I18N.Translate("Widgets.Standard.Config.ProgressBarFillDirection.Option.LeftToRight") },
            { "RightToLeft", I18N.Translate("Widgets.Standard.Config.ProgressBarFillDirection.Option.RightToLeft") },
        }
    ) {
        Category  = I18N.Translate("Widgets.Standard.Config.Category.ProgressBar"),
        DisplayIf = () => HasProgressBarFeature() && GetConfigValue<bool>(CvarNameShowProgressBar),
    };

    private BooleanWidgetConfigVariable ConfigVariableUseCustomProgressBarColor() => new(
        CvarNameUseCustomProgressBarColor,
        I18N.Translate("Widgets.Standard.Config.UseCustomProgressBarColor.Name"),
        I18N.Translate("Widgets.Standard.Config.UseCustomProgressBarColor.Description"),
        false
    ) {
        Category  = I18N.Translate("Widgets.Standard.Config.Category.ProgressBar"),
        DisplayIf = () => HasProgressBarFeature() && GetConfigValue<bool>(CvarNameShowProgressBar),
    };

    private ColorWidgetConfigVariable ConfigVariableCustomProgressBarColor() => new(
        CvarNameCustomProgressBarColor,
        I18N.Translate("Widgets.Standard.Config.CustomProgressBarColor.Name"),
        I18N.Translate("Widgets.Standard.Config.CustomProgressBarColor.Description"),
        Color.GetNamedColor("Widget.TextOutline")
    ) {
        Category = I18N.Translate("Widgets.Standard.Config.Category.ProgressBar"),
        DisplayIf = () => HasProgressBarFeature()
                          && GetConfigValue<bool>(CvarNameShowProgressBar)
                          && GetConfigValue<bool>(CvarNameUseCustomProgressBarColor)
    };

    private ColorWidgetConfigVariable ConfigVariableCustomProgressBarOverflowColor() => new(
        CvarNameCustomProgressBarOverflowColor,
        I18N.Translate("Widgets.Standard.Config.CustomProgressBarOverflowColor.Name"),
        I18N.Translate("Widgets.Standard.Config.CustomProgressBarOverflowColor.Description"),
        Color.GetNamedColor("Widget.TextOutline")
    ) {
        Category = I18N.Translate("Widgets.Standard.Config.Category.ProgressBar"),
        DisplayIf = () => HasProgressBarFeature()
                          && GetConfigValue<bool>(CvarNameShowProgressBar)
                          && GetConfigValue<bool>(CvarNameUseCustomProgressBarColor)
    };

    #endregion
}

[Flags]
public enum StandardWidgetFeatures : byte
{
    /// <summary>
    /// This widget does not have any special characteristics or features,
    /// meaning that the derived class implements rendering logic by itself.
    /// </summary>
    None = 0,

    /// <summary>
    /// Adds functionality and configuration options for the main text label of
    /// the widget. Use <see cref="StandardToolbarWidget.SetText"/> to set the
    /// text content.
    /// </summary>
    Text = 1,

    /// <summary>
    /// Adds functionality and configuration options for the subtext label of
    /// the widget, which is displayed in a smaller format underneath the main
    /// text. Use <see cref="StandardToolbarWidget.SetSubText"/> to set the
    /// subtext content.
    /// </summary>
    SubText = 2,

    /// <summary>
    /// Adds functionality and configuration options for the icon of the widget
    /// that is shown either to the left or to the right of the text.
    /// </summary>
    Icon = 4,

    /// <summary>
    /// Adds options for custom icons to the widget, allowing users to select
    /// an icon and type of icon to use instead of the default icon.
    /// </summary>
    CustomizableIcon = 8,

    /// <summary>
    /// Shows a progress bar as an underlay element of the widget. Using the
    /// <see cref="StandardToolbarWidget.SetProgressBarValue"/> method allows
    /// you to set the progress of the bar. Setting the progress to 0 will hide
    /// the bar.
    /// </summary>
    ProgressBar = 16,

    /// <summary>
    /// Adds options for fine-tuning the widget sizing allowing to choose
    /// between a fixed width, fit content or growing to fill the parent
    /// element free space.
    /// </summary>
    CustomizableSize = 32,
}
