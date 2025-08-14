namespace Umbra.AuxBar;

[Serializable]
public class AuxBarConfig
{
    public string Id                     { get; set; } = "aux";
    public string Name                   { get; set; } = "Untitled Bar";
    public bool   IsEnabled              { get; set; } = true;
    public bool   Decorate               { get; set; } = true;
    public bool   RoundedCorners         { get; set; } = true;
    public int    ItemSpacing            { get; set; } = 2;
    public int    XPos                   { get; set; } = 50;
    public int    YPos                   { get; set; } = 50;
    public string XAlign                 { get; set; } = "Center";
    public string YAlign                 { get; set; } = "Top";
    public bool   EnableShadow           { get; set; } = true;
    public bool   HideInCutscenes        { get; set; } = true;
    public bool   HideInPvP              { get; set; }
    public bool   HideInDuty             { get; set; }
    public bool   HideInCombat           { get; set; }
    public bool   HideIfUnsheathed       { get; set; }
    public bool   IsConditionallyVisible { get; set; }
    public string HoldKey                { get; set; } = "None";
    public bool   ShowInCutscene         { get; set; } = true;
    public bool   ShowInGPose            { get; set; } = true;
    public bool   ShowInInstance         { get; set; } = true;
    public bool   ShowInCombat           { get; set; } = true;
    public bool   ShowUnsheathed         { get; set; } = true;
    public bool   IsVertical             { get; set; }

    public void Update(AuxBarConfig other)
    {
        Name                   = other.Name;
        IsEnabled              = other.IsEnabled;
        Decorate               = other.Decorate;
        XPos                   = other.XPos;
        YPos                   = other.YPos;
        XAlign                 = other.XAlign;
        YAlign                 = other.YAlign;
        EnableShadow           = other.EnableShadow;
        HideInCutscenes        = other.HideInCutscenes;
        HideInPvP              = other.HideInPvP;
        HideInDuty             = other.HideInDuty;
        HideInCombat           = other.HideInCombat;
        HideIfUnsheathed       = other.HideIfUnsheathed;
        IsConditionallyVisible = other.IsConditionallyVisible;
        HoldKey                = other.HoldKey;
        ShowInCutscene         = other.ShowInCutscene;
        ShowInGPose            = other.ShowInGPose;
        ShowInInstance         = other.ShowInInstance;
        ShowInCombat           = other.ShowInCombat;
        ShowUnsheathed         = other.ShowUnsheathed;
        IsVertical             = other.IsVertical;
    }
}
