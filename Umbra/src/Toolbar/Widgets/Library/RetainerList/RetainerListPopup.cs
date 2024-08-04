using Dalamud.Game.Text;
using Lumina.Misc;
using System;
using System.Collections.Generic;
using System.Globalization;
using Umbra.Common;
using Umbra.Game;
using Umbra.Game.Retainer;
using Una.Drawing;

namespace Umbra.Widgets.Library.RetainerList;

internal partial class RetainerListPopup : WidgetPopup
{
    public bool   ShowGil           { get; set; } = true;
    public bool   ShowInventory     { get; set; } = true;
    public bool   ShowItemsOnSale   { get; set; } = true;
    public bool   ShowVenture       { get; set; } = true;
    public string CurrencySeparator { get; set; } = ".";
    public string JobIconType       { get; set; } = "Default";

    protected override Node Node { get; } = new() {
        Id         = "Popup",
        Stylesheet = Stylesheet,
        ChildNodes = [
            new() {
                ClassList = ["row", "header"],
                ChildNodes = [
                    new() {
                        ClassList = ["cell", "job-icon"],
                    },
                    new() {
                        ClassList = ["cell", "job-level"],
                        NodeValue = I18N.Translate("Widget.RetainerList.Popup.Job"),
                    },
                    new() {
                        ClassList = ["cell", "name"],
                        NodeValue = I18N.Translate("Widget.RetainerList.Popup.Name"),
                    },
                    new() {
                        ClassList = ["cell", "gil"],
                        NodeValue = I18N.Translate("Widget.RetainerList.Popup.Gil"),
                    },
                    new() {
                        ClassList = ["cell", "items", "inv"],
                        NodeValue = I18N.Translate("Widget.RetainerList.Popup.Inventory"),
                    },
                    new() {
                        ClassList = ["cell", "items", "market"],
                        NodeValue = I18N.Translate("Widget.RetainerList.Popup.Market"),
                    },
                    new() {
                        ClassList = ["cell", "venture-time"],
                        NodeValue = I18N.Translate("Widget.RetainerList.Popup.Venture"),
                    }
                ]
            }
        ]
    };

    private IRetainerRepository Repository { get; } = Framework.Service<IRetainerRepository>();

    private readonly Dictionary<string, Retainer> _retainers = [];

    protected override bool CanOpen()
    {
        IPlayer player = Framework.Service<IPlayer>();

        // You are prohibited from retrieving retainer info while in an instanced duty
        // or if you're currently visiting a different world.
        return player.CurrentWorldName == player.HomeWorldName && !player.IsBoundByInstancedDuty;
    }

    protected override void OnOpen()
    {
        Repository.RequestRetainerList();
    }

    protected override void OnUpdate()
    {
        foreach (var retainer in Repository.GetRetainers()) {
            if (_retainers.TryAdd(retainer.Name, retainer)) {
                BuildRetainerNodeFor(retainer);
                continue;
            }

            UpdateRetainerNodeFor(retainer);
        }

        foreach (Node n in Node.QuerySelectorAll(".gil")) n.Style.IsVisible          = ShowGil;
        foreach (Node n in Node.QuerySelectorAll(".items.inv")) n.Style.IsVisible    = ShowInventory;
        foreach (Node n in Node.QuerySelectorAll(".items.market")) n.Style.IsVisible = ShowItemsOnSale;
        foreach (Node n in Node.QuerySelectorAll(".venture-time")) n.Style.IsVisible = ShowVenture;
    }

    private void BuildRetainerNodeFor(Retainer retainer)
    {
        Node node = new() {
            Id        = GetNodeIdOf(retainer),
            ClassList = ["row"],
            ChildNodes = [
                new() {
                    ClassList = ["cell", "job-icon"],
                    Style     = new() { IconId = retainer.Job?.GetIcon(JobIconType) }
                },
                new() {
                    ClassList = ["cell", "job-level"],
                    NodeValue = retainer.Job?.Level.ToString()
                },
                new() {
                    ClassList = ["cell", "name"],
                    NodeValue = retainer.Name
                },
                new() {
                    ClassList = ["cell", "gil"],
                    NodeValue = retainer.Gil
                },
                new() {
                    ClassList = ["cell", "items", "inv"],
                    NodeValue = retainer.ItemCount.ToString()
                },
                new() {
                    ClassList = ["cell", "items", "market"],
                    NodeValue = retainer.MarketItemCount.ToString()
                },
                new() {
                    ClassList = ["cell", "venture-time"],
                    NodeValue = GetRemainingTime(retainer.VentureCompleteTime),
                }
            ]
        };

        Node.AppendChild(node);
    }

    private void UpdateRetainerNodeFor(Retainer retainer)
    {
        Node? node = Node.FindById(GetNodeIdOf(retainer));
        if (null == node) return;

        node.QuerySelector(".cell.job-icon")!.Style.IconId = retainer.Job?.GetIcon(JobIconType);
        node.QuerySelector(".cell.job-level")!.NodeValue = retainer.Job?.Level.ToString();
        node.QuerySelector(".cell.name")!.NodeValue = retainer.Name;
        node.QuerySelector(".cell.gil")!.NodeValue = $"{SeIconChar.Gil.ToIconString()} {FormatNumber(retainer.Gil)}";
        node.QuerySelector(".cell.items.inv")!.NodeValue = retainer.ItemCount.ToString();
        node.QuerySelector(".cell.items.market")!.NodeValue = retainer.MarketItemCount.ToString();
        node.QuerySelector(".cell.venture-time")!.NodeValue = GetRemainingTime(retainer.VentureCompleteTime);
    }

    private static string GetRemainingTime(DateTime? targetTime)
    {
        if (null == targetTime) {
            return "-";
        }

        DateTime localTargetTime = targetTime.Value.ToLocalTime();
        DateTime now             = DateTime.Now;

        if (localTargetTime <= now) {
            return I18N.Translate("Widget.RetainerList.Popup.Completed");
        }

        TimeSpan remainingTime = localTargetTime - now;

        return $"{remainingTime.Hours:D2}:{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}";
    }

    private static string GetNodeIdOf(Retainer retainer)
    {
        return $"Retainer_{Crc32.Get(retainer.Name)}";
    }

    public string FormatNumber(uint value)
    {
        return value.ToString(
            "N0",
            new NumberFormatInfo {
                NumberGroupSeparator = CurrencySeparator,
                NumberGroupSizes     = [3]
            }
        );
    }
}
