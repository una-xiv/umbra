using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Umbra.Common;
using Umbra.Game;
using Una.Drawing;

namespace Umbra.Widgets.Popup;

internal sealed partial class GearsetSwitcherPopup : WidgetPopup
{
    protected override Node Node { get; }

    private IGearsetRepository GearsetRepository { get; } = Framework.Service<IGearsetRepository>();
    private IPlayer            Player            { get; } = Framework.Service<IPlayer>();
    private UdtDocument        Document          { get; }
    private Gearset            CurrentGearset    { get; set; } = null!;

    public GearsetSwitcherPopup()
    {
        Document = UmbraDrawing.DocumentFrom("umbra.widgets.popup_gearset_switcher.xml");
        Node     = Document.RootNode!;

        foreach (var btn in Node.QuerySelectorAll(".header .buttons > .uld"))
            btn.OnClick += OnHeaderButtonClicked;

        CreateGroupNodes();
        CreateContextMenu();
    }

    protected override bool CanOpen()
    {
        return null != GearsetRepository.CurrentGearset;
    }

    protected override void OnOpen()
    {
        CurrentGearset = GearsetRepository.CurrentGearset!;
        UpdateGroupPositions();

        GearsetRepository.OnGearsetCreated  += OnGearsetCreatedOrUpdated;
        GearsetRepository.OnGearsetChanged  += OnGearsetCreatedOrUpdated;
        GearsetRepository.OnGearsetRemoved  += OnGearsetRemoved;
        GearsetRepository.OnGearsetEquipped += OnGearsetEquipped;

        foreach (var gearset in GearsetRepository.GetGearsets()) {
            OnGearsetCreatedOrUpdated(gearset);
        }
    }

    protected override void OnClose()
    {
        GearsetRepository.OnGearsetCreated  -= OnGearsetCreatedOrUpdated;
        GearsetRepository.OnGearsetChanged  -= OnGearsetCreatedOrUpdated;
        GearsetRepository.OnGearsetRemoved  -= OnGearsetRemoved;
        GearsetRepository.OnGearsetEquipped -= OnGearsetEquipped;

        foreach (var node in Node.QuerySelectorAll(".gearset")) {
            if (!_nodeToGearset.TryGetValue(node, out var gearset)) continue;
            OnGearsetRemoved(gearset);
        }
    }

    protected override void OnUpdate()
    {
        if (GearsetRepository.CurrentGearset is not { IsValid: true }) {
            Close();
            return;
        }

        CurrentGearset = GearsetRepository.CurrentGearset!;

        UpdateHeaderNodes();
        UpdateGroupPositions();
        UpdateGearsetButtons();
    }

    #region Header

    private Node HeaderIconNode => Node.QuerySelector("#header-icon")!;
    private Node HeaderNameNode => Node.QuerySelector("#header-name")!;
    private Node HeaderInfoNode => Node.QuerySelector("#header-info")!;
    private Node HeaderIlvlNode => Node.QuerySelector("#header-ilvl")!;
    private Node BackgroundNode => Node.QuerySelector(".background")!;

    private void UpdateHeaderNodes()
    {
        var jobInfo = Player.GetJobInfo(CurrentGearset.JobId);

        HeaderIconNode.Style.IconId    = jobInfo.Icons[_headerIconType];
        HeaderNameNode.NodeValue       = CurrentGearset.Name;
        HeaderInfoNode.NodeValue       = GearsetSwitcherInfoDisplayProvider.GetInfoText(GearsetSwitcherInfoDisplayType.JobLevel, CurrentGearset, true);
        HeaderIlvlNode.NodeValue       = $"{CurrentGearset.ItemLevel}";
        BackgroundNode.Style.IsVisible = _showGradientBackground;

        BackgroundNode.ToggleClass("tank", CurrentGearset.Category == GearsetCategory.Tank);
        BackgroundNode.ToggleClass("healer", CurrentGearset.Category == GearsetCategory.Healer);
        BackgroundNode.ToggleClass("melee", CurrentGearset.Category == GearsetCategory.Melee);
        BackgroundNode.ToggleClass("ranged", CurrentGearset.Category == GearsetCategory.Ranged);
        BackgroundNode.ToggleClass("caster", CurrentGearset.Category == GearsetCategory.Caster);
        BackgroundNode.ToggleClass("crafter", CurrentGearset.Category == GearsetCategory.Crafter);
        BackgroundNode.ToggleClass("gatherer", CurrentGearset.Category == GearsetCategory.Gatherer);
    }

    private unsafe void OnHeaderButtonClicked(Node node)
    {
        switch (node.Id) {
            case "BestInSlot":
                AgentModule.Instance()->GetAgentByInternalId(AgentId.RecommendEquip)->Show();
                Close();
                break;
            case "OpenGlam":
                Player.UseGeneralAction(25); // Glamour Plate.
                break;
            case "Update":
                GearsetRepository.UpdateEquippedGearset();
                break;
            case "Duplicate":
                GearsetRepository.DuplicateEquippedGearset();
                break;
            case "RandomJob":
                GearsetRepository.EquipRandomJob();
                break;
        }
    }

    #endregion
}
