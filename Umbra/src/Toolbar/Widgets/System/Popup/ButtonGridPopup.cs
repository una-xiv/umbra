using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Umbra.Style;
using Una.Drawing;

namespace Umbra.Widgets;

public abstract class ButtonGridPopup : WidgetPopup
{
    private const int SlotSize = 48;
    private const int GapSize  = 6;

    public event Action<byte>? OnCategoryChanged;

    protected sealed override Node Node { get; } = new() {
        Id         = "Popup",
        Stylesheet = PopupStyles.ButtonGridStylesheet,
        ChildNodes = [new() { Id = "CategoryBar" }],
    };

    protected void ReconfigurePanel(string[] categoryNames, byte numRows, byte numCols)
    {
        foreach (var node in Node.ChildNodes.ToArray()) node.Remove();

        categoryNames = categoryNames.Where(name => !string.IsNullOrEmpty(name.Trim())).ToArray();
        int numCategories = categoryNames.Length;

        Node categoryBarNode = new() { Id = "CategoryBar" };
        Node.AppendChild(categoryBarNode);

        int width  = (numCols * (SlotSize + GapSize)) + 8;
        int height = (numRows * (SlotSize + GapSize)) + 24 + 16;

        Node.Style.Size = new(width, height);
        categoryBarNode.Style.Size = new(Node.Style.Size.Width - 16, 24);

        for (byte i = 0; i < categoryNames.Length; i++) {
            categoryBarNode.ChildNodes.Add(CreateCategoryButton(i, i == 0, categoryNames[i], width, numCategories));
            Node.ChildNodes.Add(CreateSlotContainer(i, i == 0, numRows, numCols));
        }
    }

    private Node CreateCategoryButton(byte id, bool isActive, string name, int width, int numCategories)
    {
        Node node = new() {
            Id        = $"CategoryButton_{id}",
            ClassList = ["category-button"],
            NodeValue = name,
            TagsList  = isActive ? ["selected"] : [],
            Style     = new() { IsVisible = true },
            BeforeDraw = node => {
                node.Style.Size = new((width - 14) / numCategories, 24);
            }
        };

        node.OnMouseUp += _ => ActivateCategory(id);

        return node;
    }

    private Node CreateSlotContainer(byte id, bool isVisible = false, byte numRows = 4, byte numCols = 8)
    {
        ObservableCollection<Node> rows = [];

        for (byte i = 0; i < numRows; i++) {
            rows.Add(CreateSlotRow(id, i, numCols));
        }

        return new() {
            Id        = $"SlotContainer_{id}",
            ClassList = ["slot-container"],
            Style = new() {
                IsVisible = isVisible,
                Size      = new(numCols * (SlotSize + GapSize), numRows * (SlotSize + GapSize)),
            },
            ChildNodes = rows,
        };
    }

    private Node CreateSlotRow(byte categoryId, byte rowId, byte numCols = 8)
    {
        ObservableCollection<Node> buttons = [];

        for (byte i = 0; i < numCols; i++) {
            buttons.Add(CreateSlotButton(categoryId, rowId, numCols, i));
        }

        return new() {
            Id         = $"SlotRow_{categoryId}_{rowId}",
            ClassList  = ["slot-row"],
            ChildNodes = buttons
        };
    }

    private Node CreateSlotButton(byte categoryId, byte rowId, byte numCols, byte index)
    {
        var id = (rowId * numCols) + index;

        Node node = new() {
            Id        = $"Slot_{categoryId}_{id}",
            ClassList = ["slot-button"],
            TagsList  = ["empty"],
            ChildNodes = [
                new() { ClassList = ["slot-button--icon"], InheritTags     = true},
                new() { ClassList = ["slot-button--sub-icon"], InheritTags = true },
                new() { ClassList = ["slot-button--count"], InheritTags    = true }
            ]
        };

        OnButtonSlotCreated(node, categoryId, id);

        return node;
    }

    protected abstract void OnButtonSlotCreated(Node node, byte categoryId, int slotId);

    protected Node GetCategoryButton(byte id) => Node.QuerySelector($"#CategoryButton_{id}")!;
    protected Node GetSlotContainer(byte  id) => Node.FindById($"SlotContainer_{id}")!;
    protected int  NumCategories              => Node.QuerySelectorAll(".category-button").Count();

    protected void ActivateCategory(byte id)
    {
        if (NumCategories == 1) return;

        if (null == Node.FindById($"SlotContainer_{id}")) {
            List<Node> containers = Node.QuerySelectorAll(".slot-container").ToList();
            if (containers.Count == 0) return;

            id = byte.Parse(containers[0].Id!.Split('_')[1]);
        }

        Node selectedCategoryButton = GetCategoryButton(id);
        Node selectedSlotContainer  = GetSlotContainer(id);

        if (selectedCategoryButton.TagsList.Contains("selected")) return;

        selectedCategoryButton.TagsList.Add("selected");
        selectedSlotContainer.Style.IsVisible = true;

        foreach (var node in Node.QuerySelectorAll(".category-button")) {
            if (node != selectedCategoryButton) node.TagsList.Remove("selected");
        }

        foreach (var node in Node.QuerySelectorAll(".slot-container")) {
            if (node != selectedSlotContainer) node.Style.IsVisible = false;
        }

        OnCategoryChanged?.Invoke(id);
    }
}
