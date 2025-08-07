using Lumina.Excel.Sheets;

namespace Umbra.Widgets;

internal partial class CompanionPopup
{
    protected override Node Node { get; } = UmbraDrawing.DocumentFrom("umbra.widgets.popup_companion.xml").RootNode!;

    private void CreateStanceButton(uint actionId)
    {
        Node node = new() {
            Id        = $"Stance_{actionId}",
            ClassList = ["button", "usable"],
            Tooltip   = Companion.GetStanceName(actionId),
            ChildNodes = [
                new() {
                    ClassList = ["icon"],
                    Style = new() {
                        IconId = Companion.GetStanceIcon(actionId),
                    }
                }
            ],
        };

        node.OnClick += _ => Companion.SetStance(actionId);

        node.BeforeDraw += n => {
            node.QuerySelector(".icon")!.Style.ImageGrayscale = !n.IsMouseOver && (!Companion.CanSetStance(actionId) || actionId != Companion.ActiveCommand);
            node.Style.Opacity = Companion.CanSetStance(actionId) ? 1 : 0.33f;
        };

        Node.QuerySelector("#StanceButtons")!.AppendChild(node);
    }

    private void CreateFoodButton(CompanionFood foodType)
    {
        Item? item = DataManager.GetExcelSheet<Item>().FindRow((uint)foodType);
        if (item == null) return;

        Node node = new() {
            Id        = $"Food_{foodType}",
            ClassList = ["button"],
            Tooltip   = $"{item.Value.Name.ExtractText()}\n\n{item.Value.Description.ExtractText().Split("\n").LastOrDefault() ?? ""}",
            ChildNodes = [
                new() {
                    ClassList = ["icon"],
                    Style     = new() { IconId = item.Value.Icon }
                },
                new() {
                    ClassList = ["count"],
                }
            ],
        };

        node.OnMouseUp += _ => {
            if (Companion.HasCompanionFood(foodType)) Companion.UseCompanionFood(foodType);
        };

        Node.QuerySelector("#FoodButtons")!.AppendChild(node);
    }
}
