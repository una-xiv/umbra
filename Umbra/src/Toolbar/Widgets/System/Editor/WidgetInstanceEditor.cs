using System;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;
using Umbra.Widgets.System;
using Umbra.Windows;
using Umbra.Windows.Components;
using Umbra.Windows.Library.VariablesWindow;
using Una.Drawing;

namespace Umbra.Widgets;

[Service]
internal sealed class WidgetInstanceEditor : IDisposable
{
    private WidgetManager WidgetManager { get; }
    private WindowManager WindowManager { get; }

    private InstanceEditor? _editor;

    public WidgetInstanceEditor(WidgetManager widgetManager, WindowManager windowManager)
    {
        WidgetManager = widgetManager;
        WindowManager = windowManager;

        WidgetManager.OnWidgetRemoved += OnWidgetRemoved;
    }

    public void OpenEditor(ToolbarWidget widget)
    {
        _editor?.Dispose();
        _editor = new(widget, WidgetManager, WindowManager);
        Logger.Info($"Open settings for widget {widget.Id} (Type={widget.Info.Id})");

        _editor.OnDisposed += () => _editor = null;
    }

    public void Dispose()
    {
        WidgetManager.OnWidgetRemoved -= OnWidgetRemoved;

        _editor?.Dispose();
        _editor = null;
    }

    [OnTick]
    private void OnTick()
    {
        _editor?.Update();
    }

    private void OnWidgetRemoved(ToolbarWidget widget)
    {
        _editor?.Dispose();
        _editor = null;
    }

    private sealed class InstanceEditor : IDisposable
    {
        public event Action? OnDisposed;

        private readonly Dictionary<Variable, IWidgetConfigVariable> _variables = new();

        private bool _isDisposed;

        private readonly ButtonNode _copyButton  = new("Copy", I18N.Translate("Copy"));
        private readonly ButtonNode _pasteButton = new("Paste", I18N.Translate("Paste"));

        private ToolbarWidget Widget        { get; }
        private WidgetManager WidgetManager { get; }

        public InstanceEditor(ToolbarWidget widget, WidgetManager widgetManager, WindowManager windowManager)
        {
            Widget        = widget;
            WidgetManager = widgetManager;

            foreach (IWidgetConfigVariable wVar in widget.GetConfigVariableList()) {
                if (wVar.IsHidden) continue;

                Variable? variable = ConvertWidgetVariable(widget, wVar);
                if (variable == null) continue;

                _variables.Add(variable, wVar);
            }

            _copyButton.OnMouseUp  += _ => Widget.CopyInstanceSettingsToClipboard();
            _pasteButton.OnMouseUp += _ => PasteSettings();

            windowManager.Present(
                "WidgetInstanceEditor",
                new VariablesWindow(
                    widget.GetInstanceName(),
                    _variables.Keys.ToList(),
                    [_copyButton, _pasteButton]
                ),
                _ => {
                    Dispose();
                }
            );
        }

        public void Dispose()
        {
            _isDisposed = true;

            foreach (Variable variable in _variables.Keys) variable.Dispose();
            _variables.Clear();

            OnDisposed?.Invoke();

            foreach (Delegate handler in OnDisposed?.GetInvocationList() ?? []) {
                OnDisposed -= (Action)handler;
            }

            OnDisposed = null;
        }

        public void Update()
        {
            if (_isDisposed) return;

            _pasteButton.IsDisabled = !WidgetManager.HasInstanceClipboardData(Widget);
        }

        private void PasteSettings()
        {
            Widget.PasteInstanceSettingsFromClipboard();

            foreach ((Variable variable, IWidgetConfigVariable wVar) in _variables) {
                switch (variable) {
                    case BooleanVariable b when wVar is BooleanWidgetConfigVariable wb:
                        b.Value = wb.Value;
                        break;
                    case ColorVariable c when wVar is ColorWidgetConfigVariable wc:
                        c.Value = wc.Value;
                        break;
                    case FloatVariable f when wVar is FloatWidgetConfigVariable wf:
                        f.Value = wf.Value;
                        break;
                    case IconIdVariable ic when wVar is IconIdWidgetConfigVariable wic:
                        ic.Value = wic.Value;
                        break;
                    case IntegerVariable i when wVar is IntegerWidgetConfigVariable wi:
                        i.Value = wi.Value;
                        break;
                    case StringVariable s when wVar is StringWidgetConfigVariable ws:
                        s.Value = ws.Value;
                        break;
                    case StringSelectVariable ss when wVar is SelectWidgetConfigVariable wss:
                        ss.Value = wss.Value;
                        break;
                    case FaIconVariable fa when wVar is FaIconWidgetConfigVariable wfa:
                        fa.Value = wfa.Value;
                        break;
                }
            }
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
                case FaIconWidgetConfigVariable fa: {
                    FaIconVariable faVar = new(fa.Id) {
                        Category    = fa.Category,
                        Name        = fa.Name,
                        Description = fa.Description,
                        Value       = fa.Value,
                    };

                    faVar.ValueChanged += v => widget.SetConfigValue(fa.Id, v);

                    return faVar;
                }
                default:
                    Logger.Warning($"No conversion for {var.GetType().Name}.");
                    return null;
            }
        }
    }
}
