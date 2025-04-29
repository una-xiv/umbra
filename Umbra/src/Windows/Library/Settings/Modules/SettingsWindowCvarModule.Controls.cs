using Dalamud.Interface;
using Umbra.Common;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Windows.Settings.Modules;

internal partial class SettingsWindowCvarModule
{
    private Node? RenderCvar(Cvar cvar)
    {
        if (cvar.Default is bool) {
            var node = new CheckboxNode(
                cvar.Slug,
                (bool)(cvar.Value ?? false),
                I18N.Translate($"CVAR.{cvar.Id}.Name"),
                I18N.Has($"CVAR.{cvar.Id}.Description") ? I18N.Translate($"CVAR.{cvar.Id}.Description") : null
            );

            node.ClassList.Add("cvar");
            node.OnValueChanged += value => ConfigManager.Set(cvar.Id, value);

            return CreateNode(cvar, node);
        }

        if (cvar.Options is not null && cvar.Options.Count > 0 && cvar.Default is string) {
            var node = new SelectNode(
                cvar.Slug,
                (string)(cvar.Value ?? cvar.Default),
                cvar.Options,
                I18N.Translate($"CVAR.{cvar.Id}.Name"),
                I18N.Has($"CVAR.{cvar.Id}.Description") ? I18N.Translate($"CVAR.{cvar.Id}.Description") : null
            );

            node.ClassList.Add("cvar");
            node.OnValueChanged += value => ConfigManager.Set(cvar.Id, value);

            return CreateNode(cvar, node);
        }

        if (cvar is { Default: int, Min: not null, Max: not null }) {
            var node = new IntegerInputNode(
                cvar.Slug,
                (int)(cvar.Value ?? cvar.Default),
                (int)cvar.Min,
                (int)cvar.Max,
                I18N.Translate($"CVAR.{cvar.Id}.Name"),
                I18N.Has($"CVAR.{cvar.Id}.Description") ? I18N.Translate($"CVAR.{cvar.Id}.Description") : null
            );

            node.ClassList.Add("cvar");
            node.OnValueChanged += value => ConfigManager.Set(cvar.Id, value);

            return CreateNode(cvar, node);
        }

        if (cvar is { Default: float, Min: not null, Max: not null }) {
            var node = new FloatInputNode(
                cvar.Slug,
                (float)(cvar.Value ?? cvar.Default),
                (float)cvar.Min,
                (float)cvar.Max,
                I18N.Translate($"CVAR.{cvar.Id}.Name"),
                I18N.Has($"CVAR.{cvar.Id}.Description") ? I18N.Translate($"CVAR.{cvar.Id}.Description") : null
            );

            node.ClassList.Add("cvar");
            node.OnValueChanged += value => ConfigManager.Set(cvar.Id, value);

            return CreateNode(cvar, node);
        }

        return null;
    }

    private Node CreateNode(Cvar cvar, Node controlNode)
    {
        ButtonNode resetNode = new(
            "Reset",
            null,
            FontAwesomeIcon.History,
            true
        ) {
            Style = new() {
                Anchor = Anchor.TopRight,
                Margin = new() { Top = -4 },
            },
            Tooltip = I18N.Translate("Revert")
        };

        Node node = new() {
            ClassList  = ["cvar-control-node"],
            ChildNodes = [controlNode, resetNode],
            Style = new() {
                AutoSize = (AutoSize.Grow, AutoSize.Fit),
                Flow     = Flow.Horizontal,
                Gap      = 8,
            }
        };

        var c = cvar;

        resetNode.OnClick += _ => {
            ConfigManager.Set(c.Id, c.Default);

            if (controlNode is CheckboxNode n) n.Value     = (bool)c.Default!;
            if (controlNode is SelectNode s) s.Value       = (string)c.Default!;
            if (controlNode is IntegerInputNode i) i.Value = (int)c.Default!;
            if (controlNode is FloatInputNode f) f.Value   = (float)c.Default!;
        };

        resetNode.BeforeDraw += _ => {
            resetNode.Style.IsVisible = c.Value is not null && !c.Value.Equals(c.Default);
        };

        return node;
    }
}
