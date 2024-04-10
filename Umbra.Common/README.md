Umbra Common
============

This is a "framework" library that provides a set of architectural components and utilities that glues other parts of
the plugin together. It also provides lifecycle management for initializing and disposing of the plugin.

The core features of this library are:

 - A lightweight Dependency Injection framework that supports collections.
 - A scheduler for running tasks on the game thread, as well as drawing to the screen.
 - A way to access all linked Assemblies from the plugin.
 - A way to invoke static methods during plugin load and unload.
 - A way to manage configuration variables that are declared using attributes on static properties.
 - A simple translation (`I18N`) system using JSON files.
 - A global `Logger` that proxies log lines to custom log targets.

## Table of Contents

  1. [Registering Assemblies](#registering-assemblies)
  2. [Dependency Injection](#dependency-injection)
     1. [Collections](#collections)
     2. [Compiling the container](#compiling-the-container)
     3. [Fetching services from the `Framework`](#fetching-services-from-the-framework)
     4. [Instantiating a class with dependencies](#instantiating-a-class-with-dependencies)
  3. [Logging](#logging)
  4. [Lifecycle Attributes](#lifecycle-attributes)
     1. [`[WhenFrameworkCompiling]`](#whenframeworkcompiling)
     2. [`[WhenFrameworkAsyncCompiling]`](#whenframeworkasynccompiling)
     3. [`[WhenFrameworkDisposing]`](#whenframeworkdisposing)
  5. [Scheduler](#scheduler)
  6. [Configuration](#configuration) 

## Registering assemblies

The static `Umbra.Common.Framework` class acts as a central access point for anything that is directly related to the common
systems of the plugin. One of the first things you should do is register all assemblies that are linked to the plugin so
that the dependency injection system has the ability to scan all assemblies for types that should be registered as
services.

```csharp
using Umbra.Common;

public class MyPlugin : IDalamudPlugin
{
    public MyPlugin()
    {
        Framework.RegisterAssembly(Assembly.GetExecutingAssembly());
    }
}
```

## Dependency Injection

Classes marked with the `[Service]` attribute are automatically registered with the dependency injection system during
the compilation stage. Instances of these classes can be injected by adding their types to arguments of constructors of
other services.

In the following example, we declare two services, `MyService` and `MyOtherService`. `MyOtherService` has a dependency
on `MyService`, so the dependency injection system will automatically inject an instance of `MyService` when creating an
instance of `MyOtherService`.

```csharp
[Service]
public class MyService
{
    public void DoSomething()
    {
        Console.WriteLine("Hello, world!");
    }
}

[Service]
public class MyOtherService(MyService myService)
{
    public void DoSomethingElse()
    {
        myService.DoSomething();
    }
}
```

### Collections

The dependency injection system also supports injecting collections of services. This is useful when you have multiple
implementations of an interface and you want to iterate over all of them.

In the following example, we declare two services that implement the `IMyInterface` interface. We then declare a third
service that has a dependency on a collection of `IMyInterface` instances.

```csharp
public interface IMyInterface
{
    void DoSomething();
}

[Service]
public class MyService1 : IMyInterface
{
    public void DoSomething()
    {
        Console.WriteLine("Hello, world!");
    }
}

[Service]
public class MyService2 : IMyInterface
{
    public void DoSomething()
    {
        Console.WriteLine("Goodbye, world!");
    }
}

[Service]
public class MyOtherService(IMyInterface[] myServices)
{
    public void DoSomethingElse()
    {
        foreach (var myService in myServices)
            myService.DoSomething();
    }
}
```

### Compiling the container

After you've registered all assemblies, it's time to compile the container. This will instantiate all services while
testing for circular dependencies.

```csharp
public class MyPlugin : IDalamudPlugin
{
    [PluginService]
    private readonly IFramework _framework { get; init; } = null!;
    
    public MyPlugin(DalamudPluginInterface pluginInterface)
    {
        // Register all assemblies.
        Framework.RegisterAssembly(Assembly.GetExecutingAssembly());
        // ... add more assemblies if needed.
        
        // Compile the container. It needs a instance of IFramework and DalamudPluginInterface.
        Framework.Compile(_framework, pluginInterface).ContinueWith(task => {
            if (task.IsFaulted) {
                // Handle exception.
            }
        });
    }
}
```

### Fetching services from the `Framework`

Services can be fetched statically from the `Framework` class by using the `.Service<>()` method. This is useful when
you need to access a service from a static context or when dealing with a class instance that isn't a service by itself.

Note that this only works properly after the container has been compiled.

```csharp
IDalamudTextureWrap myIcon = Framework.Service<ITextureProvider>().GetIcon(1234);
```

### Instantiating a class with dependencies

If you need to instantiate a class that has dependencies but isn't a service itself, you can use the `Framework` class
to create an instance of the class with its dependencies injected.

```csharp
public class MyClass(ITextureProvider textureProvider)
{
}

// Method one:
var myClass = Framework.InstantiateWithDependencies<MyClass>();

// Method two:
var myClass = Framework.InstantiateWithDependencies(typeof(MyClass));
```

## Logging

The `Logger` class is a global logging system that proxies log lines to custom log targets. By default, it does not
output any log messages. You can add log targets by calling the `Framework.AddLogTarget` method. This method accepts
an instance of `ILogger` that should handle the incoming log messages that originate from one of the `Logger` methods.

```csharp
public class MyLogTarget : ILogger
{
    public void Log(LogLevel level, object? message)
    {
        Console.WriteLine($"[{level}] {message}");
    }
}

public class MyPlugin : IDalamudPlugin
{
    public void Load()
    {
        Framework.AddLogTarget(new MyLogTarget());
        
        // Prints "Hello, World!" to the console.
        Logger.Info("Hello, world!");
    }
}
```

The framework ships with a `DefaultLogger` that has the ability to output log messages to both the Dalamud log window,
as well as the chat frame. You still have to add this logger as a log target by yourself if you wish to use it.

You can find the logger and related classes in the [src/Logger](src/Logger) directory.

### Using the Logger

The `Logger` class has the following static methods that can be used to log messages:

 - `Logger.Debug(object? message)`
 - `Logger.Info(object? message)`
 - `Logger.Warning(object? message)`
 - `Logger.Error(object? message)`

It is up to the `ILogger` implementation that has been configured as a LogTarget to decide
how to handle these log messages.

## Lifecycle Attributes

The library ships with a set of attributes that can be added to static methods that should be invoked when the plugin
loads or unloads.

### `[WhenFrameworkCompiling]`

This attribute is invoked when the `Compile` method on the `Framework` class is called. This is useful when you need to
scan registered assemblies for certain types.

```csharp
[WhenFrameworkCompiling]
public static void OnFrameworkCompiling()
{
    Framework.Assemblies.SelectMany(asm => asm.GetTypes())
        .Where(type => type.GetCustomAttribute<SomeAttribute>() != null)
        .ToList()
        .ForEach(type => Console.WriteLine(type.FullName));
}
```

> [!TIP]
> This attribute allows you to pass an `executionOrder` (int) parameter to specify the order in which methods using this
> attribute should be invoked. The default execution order is `0`. A lower number means that it is executed first. If
> two methods share the same execution order, the order in which they are invoked is undefined.

### `[WhenFrameworkAsyncCompiling]`

This is the asynchronous version of the `[WhenFrameworkCompiling]` attribute. It can be used when you need to perform
asynchronous operations during the compilation phase. Since these methods are asynchronous and are invoked in parallel,
there is no way to specify an execution order for them.

These methods are invoked _after_ the synchronous `[WhenFrameworkCompiling]` methods have been invoked.

```csharp
[WhenFrameworkAsyncCompiling]
public static async Task OnFrameworkAsyncCompiling()
{
    await Task.Delay(1000);
}
```

### `[WhenFrameworkDisposing]`

Static methods using this attribute are invoked when the `Dispose` method on the `Framework` class is called. This is
useful when you need to clean up resources that were allocated during the plugin's lifecycle.

> [!TIP]
> This attribute supports the use of an `executionOrder` parameter, just like the `[WhenFrameworkCompiling]` attribute
> which allows you to control when the method is invoked.

```csharp
[WhenFrameworkDisposing]
public static void OnFrameworkDisposing()
{
    Console.WriteLine("Disposing plugin...");
}
```

## Scheduler

The `Scheduler` is responsible for managing recurring tasks. Tasks are defined using the `[OnTick]` and `[OnDraw]`
attributes on instance members of services. The `Scheduler` will then invoke these methods at the appropriate time.

> [!TIP]
> The `[OnTick]` attribute allows you to set an `interval` parameter (in milliseconds) to specify how often the method
> should be invoked. The default interval is `0`, meaning it runs on every tick.

```csharp
[Service]
public class MyService
{
    [OnTick(interval: 1000)]
    public void OnTick()
    {
        // I am invoked on the Game's main thread every second.
    }

    [OnDraw]
    public void OnDraw()
    {
        // I am invoked on every frame.
    }
}
```

## Configuration

Configuration parameters are declared using the `[ConfigVariable]` attribute on static properties. The `ConfigManager`
scans all registered assemblies for these properties and automatically updates their values during runtime. This means
that configuration variables are scoped to the components they are used in, rather than having a global configuration
object that is shared between all components.

```csharp
[Service]
public class MyService
{
    [ConfigVariable("MyBool")]
    public static bool MyBool { get; set; } = true;
}
```

The default value of the property is also the value that is used when the configuration file does not contain the
variable.

A second parameter can be given to the attribute to specify a "Category" for the configuration variable. This is only
useful if you wish to show the configuration variable in the Umbra Settings window. Note that both the category and
variable must have translations defined in the localization files before they will be shown in the settings window.

The naming convention of this is `CVAR.Group.<Category>` for categories, and `CVAR.<Id>.Name` and
`CVAR.<Id>.Description` for variables.

```csharp
[Service]
public class MyService
{
    [ConfigVariable("MyBool", "General")]
    public static bool MyBool { get; set; } = true;
}
```

Translation file:
```json
{
    "CVAR.Group.General": "General Settings",
    "CVAR.MyBool.Name": "My Boolean",
    "CVAR.MyBool.Description": "This is a boolean variable."
}
```

If you wish to update the value of a configuration variable during runtime, you can use the `ConfigManager` class to do
so, using the `Set` method.

```csharp
ConfigManager.Set("MyBool", false);
```

The system will automatically update all properties that are marked with the `[ConfigVariable]` attribute that have the
same name as the key you are setting.
