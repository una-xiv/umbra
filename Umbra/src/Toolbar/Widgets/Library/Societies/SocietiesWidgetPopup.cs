using Dalamud.Plugin.Services;
using System;
using System.Linq;
using Umbra.Common;
using Umbra.Game;
using Umbra.Game.Societies;
using Una.Drawing;

namespace Umbra.Widgets.Library.Societies;

internal sealed partial class SocietiesWidgetPopup : WidgetPopup
{
    public uint   TrackedSocietyId             { get; set; }
    public int    MinItemsBeforeHorizontalView { get; set; } = 10;
    public string PrimaryAction                { get; set; } = "Teleport";

    public event Action<Society?>? OnSocietySelected;

    private IDataManager         DataManager { get; } = Framework.Service<IDataManager>();
    private IPlayer              Player      { get; } = Framework.Service<IPlayer>();
    private ISocietiesRepository Repository  { get; } = Framework.Service<ISocietiesRepository>();

    private uint? _selectedSocietyId;

    public SocietiesWidgetPopup()
    {
        ContextMenu = new(
            [
                new("Track") {
                    Label  = I18N.Translate("Widget.Societies.ContextMenu.Track"),
                    OnClick = () => {
                        if (null != _selectedSocietyId) OnSocietySelected?.Invoke(Repository.Societies[_selectedSocietyId.Value]);
                    },
                },
                new("Untrack") {
                    Label  = I18N.Translate("Widget.Societies.ContextMenu.Untrack"),
                    OnClick = () => {
                        if (null != _selectedSocietyId) OnSocietySelected?.Invoke(null);
                    },
                },
                new("Teleport") {
                    Label  = I18N.Translate("Widget.Societies.ContextMenu.Teleport"),
                    IconId = 60453u,
                    OnClick = () => {
                        if (null != _selectedSocietyId) Repository.TeleportToAetheryte(_selectedSocietyId.Value);
                    },
                }
            ]
        );
    }

    protected override void OnOpen()
    {
        var itemCount = 0;

        Node.FindById("AllowanceStatus")!.NodeValue = I18N.Translate(
            "Widget.CustomDeliveries.AllowanceStatus",
            12 - Repository.WeeklyAllowance
        );

        foreach (Society society in Player.Societies.OrderBy(s => s.ExpansionId)) {
            if (society.RankId == 0) continue;

            Node expansionNode = GetOrCreateExpansionNodeFor(society.ExpansionId, society.ExpansionName);
            Node societyNode   = GetOrCreateSocietyNode(society, expansionNode);

            const int barWidth = 154;
            int expWidth = society.RequiredRep > 0 ? (barWidth * society.CurrentRep / society.RequiredRep) : barWidth;
            int expPct = society.RequiredRep > 0 ? (100 * society.CurrentRep / society.RequiredRep) : 100;

            societyNode.QuerySelector(".society--exp-bar--bar")!.Style.Size = new(expWidth, 2);
            societyNode.QuerySelector(".society--rank--value")!.NodeValue   = $"{expPct}%";
            societyNode.QuerySelector(".society--rank")!.NodeValue          = society.Rank;

            societyNode.QuerySelector(".society--currency--value")!.NodeValue =
                $"{Player.GetItemCount(society.CurrencyItemId)}";

            itemCount++;
        }

        if (itemCount < MinItemsBeforeHorizontalView) {
            Node.FindById("List")!.TagsList.Add("vertical");
        } else {
            Node.FindById("List")!.TagsList.Remove("vertical");
        }
    }
}
