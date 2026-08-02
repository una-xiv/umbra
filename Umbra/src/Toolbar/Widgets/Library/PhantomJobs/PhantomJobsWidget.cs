using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
using Lumina.Excel.Sheets;

namespace Umbra.Widgets.Library.PhantomJobs;

[ToolbarWidget(
    "PhantomJobs",
    "Widget.PhantomJobs.Title",
    "Widget.PhantomJobs.Description",
    ["OC", "occult", "crescent", "phantom", "jobs"]
)]
public class PhantomJobsWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    private MenuPopup MenuPopup { get; } = new();
    private PhantomJobsColumnsPopup ColumnsPopup { get; } = new();

    public override WidgetPopup Popup => _currentDisplayMode == "Columns" ? ColumnsPopup : MenuPopup;

    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Icon |
        StandardWidgetFeatures.Text |
        StandardWidgetFeatures.SubText;

    protected override string DefaultIconType   => IconTypeGameIcon;
    protected override uint   DefaultGameIconId => 13;

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            ..base.GetConfigVariables(),
            new StringWidgetConfigVariable(
                "Label",
                I18N.Translate("Widget.CustomButton.Config.Label.Name"),
                I18N.Translate("Widget.CustomButton.Config.Label.Description"),
                Info.Name,
                1024,
                true
            ),
            new SelectWidgetConfigVariable(
                "PopupDisplayMode",
                I18N.Translate("Widget.PhantomJobs.DisplayMode.Name"),
                I18N.Translate("Widget.PhantomJobs.DisplayMode.Description"),
                "ListNew",
                new() {
                    { "ListLegacy", I18N.Translate("Widget.PhantomJobs.DisplayMode.ListLegacy") },
                    { "ListNew", I18N.Translate("Widget.PhantomJobs.DisplayMode.ListNew") },
                    { "Columns", I18N.Translate("Widget.PhantomJobs.DisplayMode.Columns") }
                }
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") }
        ];
    }

    private readonly Dictionary<byte, PhantomJob>       _jobs    = [];
    private readonly Dictionary<byte, MenuPopup.Button> _buttons = [];
    private string _currentDisplayMode = "ListNew";

    private bool        _isInfoAvailable;
    private PhantomJob? _selectedJob;

    /// <inheritdoc/>
    protected override void OnLoad()
    {
        foreach (var job in Framework.Service<IDataManager>().GetExcelSheet<MKDSupportJob>()) {
            var phJob  = new PhantomJob((byte)job.RowId, job.NameShort.ExtractText(), job.RowId + 82271u);
            var button = new MenuPopup.Button(phJob.Name) {
                Icon = phJob.IconId
            };

            button.OnClick += () => {
                if (phJob.Level > 0) {
                    PublicContentOccultCrescent.ChangeSupportJob(phJob.Id);
                }
            };

            _buttons.Add(phJob.Id, button);
            _jobs.Add((byte)job.RowId, phJob);
        }

        RebuildPopupMenu();
    }

    /// <inheritdoc/>
    protected override void OnDraw()
    {
        UpdatePhantomJobs();

        var displayMode = GetConfigValue<string>("PopupDisplayMode");
        if (displayMode != _currentDisplayMode) {
            _currentDisplayMode = displayMode;
            RebuildPopupMenu();
        }

        if (!_isInfoAvailable || null == _selectedJob) {
            IsVisible = false;
            return;
        }

        IsVisible = true;

        SetGameIconId(_selectedJob.IconId);
        SetText(_selectedJob.Name);
        SetSubText($"Lv. {_selectedJob.Level}");
    }

    private unsafe void UpdatePhantomJobs()
    {
        var state = PublicContentOccultCrescent.GetState();
        if (state == null) {
            _isInfoAvailable = false;
            MenuPopup.IsDisabled = true;
            ColumnsPopup.IsDisabled = true;
            return;
        }

        _selectedJob = _jobs.GetValueOrDefault(state->CurrentSupportJob);
        if (null == _selectedJob) return;

        MenuPopup.IsDisabled = false;
        ColumnsPopup.IsDisabled = false;
        _isInfoAvailable = true;

        foreach (var job in _jobs.Values) {
            job.Experience = state->SupportJobExperience[job.Id];
            job.Level = (state->SupportJobLevels[job.Id]);

            if (_buttons.TryGetValue(job.Id, out var button)) {
                button.IsVisible = job.Level > 0;
                button.AltText = I18N.Translate("Widget.GearsetSwitcher.JobLevel", job.Level);
            }
        }
    }

    private void RebuildPopupMenu()
    {
        if (_currentDisplayMode == "Columns") {
            BuildColumnsMenu();
        } else {
            MenuPopup.Clear(false);

            if (_currentDisplayMode == "ListNew") {
                BuildListNewMenu();
            } else {
                BuildListLegacyMenu();
            }
        }
    }

    private void BuildListLegacyMenu()
    {
        foreach (var button in _buttons.Values) {
            MenuPopup.Add(button);
        }
    }

    private Dictionary<string, List<byte>> GetJobCategories()
    {
        return new() {
            { I18N.Translate("Widget.PhantomJobs.JobCategory.Tank"), new() { 1 } },
            { I18N.Translate("Widget.PhantomJobs.JobCategory.MainDPS"), new() { 2, 3, 5, 9, 13, 14, 19, 16 } },
            { I18N.Translate("Widget.PhantomJobs.JobCategory.SupportDPS"), new() { 6, 12, 4, 15 } },
            { I18N.Translate("Widget.PhantomJobs.JobCategory.Caster"), new() { 8, 11, 18, 21, 22, 20, 23 } },
            { I18N.Translate("Widget.PhantomJobs.JobCategory.PureSupport"), new() { 7, 10, 17 } },
            { I18N.Translate("Widget.PhantomJobs.JobCategory.Other"), new() { 0 } }
        };
    }

    private void BuildListNewMenu()
    {
        var categories = GetJobCategories();

        foreach (var (categoryName, jobIds) in categories) {
            var group = new MenuPopup.Group(categoryName);

            foreach (var jobId in jobIds) {
                if (_buttons.TryGetValue(jobId, out var button)) {
                    group.Add(button);
                }
            }

            MenuPopup.Add(group);
        }
    }

    private void BuildColumnsMenu()
    {
        var categoriesDict = GetJobCategories();

        var jobDataDict = new Dictionary<byte, (string name, uint iconId, string level, Action<byte> onClick)>();

        foreach (var job in _jobs.Values) {
            if (job.Level > 0) {
                jobDataDict[job.Id] = (
                    job.Name,
                    job.IconId,
                    $"Lv. {job.Level}",
                    _ => {
                            PublicContentOccultCrescent.ChangeSupportJob(job.Id);
                        }
                );
            }
        }

        ColumnsPopup.BuildColumnsView(categoriesDict, jobDataDict);
    }

    private class PhantomJob(byte id, string name, uint iconId)
    {
        public byte   Id                 { get; } = id;
        public string Name               { get; } = name;
        public uint   IconId             { get; } = iconId;
        public byte   Level              { get; set; }
        public uint   Experience         { get; set; }

        public override string ToString()
        {
            return $"{Id}: {Name} (Lv. {Level}, Exp: {Experience})";
        }
    }
}
