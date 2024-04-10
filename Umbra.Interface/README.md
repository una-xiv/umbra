Umbra Interface Library
=======================

## Abstract

The Umbra Interface Library enables declarative drawing functionality that uses ImGui as a backend. It provides
automatic layout calculation, event handling, and rendering of widgets. The library exposes the so called `Element`
class, which can be used as a standalone container or as a base class for custom UI components. Its API roughly
resembles that of HTML and CSS, but is more limited in scope due to the immediate mode nature of ImGui.

The motivation behind this library is to abstract away the complexity of dynamic layout composition and separation of
concerns between functional and presentational code. For example, you declare interface elements once and then update
their properties based on user input or other factors. The library takes care of the layout and rendering, so you don't
have to worry about the details of how the elements are positioned and sized. This takes a similar concept to the
"declarative UI" approach used in web development, where you define the structure of the UI and let the framework handle
the rendering.

Although the library could be used standalone, it is primarily developed to be used in conjunction with the Umbra
Framework.

---

# Table of Contents

1. [Quick Start](#quick-start)
2. [Element Composition](#element-composition)
3. [Automatic Layout Calculation](#automatic-layout-calculation)
4. [Element Size](#element-size)
5. [Querying Elements](#querying-elements)
6. [Tagged Elements](#tagged-elements)
7. [Styling](#styling)
8. [Animation](#animation)
9. [Interactive Elements](#interactive-elements)
10. [Custom Elements](#custom-elements)
11. [Windows](#windows)

---

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
        _myButton.Render(ImGui.GetForegroundDrawList(), new Vector2(100, 100));
    }
}
```

## Element Composition

Elements are composed of a tree structure, where each element can have children. The layout of the elements is then
determined based on the Anchor and Flow properties of the elements. Elements themselves contain properties that directly
affect their layout, such as size, margin, padding, visibility, and more. The graphical appearance of the elements is
determined by the Style property, which holds a reference to a Style object. The Style object contains properties that
affect the appearance of the element, such as color, font, outline, background, and more. Modifications to the Style
object typically don't trigger a "reflow" of the element, meaning that the layout is not recalculated when the Style
object is modified. This is useful for creating dynamic UIs where the layout is fixed, but the appearance can change
dynamically based on user interaction or other factors.

The following example demonstrates a list of boxes of 32x32 pixels that are stacked vertically with a gap of 8px between
them:

```csharp
Size  BoxSize  = new(32, 32);
Style BoxStyle = new() { BackgroundColor = 0xFF0000FF };

Element listOfBoxes = new(
    id:     "listOfBoxes",
    anchor: Anchor.TopLeft,
    flow:   Flow.Vertical,
    gap:    8,
    children: [
        new(id: "a", size: BoxSize, style: BoxStyle),
        new(id: "b", size: BoxSize, style: BoxStyle),
        new(id: "c", size: BoxSize, style: BoxStyle),
    ]
);

// Becaus the anchor point is set to TopLeft, the top-left point of the element
// is positioned at 100, 100.
Element.Render(ImGui.GetForegroundDrawList(), new Vector2(100, 100));
```

To turn this list into a horizontal row of boxes, all you need to do is change the `Flow` property of the parent element
from `Flow.Vertical` to `Flow.Horizontal`.

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

> [!WARNING]
> Due to performance considerations, the layout calculation is done in a single depth-first pass. This means that the
> size of an element is calculated based on the size of its children, and the size of those children is calculated based
> on their children, and so on. The size of an element is not known until all its children have been processed
> recursively. Because of this, having multiple Fit/Stretch elements in a tree may lead to unexpected results. Try to
> use fixed sizes as much as possible.

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

## Tagged Elements

Elements can be tagged with a string value. This is useful for grouping elements together or for querying elements
based on a specific tag. An element can hold a single tag for performance reasons. The tag can be set using the `Tag`
property of the element. You can use the `FindByTag` method to query for elements that have the specified tag.

```csharp
// Hide all elements tagged with "buttons" in the tree.
element.FindByTag("buttons", recursive: true).ForEach(button => button.IsVisible = false);
```

> [!TIP]
> For performance reasons, the `FindByTag` method returns a list of elements that match the tag from the current element
> by default. If you want to query for elements in the entire tree, you can set the `recursive` parameter to `true`.

## Styling

The `Element` class has several properties to style the element. Everything that determines the graphical appearance of
the element can be found in the `.Style` property that holds a reference to a `Style` object.

Rendering of styled elements is done in multiple passes, depending on which properties have values. The render order is
defined as follows:

1. Shadow
2. Background (Solid and gradients)
3. Image (Icons and textures)
4. Borders (BackgroundBorder and individual edges)
5. Text

After the rendering of the element is done, the children are rendered using the same process. The order of the children
can be controlled using the `SortIndex` property of the child element.

You find see the implementation of this in the `Element.Renderer.cs` file. The rendering implementations themselves are
separated in in several files located in the `Element/Rendering` directory.

> [!TIP]
> Style properties that determine how **text** is rendered are inherited from the parent element. This means that if you
> set a font, color, or outline at the root element, it will be applied to all children unless they override it. Other
> properties, such as background color, gradients, images, etc. are not inherited because they are considered to be more
> specific to a single element rather than a group of elements.

## Animation

The `Element` class has built-in support for animating properties. This is done by using the `Animate` method, which
accepts an instance of the `Animation` class. The `Animation` class consists of animatable properties, such as `Offset`,
`Size`, `Padding`, `Margin`, any color or gradient style properties and more.

The `Animation` class takes in an easing function as a generic type parameter and a duration in milliseconds. The easing
function determines how the animation progresses over time. The library relies on the easing function from Dalamud but
uses custom wrappers to allow them to be used as typed parameters for a cleaner API.

The properties in the `Animation` class define the "end goal" of the element. This means that if your element currently
has an offset of `(0, 0)` and the animation class defines an offset of `(250, 0)`, the element will animate
from `(0, 0)`
to `(250, 0)` over the specified duration and be interpolated based on the configured easing function.

```csharp
using Umbra.Interface;

// Create a simple 32x32 red block.
Element element = new(
    id     : "MyElement",
    size   : new (32, 32),
    offset : new (0, 0),
    style  : new Style { BackgroundColor = 0xFF0000FF }
);

// Animate the element to move 250 pixels to the right over 500 milliseconds.
element.Animate(new Animation<OutElastic>(500) {
    Offset = new (250, 0)
});
```

As soon as you start rendering the element, the animation will be updated automatically. The animation will be stopped
once it reaches its end goal. If you wish to repeat the animation, you can set the "once" parameter to `false` in the
`Animate` method, which is `true` by default.

> [!TIP]
> Some properties, such as padding and margin in particular may not always work due to the configured anchors and flows.
> If your goal is to simply move an element from one place to another, you should use the `Offset` property instead,
> which always triggers a layout reflow.

## Interactive Elements

Elements can be made interactive by attaching event listeners to them. The following events are available
out-of-the-box:

| Event                 | Description                                                    |
|-----------------------|----------------------------------------------------------------|
| `OnClick`             | Invoked when the element is clicked.                           |
| `OnMiddleClick`       | Invoked when the element is middle-clicked.                    |
| `OnRightClick`        | Invoked when the element is right-clicked.                     |
| `OnMouseEnter`        | Invoked when the mouse enters the element.                     |
| `OnDelayedMouseEnter` | Invoked after a short delay when the mouse enters the element. |
| `OnMouseLeave`        | Invoked when the mouse leaves the element.                     |
| `OnMouseDown`         | Invoked when a mouse button is pressed down.                   |
| `OnMouseUp`           | Invoked when a mouse button is released.                       |

An element may also define a tooltip string using the `Tooltip` property.

Once an element is interactive, an invisible ImGui button is created that covers the entire area of the element. This
button is used to detect mouse events.

> [!TIP]
> Event listeners can be disabled by setting the `IsDisabled` property on the element to `true`. This is useful if you
> have a button that should not be clickable under certain conditions.

## Custom Elements

The interface library comes with a couple of built-in prefab elements, such as buttons, dropdown elements and decorative
elements. Implementing a custom element is typically done by extending the `Element` class and implementing one or more
of the following methods:

### `BeforeCompute`

The `BeforeCompute` method is called before the layout calculation of the element. This is useful for setting up
properties that affect the layout, such as padding, margin, or size. This method is called once per frame, so it is
important to not perform any heavy calculations in this method.

### `AfterCompute`

The `AfterCompute` method is called after the layout calculation of the element. This is useful for setting up
properties that affect the rendering, such as colors, fonts, or images. This method is called once per frame, so it is
important to not perform any heavy calculations in this method.

### `Draw`

The `Draw` method is called when the element is being rendered, but after the rendering based on `Style` properties has
been completed. You have access to the `DrawList` object, which is an instance of the ImGui draw list. This object can
then be used to draw custom shapes, text, or images.

### `BeginDraw`

If your element needs to override the current `DrawList`, you can implement the `BeginDraw` method which is invoked
immediately before rendering any child elements and the `Draw` method. This is useful for creating custom clipping
regions or scrolling containers. Use the protected `PushDrawList` method to push a new draw list onto the stack.

> [!CAUTION]
> The `BeginDraw` method is only called if the element is drawn onto an ImGui Window. If you are using the Foreground
> or Background draw lists, this method will not be called.

> [!TIP]
> Have a look at the `OverflowContainer` prefab element for an example of how to implement a scrolling container.

### `EndDraw`

If you have overridden the `BeginDraw` method, you should also implement the `EndDraw` method to pop the draw list from
the stack and close any open ImGui windows or groups.

> [!CAUTION]
> The `EndDraw` method is only called if the element is drawn onto an ImGui Window. If you are using the Foreground
> or Background draw lists, this method will not be called. It is also imperative that you pop any pushed ImGui fonts,
> vars and colors from the stack that you may have pushed in the `BeginDraw` method. Failing to do so will result in
> ImGui crashes.

## Windows

The interface library provides an abstract `Window` class that can be used to declare a window with a title bar and
close button. A class that inherits from `Window` can be created using the `WindowManager.CreateWindow` method.

```csharp
using Umbra.Interface;

class MyWindow : Window
{
    public MyWindow()
    {
        Title       = "My Awesome Window";
        DefaultSize = new(800, 600);
        MinSize     = new(650, 480);
        MaxSize     = new(1200, 900);
    }
    
    protected override void OnDraw(int instanceId)
    {
        // Draw your window contents to `ImGui.GetWindowDrawList()` here.
    }
}
```

```csharp
[Service]
class MyClass(WindowManager wm)
{
    public void OpenWindow()
    {
        wm.CreateWindow<MyWindow>();
    }    
}
```

The `OnDraw` method is invoked on every frame for as long as the window is open. The `instanceId` parameter is a unique
identifier that is used for window clipping behind native game windows. Any ImGui groups or child windows that you
create should be unique based on this identifier.

> [!TIP]
> If you implement the `Dispose` method in your window class, it will be called when the window is closed. This is
> useful for cleaning up resources or event listeners that you may have set up.

Disposing a window can be done in two ways:

1. From inside the Window class by invoking the `.Close()` method.
2. From outside the Window by invoking `WindowManager.CloseWindow<T>()` where `T` is the type of the window.

Only one window of a specific type can be open at a time. If you try to open a window that is already open, the existing
window will be disposed of and reopened instead.
