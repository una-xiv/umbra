using System;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;
using Umbra.Windows;
using Umbra.Windows.Library.VariablesWindow;

namespace Umbra.Widgets;

[Service]
internal sealed class WidgetInstanceEditor(WindowManager windowManager) : IDisposable
{
    private WindowManager WindowManager { get; } = windowManager;

    private InstanceEditor? _editor;

    public void OpenEditor(ToolbarWidget widget)
    {
        _editor?.Dispose();
        _editor = new(widget, WindowManager);
        Logger.Info($"Open settings for widget {widget.Id} (Type={widget.Info.Id})");

        _editor.OnDisposed += () => _editor = null;
    }

    public void Dispose()
    {
        _editor?.Dispose();
    }

    private sealed class InstanceEditor : IDisposable
    {
        public event Action? OnDisposed;

        private readonly Dictionary<Variable, IWidgetConfigVariable> _variables = new();

        public InstanceEditor(ToolbarWidget widget, WindowManager windowManager)
        {
            foreach (IWidgetConfigVariable wVar in widget.GetConfigVariableList()) {
                if (wVar.IsHidden) continue;

                Variable? variable = ConvertWidgetVariable(widget, wVar);
                if (variable == null) continue;

                _variables.Add(variable, wVar);
            }

            windowManager.Present(
                "WidgetInstanceEditor",
                new VariablesWindow(widget.GetInstanceName(), _variables.Keys.ToList()),
                _ => {
                    Dispose();
                }
            );
        }

        public void Dispose()
        {
            foreach (Variable variable in _variables.Keys) variable.Dispose();
            _variables.Clear();

            OnDisposed?.Invoke();

            foreach (Delegate handler in OnDisposed?.GetInvocationList() ?? []) {
                OnDisposed -= (Action)handler;
            }

            OnDisposed = null;
        }

        private static Variable? ConvertWidgetVariable(ToolbarWidget widget, IWidgetConfigVariable var)
        {
            switch (var) {
                case IntegerWidgetConfigVariable i: {
                    IntegerVariable intVar = new(i.Id) {
                        Category    = i.Category,
                        Name        = i.Name,
                        Description = i.Description,
                        Value       = i.Value,
                        Min         = i.MinValue,
                        Max         = i.MaxValue,
                    };

                    intVar.ValueChanged += v => widget.SetConfigValue(i.Id, v);

                    return intVar;
                }
                case IconIdWidgetConfigVariable i: {
                    IconIdVariable iconVar = new(i.Id) {
                        Category    = i.Category,
                        Name        = i.Name,
                        Description = i.Description,
                        Value       = i.Value,
                    };

                    iconVar.ValueChanged += v => widget.SetConfigValue(i.Id, v);

                    return iconVar;
                }
                case FloatWidgetConfigVariable f: {
                    FloatVariable floatVar = new(f.Id) {
                        Category    = f.Category,
                        Name        = f.Name,
                        Description = f.Description,
                        Value       = f.Value,
                        Min         = f.MinValue,
                        Max         = f.MaxValue,
                    };

                    floatVar.ValueChanged += v => widget.SetConfigValue(f.Id, v);

                    return floatVar;
                }
                case BooleanWidgetConfigVariable b: {
                    BooleanVariable boolVar = new(b.Id) {
                        Category    = b.Category,
                        Name        = b.Name,
                        Description = b.Description,
                        Value       = b.Value,
                    };

                    boolVar.ValueChanged += v => widget.SetConfigValue(b.Id, v);

                    return boolVar;
                }
                case StringWidgetConfigVariable s: {
                    StringVariable stringVar = new(s.Id) {
                        Category    = s.Category,
                        Name        = s.Name,
                        Description = s.Description,
                        Value       = s.Value,
                    };

                    stringVar.ValueChanged += v => widget.SetConfigValue(s.Id, v);

                    return stringVar;
                }
                case SelectWidgetConfigVariable select: {
                    StringSelectVariable selectVar = new(select.Id) {
                        Category    = select.Category,
                        Name        = select.Name,
                        Description = select.Description,
                        Value       = select.Value,
                        Choices     = select.Options,
                    };

                    selectVar.ValueChanged += v => widget.SetConfigValue(select.Id, v);

                    return selectVar;
                }
                case ColorWidgetConfigVariable c: {
                    ColorVariable colorVar = new(c.Id) {
                        Category    = c.Category,
                        Name        = c.Name,
                        Description = c.Description,
                        Value       = c.Value,
                    };

                    colorVar.ValueChanged += v => widget.SetConfigValue(c.Id, v);

                    return colorVar;
                }
                default:
                    Logger.Warning($"No conversion for {var.GetType().Name}.");
                    return null;
            }
        }
    }
}
