﻿using System;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;
using Umbra.Widgets.System;
using Umbra.Windows;
using Umbra.Windows.Components;
using Umbra.Windows.Library.VariableEditor;

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
                new VariablesEditorWindow(
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

            // foreach (Variable variable in _variables.Keys) variable.Dispose();
            // _variables.Clear();

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
                    case GameIconVariable ic when wVar is IconIdWidgetConfigVariable wic:
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
                        Group       = i.Group,
                        Value       = i.Value,
                        Min         = i.MinValue,
                        Max         = i.MaxValue,
                        DisplayIf   = i.DisplayIf,
                    };

                    intVar.ValueChanged += v => widget.SetConfigValue(i.Id, v);

                    return intVar;
                }
                case IconIdWidgetConfigVariable i: {
                    GameIconVariable gameIconVar = new(i.Id) {
                        Category    = i.Category,
                        Name        = i.Name,
                        Description = i.Description,
                        Group       = i.Group,
                        Value       = i.Value,
                        DisplayIf   = i.DisplayIf,
                    };

                    gameIconVar.ValueChanged += v => widget.SetConfigValue(i.Id, v);

                    return gameIconVar;
                }
                case FloatWidgetConfigVariable f: {
                    FloatVariable floatVar = new(f.Id) {
                        Category    = f.Category,
                        Name        = f.Name,
                        Description = f.Description,
                        Group       = f.Group,
                        Value       = f.Value,
                        Min         = f.MinValue,
                        Max         = f.MaxValue,
                        DisplayIf   = f.DisplayIf,
                    };

                    floatVar.ValueChanged += v => widget.SetConfigValue(f.Id, v);

                    return floatVar;
                }
                case BooleanWidgetConfigVariable b: {
                    BooleanVariable boolVar = new(b.Id) {
                        Category    = b.Category,
                        Name        = b.Name,
                        Description = b.Description,
                        Group       = b.Group,
                        Value       = b.Value,
                        DisplayIf   = b.DisplayIf,
                    };

                    boolVar.ValueChanged += v => widget.SetConfigValue(b.Id, v);

                    return boolVar;
                }
                case StringWidgetConfigVariable s: {
                    StringVariable stringVar = new(s.Id) {
                        Category    = s.Category,
                        Name        = s.Name,
                        Description = s.Description,
                        Group       = s.Group,
                        Value       = s.Value,
                        DisplayIf   = s.DisplayIf,
                    };

                    stringVar.ValueChanged += v => widget.SetConfigValue(s.Id, v);

                    return stringVar;
                }
                case SelectWidgetConfigVariable select: {
                    StringSelectVariable selectVar = new(select.Id) {
                        Category    = select.Category,
                        Name        = select.Name,
                        Description = select.Description,
                        Group       = select.Group,
                        Value       = select.Value,
                        Choices     = select.Options,
                        DisplayIf   = select.DisplayIf,
                    };

                    selectVar.ValueChanged += v => widget.SetConfigValue(select.Id, v);

                    return selectVar;
                }
                case ColorWidgetConfigVariable c: {
                    ColorVariable colorVar = new(c.Id) {
                        Category    = c.Category,
                        Name        = c.Name,
                        Description = c.Description,
                        Group       = c.Group,
                        Value       = c.Value,
                        DisplayIf   = c.DisplayIf,
                    };

                    colorVar.ValueChanged += v => widget.SetConfigValue(c.Id, v);

                    return colorVar;
                }
                case FaIconWidgetConfigVariable fa: {
                    FaIconVariable faVar = new(fa.Id) {
                        Category    = fa.Category,
                        Name        = fa.Name,
                        Description = fa.Description,
                        Group       = fa.Group,
                        Value       = fa.Value,
                        DisplayIf   = fa.DisplayIf,
                    };

                    faVar.ValueChanged += v => widget.SetConfigValue(fa.Id, v);

                    return faVar;
                }
                case GameGlyphWidgetConfigVariable gg: {
                    GameGlyphVariable ggVar = new(gg.Id) {
                        Category    = gg.Category,
                        Name        = gg.Name,
                        Description = gg.Description,
                        Group       = gg.Group,
                        Value       = gg.Value,
                        DisplayIf   = gg.DisplayIf,
                    };

                    ggVar.ValueChanged += v => widget.SetConfigValue(gg.Id, v);

                    return ggVar;
                }
                case BitmapIconWidgetConfigVariable bi: {
                    BitmapIconVariable biVar = new(bi.Id) {
                        Category    = bi.Category,
                        Name        = bi.Name,
                        Description = bi.Description,
                        Group       = bi.Group,
                        Value       = bi.Value,
                        DisplayIf   = bi.DisplayIf,
                    };

                    biVar.ValueChanged += v => widget.SetConfigValue(bi.Id, v);

                    return biVar;
                }
                case IEnumWidgetConfigVariable e: {
                    return e.CreateEnumVariable(widget);
                }
                default:
                    Logger.Warning($"No conversion for {var.GetType().Name}.");
                    return null;
            }
        }
    }
}
