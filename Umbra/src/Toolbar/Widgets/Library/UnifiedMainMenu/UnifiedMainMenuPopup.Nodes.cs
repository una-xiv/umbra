using Dalamud.Game.Text;
using System.Linq;
using Umbra.Game;
using Una.Drawing;

namespace Umbra.Widgets.Library.UnifiedMainMenu;

internal sealed partial class UnifiedMainMenuPopup
{
    private Node CategoryListNode    => Node.QuerySelector("#CategoryList")!;
    private Node CategoriesNode      => Node.QuerySelector("#Categories")!;
    private Node EntriesNode         => Node.QuerySelector("#Entries")!;
    private Node PinnedListNode      => Node.QuerySelector("#PinnedList")!;
    private Node HeaderNode          => Node.QuerySelector("#Header")!;
    private Node HeaderIconNode      => Node.QuerySelector("#HeaderIcon")!;
    private Node HeaderLabelNameNode => Node.QuerySelector("#HeaderLabelName")!;
    private Node HeaderLabelInfoNode => Node.QuerySelector("#HeaderLabelInfo")!;

    protected override Node Node { get; } = new() {
        Id         = "Popup",
        Stylesheet = Stylesheet,
        ChildNodes = [
            new() {
                Id          = "Header",
                InheritTags = true,
                ChildNodes = [
                    new() {
                        Id    = "HeaderIcon",
                        Style = new() { IconId = 76985u }
                    },
                    new() {
                        Id = "HeaderLabel",
                        ChildNodes = [
                            new() { Id = "HeaderLabelName", NodeValue = "Una" },
                            new() { Id = "HeaderLabelInfo", NodeValue = "Lv. 100 Viper" }
                        ]
                    }
                ]
            },
            new() {
                Id          = "Body",
                InheritTags = true,
                ChildNodes = [
                    new() {
                        Id          = "Categories",
                        Overflow    = false,
                        InheritTags = true,
                        ChildNodes = [
                            new() {
                                Id = "CategoriesWrapper",
                                ChildNodes = [
                                    new() { Id = "CategoryList", ClassList = ["categories-list"], SortIndex = 0 },
                                    new() {
                                        ClassList  = ["separator"], SortIndex = 1000,
                                        ChildNodes = [new() { ClassList = ["separator--line"] }]
                                    },
                                    new() { Id = "PinnedList", ClassList = ["categories-list"], SortIndex = 1001 },
                                ]
                            },
                        ]
                    },
                    new() {
                        Id         = "Entries",
                        Overflow   = false,
                        ChildNodes = []
                    }
                ]
            }
        ]
    };

    private static Node CreateCategory(string id, uint iconId, string label)
    {
        return new() {
            Id        = id,
            ClassList = ["category"],
            ChildNodes = [
                new() {
                    ClassList   = ["category--icon"],
                    Style       = new() { IconId = iconId },
                    InheritTags = true,
                },
                new() {
                    ClassList   = ["category--label"],
                    NodeValue   = label,
                    InheritTags = true,
                }
            ]
        };
    }

    private Node CreateEntriesForCategory(MainMenuCategory category)
    {
        return new() {
            Id         = $"Category_{category.Category}",
            ClassList  = ["entries"],
            ChildNodes = [..category.Items.Select(CreateMainMenuEntry)]
        };
    }

    private Node CreateMainMenuEntry(MainMenuItem entry)
    {
        if (entry.Type == MainMenuItemType.Separator) {
            return new() {
                Id         = entry.Id,
                SortIndex  = entry.SortIndex,
                ClassList  = ["entry-separator"],
                ChildNodes = [new() { ClassList = ["entry-separator--line"] }]
            };
        }

        uint?   iconId  = entry.Icon is uint icon ? icon : null;
        string? iconStr = entry.Icon is SeIconChar iconChar ? iconChar.ToIconString() : null;

        Node node = new() {
            Id         = entry.Id,
            ClassList  = ["entry"],
            SortIndex  = entry.SortIndex,
            IsDisabled = entry.IsDisabled,
            ChildNodes = [
                new() {
                    ClassList   = ["entry--icon"],
                    InheritTags = true,
                    NodeValue   = iconStr,
                    Style = new() {
                        IconId = iconId,
                        Color  = entry.IconColor != null ? new(entry.IconColor.Value) : null
                    },
                },
                new() {
                    ClassList   = ["entry--name"],
                    NodeValue   = entry.Name,
                    InheritTags = true,
                },
                new() {
                    ClassList   = ["entry--info"],
                    NodeValue   = entry.ShortKey,
                    InheritTags = true,
                }
            ]
        };

        node.OnMouseUp += _ => {
            entry.Invoke();
            Close();
        };

        node.OnRightClick += _ => {
            SelectedMenuItem = entry;
            ContextMenu?.SetEntryVisible("MoveUp",   false);
            ContextMenu?.SetEntryVisible("MoveDown", false);
            ContextMenu?.SetEntryVisible("Pin",      !PinnedItems.Contains(entry.Id));
            ContextMenu?.SetEntryVisible("Unpin",    PinnedItems.Contains(entry.Id));
            ContextMenu?.Present();
        };

        return node;
    }

    private Node? CreatePinnedItem(string id, int sortIndex)
    {
        MainMenuItem? entry = MainMenuRepository.FindById(id);
        if (entry == null) return null;

        uint?   iconId  = entry.Icon is uint icon ? icon : null;
        string? iconStr = entry.Icon is SeIconChar iconChar ? iconChar.ToIconString() : null;

        Node node = new() {
            Id        = $"Pin_{id}",
            ClassList = ["pinned-entry"],
            SortIndex = sortIndex,
            ChildNodes = [
                new() {
                    ClassList   = ["pinned-entry--icon"],
                    InheritTags = true,
                    NodeValue   = iconStr,
                    Style = new() {
                        IconId = iconId,
                        Color  = entry.IconColor != null ? new(entry.IconColor.Value) : null,
                    }
                },
                new() {
                    ClassList   = ["pinned-entry--name"],
                    NodeValue   = entry.Name,
                    InheritTags = true,
                }
            ]
        };

        node.OnMouseUp += _ => {
            entry.Invoke();
            Close();
        };

        node.OnRightClick += _ => {
            SelectedMenuItem = entry;
            ContextMenu?.SetEntryVisible("MoveUp",   true);
            ContextMenu?.SetEntryVisible("MoveDown", true);
            ContextMenu?.SetEntryVisible("Pin",      false);
            ContextMenu?.SetEntryVisible("Unpin",    true);

            var index = PinnedItems.IndexOf(entry.Id);
            ContextMenu?.SetEntryDisabled("MoveUp",   index == 0);
            ContextMenu?.SetEntryDisabled("MoveDown", index == PinnedItems.Count - 1);

            ContextMenu?.Present();
        };

        return node;
    }
}
