```
    ____ ___        ___.
   |    |   \ _____ \_ |__ _______ _____       ___     _            __            
   |    |   //     \ | __ \\_  __ \\__  \     |_ _|_ _| |_ ___ _ _ / _|__ _ __ ___
   |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_   | || ' |  _/ -_| '_|  _/ _` / _/ -_)
   |______//__|_|  /____  /|__|   (____  /    |___|_||_\__\___|_| |_| \__,_\__\___|
                 \/     \/             \/
```

Umbra Interface Library
=======================

## Abstract

The Umbra Interface Library enables declarative drawing functionality that uses ImGui as a backend. It provides
automatic layout calculation, event handling, and rendering of widgets. The library exposes the so called `Element`
class, which can be used as a standalone container or as a base class for custom UI components. Its API roughly
resembles that of HTML and CSS, but is more limited in scope due to the immediate mode nature of ImGui.

Although the library could be used standalone, it is primarily developed to be used in conjunction with the Umbra
Framework.

## Quick Start

To create a simple button element that prints a message when clicked, you can use the following code:

```csharp
using Umbra.Common;
using Umbra.Interface;

[Service]
class MyClass
{
    private Element _myButton = new(
        id: "myButton",
        anchor: Anchor.TopLeft,
        flow: Flow.Horizontal,
        size: (100, 50),
        text: "Click me!"
    );
    
    public MyClass()
    {
        _myButton.OnClick += () => Logger.Info("Button clicked!");
    }
    
    [OnDraw]
    public void OnDraw()
    {
        _myButton.Render(ImGui.GetWindowDrawList(), new Vector2(100, 100));
    }
}
```

## Automatic Layout Calculation

Elements are automatically positioned and sized based on their properties, siblings and parent element. The layout is
calculated *once* for optimal performance, and whenever properties that affect the layout are changed.

Positioning is based on the `Anchor` property of the element itself and the `Flow` property of the parent element. An
element can have a fixed size when the `Size` property contains values for both width and height. Otherwise, the size
is calculated based on the size of the children or text content. Element positions are calculated on a
per-anchor-per-flow basis. This means that two elements with the same anchor will be placed next to each other in a
horizontal flow. If the flow is vertical, they will be placed below each other. A bottom or right anchor will reverse
the order of the elements. When the Flow property is set to `Flow.None`, children are simply stacked on top of each
other. This is useful for creating more advanced graphical elements. When a child element has an anchor of `Anchor.None`
and does not specify a fixed size, it will have its size calculated based on its parent element. This is useful for
adding background elements to a container, for example.

## Element Size

The size of an element is calculated based on the size of its children or text content. If the `Size` property is set,
the element will have a fixed size. If the `Size` property is not set, the size will be calculated based on the size of
the children or text content. If either the width or height is `0`, it will be calculated based on the content. This
means that an element may have a fixed width, but a flexible height or vice versa.

An element may have a `margin` and `padding` configured. The margin simply expands the size of the element, regardless
if it has a fixed size or not. The padding determines the space between the edges of the element and its _own content_.
With "own content" we mean the graphical properties that belong to the element itself, such as styling or text. The
children of an element are deliberately not affected by padding to give the developer more flexibility in styling.

### Fitting elements to match their siblings.

There are two types of fitting mechanisms that can be used to adjust the size of elements based on their siblings or
parent element.

#### The `Fit` property

When `Fit` is set to true, the element will adjust its size to match the size of its siblings based on the flow of the
parent element. This is useful when you want to create a button bar, for example. If the flow is horizontal, the height
of the element will be adjusted to match the tallest sibling. If the flow is vertical, the width of the element will be
adjusted to match the widest sibling.

#### The `Stretch` property

When `Stretch` is set to true, the element will try to fill the available (remaining) space of the parent element. This
is useful when you want to create a flexible layout where one element takes up the remaining space. The `Stretch`
property may only appear once in an element. The parent element is expected to have a fixed width or height, depending
on its flow direction.

> **Warning**: Due to performance considerations, the layout calculation is done in a single depth-first pass. This
> means that the size of an element is calculated based on the size of its children, and the size of those children is
> calculated based on their children, and so on. The size of an element is not known until all its children have been
> processed recursively. Because of this, having multiple Fit/Stretch elements in a tree may lead to unexpected results.
> Try to use fixed sizes as much as possible.

## Querying Elements

Each element should have an ID and must be unique within the scope of a single element. This ID can then be used to
find a reference to a specific element. The `Element` class has several methods to query for elements:

- `Element? Find(string id)` - Finds an element by its ID, returns NULL if no such element exists.
- `Element Get(string id)` - Finds an element by its ID, throws an exception if no such element exists.
- `bool Has(string id)` - Checks if an element with the given ID exists.

The ID of an element can be concatenated with a dot to query for a child element. For example, if you have a deeply
nested element that represents some kind of text element, you can query for it like this:

```csharp
var textElement = element.Get("child1.child2.myTextElement");
textElement.Text = "Hello, World!";
```

## Events

Element become interactive once one or more event listeners are attached. The following events are supported:

- `OnClick` - Invoked when the element is clicked with the left mouse button.
- `OnMiddleClick` - Invoked when the element is clicked with the middle mouse button.
- `OnRightClick` - Invoked when the element is clicked with the right mouse button.
- `OnMouseEnter` - Invoked when the mouse enters the element.
- `OnMouseLeave` - Invoked when the mouse leaves the element.

## Styling

The `Element` class has several properties to style the element. Everything that determines the graphical appearance of
the element can be found in the `.Style` property that holds a reference to a `Style` object.

Any property that affects text rendering can be inherited from parent elements, meaning you may choose to define a
font, color, outline, etc. at the root element and have it applied to all children, unless they override it. Properties
that affect the background color, gradients, image processing, etc. are not inherited because they are considered to be
more specific to a single element rather than a group of elements.
