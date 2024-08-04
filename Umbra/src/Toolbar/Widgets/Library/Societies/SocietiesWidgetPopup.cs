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
    public int MinItemsBeforeHorizontalView { get; set; } = 10;

    public event Action<Society>? OnSocietySelected;

    private IDataManager DataManager { get; } = Framework.Service<IDataManager>();
    private IPlayer      Player      { get; } = Framework.Service<IPlayer>();

    protected override void OnOpen()
    {
        var itemCount = 0;

        foreach (Society society in Player.Societies.OrderBy(s => s.ExpansionId)) {
            if (society.RankId == 0) continue;

            Node expansionNode = GetOrCreateExpansionNodeFor(society.ExpansionId, society.ExpansionName);
            Node societyNode   = GetOrCreateSocietyNode(society, expansionNode);

            const int barWidth = 154;
            int       expWidth = society.RequiredRep > 0 ? (barWidth * society.CurrentRep / society.RequiredRep) : barWidth;
            int       expPct   = society.RequiredRep > 0 ? (100 * society.CurrentRep / society.RequiredRep) : 100;

            societyNode.QuerySelector(".society--exp-bar--bar")!.Style.Size = new(expWidth, 2);
            societyNode.QuerySelector(".society--rank--value")!.NodeValue   = $"{expPct}%";
            societyNode.QuerySelector(".society--rank")!.NodeValue          = society.Rank;

            societyNode.QuerySelector(".society--currency--value")!.NodeValue =
                $"{Player.GetItemCount(society.CurrencyItemId)}";

            itemCount++;
        }

        if (itemCount < MinItemsBeforeHorizontalView) {
            Node.TagsList.Add("vertical");
        } else {
            Node.TagsList.Remove("vertical");
        }

        // foreach (var society in Player.Societies) {
        //     // Skip societies with no progress to avoid giving spoilers.
        //     // if (society.RankId == 0) continue;
        //
        //     Node expansionNode = GetOrCreateExpansionNodeFor(society.ExpansionId, society.ExpansionName);
        //     expansionNode.AppendChild(CreateSocietyNode(society));
        // }
    }
}
