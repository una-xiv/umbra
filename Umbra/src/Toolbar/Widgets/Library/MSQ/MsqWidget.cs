using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Umbra.Widgets;

[ToolbarWidget(
    "MSQ",
    "Widget.MSQ.Name",
    "Widget.MSQ.Description",
    ["msq", "main", "quest", "journal"]
)]
public class MsqWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Text |
        StandardWidgetFeatures.SubText |
        StandardWidgetFeatures.ProgressBar |
        StandardWidgetFeatures.Icon;

    private readonly Dictionary<uint, Quest>        _questCache = [];
    private readonly Dictionary<uint, ScenarioTree> _treeCache  = [];

    private Quest? _currentQuest;

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            ..base.GetConfigVariables(),
            new BooleanWidgetConfigVariable(
                "ShowProgress",
                I18N.Translate("Widget.MSQ.Config.ShowProgress.Name"),
                I18N.Translate("Widget.MSQ.Config.ShowProgress.Description"),
                true
            ),
            new BooleanWidgetConfigVariable(
                "CurrentExpansionOnly",
                I18N.Translate("Widget.MSQ.Config.CurrentExpansionOnly.Name"),
                I18N.Translate("Widget.MSQ.Config.CurrentExpansionOnly.Description"),
                false
            ),
        ];
    }

    protected override void OnLoad()
    {
        Node.OnClick += OpenMapToQuestLocation;
    }

    protected override void OnUnload()
    {
        Node.OnClick -= OpenMapToQuestLocation;
    }

    protected override void OnDraw()
    {
        _currentQuest = GetCurrentMainScenarioQuest();

        if (_currentQuest == null) {
            IsVisible = false;
            return;
        }

        IsVisible = true;
        SetText(_currentQuest.Value.Name.ToString());
        SetGameIconId(71001);

        if (GetConfigValue<bool>("ShowProgress")) {
            var progress = GetQuestProgress();
            SetProgressBarConstraint(0, progress.Item2);
            SetProgressBarValue(progress.Item1);
            SetSubText($"{progress.Item1} / {progress.Item2}");
        } else {
            SetProgressBarConstraint(0, 0);
            SetProgressBarValue(0);
            SetSubText(null);
        }
    }

    private unsafe void OpenMapToQuestLocation(Node _)
    {
        if (_currentQuest == null) return;

        var result = stackalloc AtkValue[1];
        var values = stackalloc AtkValue[2];
        values[0].SetInt(0);
        values[1].SetInt(1);

        AgentScenarioTree.Instance()->ReceiveEvent(result, values, 2, 0);
    }

    private unsafe ScenarioTree? GetCurrentMainScenarioTree()
    {
        var agent = AgentScenarioTree.Instance();
        if (agent == null || agent->Data == null) return null;

        var index = agent->Data->CurrentScenarioQuest;

        if (_treeCache.TryGetValue(index, out var cachedTree)) {
            return cachedTree;
        }

        var tree = Framework.Service<IDataManager>().GetExcelSheet<ScenarioTree>().FindRow(index | 0x10000U);
        if (tree == null) return null;

        _treeCache.TryAdd(index, tree.Value);

        return tree;
    }

    private Quest? GetCurrentMainScenarioQuest()
    {
        var tree = GetCurrentMainScenarioTree();

        if (tree != null) {
            if (_questCache.TryGetValue(tree.Value.RowId, out var cachedQuest)) {
                return cachedQuest;
            }
        }

        var quest = tree.HasValue
            ? Framework.Service<IDataManager>().GetExcelSheet<Quest>().FindRow(tree.Value.RowId)
            : null;

        if (tree != null && quest != null) {
            _questCache.TryAdd(tree.Value.RowId, quest.Value);
        }

        return quest;
    }

    private Tuple<int, int> GetQuestProgress()
    {
        if (_currentQuest == null) return Tuple.Create(0, 0);

        IDataManager      dataManager = Framework.Service<IDataManager>();
        RowRef<ExVersion> expansion   = _currentQuest.Value.Expansion;
        bool              thisExOnly  = GetConfigValue<bool>("CurrentExpansionOnly");

        int total     = 0;
        int completed = 0;

        foreach (var st in dataManager.GetExcelSheet<ScenarioTree>()) {
            var quest = dataManager.GetExcelSheet<Quest>().GetRow(st.RowId);

            // expansion.IsValid is a check for patch days when the data is not available yet.
            if (thisExOnly && expansion.IsValid && quest.Expansion.RowId != expansion.Value.RowId) continue;

            total++;

            if (QuestManager.IsQuestComplete(quest.RowId)) {
                completed++;
            }
        }

        return Tuple.Create(completed, total);
    }
}
