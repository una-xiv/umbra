namespace Umbra;

internal partial class Toolbar
{
    [ConfigVariable("Toolbar.Enabled", "General", "Toolbar")]
    public static bool Enabled { get; set; } = true;

    [ConfigVariable("Toolbar.IsAutoHideEnabled", "General", "ToolbarVisibilitySettings")]
    public static bool IsAutoHideEnabled { get; set; } = false;

    [ConfigVariable("Toolbar.AutoHideDuringCutscenes", "General", "ToolbarVisibilitySettings")]
    private static bool AutoHideDuringCutscenes { get; set; } = false;

    [ConfigVariable("Toolbar.AutoHideDuringDuty", category: "General", subCategory: "ToolbarVisibilitySettings")]
    private static bool AutoHideDuringDuty { get; set; } = false;

    [ConfigVariable("Toolbar.AutoHideDuringPvp", category: "General", subCategory: "ToolbarVisibilitySettings")]
    private static bool AutoHideDuringPvp { get; set; } = false;

    [ConfigVariable("Toolbar.IsTopAligned", "General", "Toolbar")]
    public static bool IsTopAligned { get; set; } = false;

    [ConfigVariable("Toolbar.IsStretched", "General", "Toolbar")]
    public static bool IsStretched { get; set; } = true;

    [ConfigVariable("Toolbar.RoundedCorners")]
    public static bool RoundedCorners { get; set; } = true;

    [ConfigVariable("Toolbar.EnableShadow")]
    private static bool EnableShadow { get; set; } = true;

    [ConfigVariable("Toolbar.EnableInactiveColors")]
    private static bool EnableInactiveColors { get; set; } = false;

    [ConfigVariable("Toolbar.Height", "General", "Toolbar", min: 26, max: 64)]
    public static int Height { get; set; } = 32;

    [ConfigVariable("Toolbar.ItemSpacing", "General", "Toolbar", min: 0, max: 1000)]
    public static int ItemSpacing { get; set; } = 6;

    [ConfigVariable("Toolbar.MarginLeft", "General", "Toolbar", min: -16384, max: 16384)]
    private static int ToolbarLeftMargin { get; set; } = 0;

    [ConfigVariable("Toolbar.MarginRight", "General", "Toolbar", min: -16384, max: 16384)]
    private static int ToolbarRightMargin { get; set; } = 0;

    [ConfigVariable("Toolbar.YOffset", "General", "Toolbar", min: -16384, max: 16384)]
    private static int YOffset { get; set; } = 0;
    
    #region Legacy Aux Bar - DO NOT USE

    [ConfigVariable("Toolbar.AuxBar.Enabled")]
    public static bool AuxBarEnabled { get; set; } = false;

    [ConfigVariable("Toolbar.AuxBar.Decorate")]
    public static bool AuxBarDecorate { get; set; } = true;

    [ConfigVariable("Toolbar.AuxBar.XPos", min: -10000, max: 10000)]
    public static int AuxBarXPos { get; set; } = 0;

    [ConfigVariable("Toolbar.AuxBar.YPos", min: -10000, max: 10000)]
    public static int AuxBarYPos { get; set; } = 0;

    [ConfigVariable("Toolbar.AuxBar.XAlign", options: ["Left", "Center", "Right"])]
    public static string AuxBarXAlign { get; set; } = "Center";
    
    [ConfigVariable("Toolbar.AuxBar.YAlign", options: ["Top", "Center", "Bottom"])]
    public static string AuxBarYAlign { get; set; } = "Top";

    [ConfigVariable("Toolbar.AuxBar.Width", min: 0)]
    public static int AuxBarWidth { get; set; } = 0;

    [ConfigVariable("Toolbar.AuxBar.EnableShadow")]
    public static bool AuxEnableShadow { get; set; } = true;

    [ConfigVariable("Toolbar.AuxBar.HideInCutscenes")]
    public static bool AuxBarHideInCutscenes { get; set; } = false;

    [ConfigVariable("Toolbar.AuxBar.HideInPvP")]
    public static bool AuxBarHideInPvP { get; set; } = false;

    [ConfigVariable("Toolbar.AuxBar.HideInDuty")]
    public static bool AuxBarHideInDuty { get; set; } = false;

    [ConfigVariable("Toolbar.AuxBar.HideInCombat")]
    public static bool AuxBarHideInCombat { get; set; } = false;

    [ConfigVariable("Toolbar.AuxBar.HideIfUnsheathed")]
    public static bool AuxBarHideIfUnsheathed { get; set; } = false;

    [ConfigVariable("Toolbar.AuxBar.IsConditionallyVisible")]
    public static bool AuxBarIsConditionallyVisible { get; set; } = false;

    [ConfigVariable("Toolbar.AuxBar.HoldKey")]
    public static string AuxBarHoldKey { get; set; } = "None";

    [ConfigVariable("Toolbar.AuxBar.ShowInCutscene")]
    public static bool AuxBarShowInCutscene { get; set; } = true;

    [ConfigVariable("Toolbar.AuxBar.ShowInGPose")]
    public static bool AuxBarShowInGPose { get; set; } = true;

    [ConfigVariable("Toolbar.AuxBar.ShowInInstance")]
    public static bool AuxBarShowInInstance { get; set; } = true;

    [ConfigVariable("Toolbar.AuxBar.ShowInCombat")]
    public static bool AuxBarShowInCombat { get; set; } = true;

    [ConfigVariable("Toolbar.AuxBar.ShowUnsheathed")]
    public static bool AuxBarShowUnsheathed { get; set; } = true;
    
    #endregion
}
