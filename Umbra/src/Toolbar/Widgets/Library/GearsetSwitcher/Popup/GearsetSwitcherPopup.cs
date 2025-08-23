using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using System.ComponentModel;
using System.Diagnostics;

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
        UpdateColumns();
    }

    #region Header

    private Node HeaderIconNode => Node.QuerySelector("#header-icon")!;
    private Node HeaderNameNode => Node.QuerySelector("#header-name")!;
    private Node HeaderInfoNode => Node.QuerySelector("#header-info")!;
    private Node HeaderIlvlNode => Node.QuerySelector("#header-ilvl")!;
    private Node BackgroundNode => Node.QuerySelector(".background")!;
    private Node RandomJobNode => Node.QuerySelector("#RandomJob")!;

    private void UpdateHeaderNodes()
    {
        SetIcon(HeaderIconNode, _headerIconType, CurrentGearset);
        HeaderNameNode.NodeValue = CurrentGearset.Name;
        HeaderInfoNode.NodeValue = GearsetSwitcherInfoDisplayProvider.GetInfoText(GearsetSwitcherInfoDisplayType.JobLevel, CurrentGearset, true);
        HeaderIlvlNode.NodeValue = $"{CurrentGearset.ItemLevel}";
        BackgroundNode.Style.IsVisible = _showGradientBackground;

        switch (CurrentGearset.Category) {
            case GearsetCategory.None:
            case GearsetCategory.Crafter:
            case GearsetCategory.Gatherer:
                RandomJobNode.Style.Opacity = 0.3f;
                RandomJobNode.Tooltip = I18N.Translate("Widget.GearsetSwitcher.RandomJobDisabled");
                break;
            default:
                RandomJobNode.Style.Opacity = 1f;
                RandomJobNode.Tooltip = I18N.Translate("Widget.GearsetSwitcher.RandomJob");
                break;
        }

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

    private void SetIcon(Node node, JobIconType jobIconType, Gearset gearset)
    {
        var jobInfo = Player.GetJobInfo(gearset.JobId);

        switch (jobIconType) {
            case JobIconType.PixelSprites:
                node.Style.IconId = null;
                node.ToggleClass("uld", true);
                node.Style.UldPartId = jobInfo.GetUldIcon(jobIconType);
                switch (gearset.Category) {
                    case GearsetCategory.Crafter:
                    case GearsetCategory.Gatherer:
                        node.Style.UldResource = "ui/uld/WKSScoreList";
                        node.Style.UldPartsId = 2;
                        break;
                    default:
                        node.Style.UldResource = "ui/uld/DeepDungeonScoreList";
                        node.Style.UldPartsId = 3;
                        break;
                }
                break;
            default:
                node.ToggleClass("uld", false);
                node.Style.IconId = jobInfo.Icons[jobIconType];
                node.Style.UldResource = null;
                node.Style.UldPartsId = null;
                node.Style.UldPartId = null;
                break;
        }
    }

    #endregion

    private void UpdateColumns()
    {
        UpdateColumnVisibility(LeftColumnNode);
        UpdateColumnVisibility(MiddleColumnNode);
        UpdateColumnVisibility(RightColumnNode);
    }

    private void UpdateColumnVisibility(Node columnNode)
    {
        bool isColumnVisible = false;

        foreach (var node in GearsetGroupNodes.Values) {
            if (node.ParentNode != columnNode) continue;

            if (node.QuerySelector(".body")!.ChildNodes.Count == 0) {
                node.Style.IsVisible = false;
                continue;
            }

            node.Style.IsVisible = true;
            isColumnVisible = true;
        }

        columnNode.Style.IsVisible = isColumnVisible;
    }
}
