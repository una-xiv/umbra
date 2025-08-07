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
    public override MenuPopup Popup { get; } = new();

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
            )
        ];
    }

    private readonly Dictionary<byte, PhantomJob>       _jobs    = [];
    private readonly Dictionary<byte, MenuPopup.Button> _buttons = [];

    private bool        _isInfoAvailable;
    private PhantomJob? _selectedJob;

    /// <inheritdoc/>
    protected override void OnLoad()
    {
        foreach (var job in Framework.Service<IDataManager>().GetExcelSheet<MKDSupportJob>()) {
            var phJob  = new PhantomJob((byte)job.RowId, job.Unknown1.ExtractText(), job.RowId + 82271u);
            var button = new MenuPopup.Button(phJob.Name) { Icon = phJob.IconId };

            button.OnClick += () => {
                if (phJob.Level > 0) {
                    PublicContentOccultCrescent.ChangeSupportJob(phJob.Id);
                }
            };

            _buttons.Add(phJob.Id, button);
            _jobs.Add((byte)job.RowId, phJob);
            Popup.Add(button);
        }
    }

    /// <inheritdoc/>
    protected override void OnUnload()
    {
    }

    /// <inheritdoc/>
    protected override void OnDraw()
    {
        UpdatePhantomJobs();

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
            Popup.IsDisabled = true;
            return;
        }
        
        _selectedJob = _jobs.GetValueOrDefault(state->CurrentSupportJob);
        if (null == _selectedJob) return;

        Popup.IsDisabled = false;
        _isInfoAvailable = true;
        _selectedJob     = _jobs.GetValueOrDefault(state->CurrentSupportJob);

        foreach (var job in _jobs.Values) {
            job.Experience = state->SupportJobExperience[job.Id];
            job.Level = (state->SupportJobLevels[job.Id]);
            var button = _buttons[job.Id];
            
            button.IsVisible = job.Level > 0;
            button.AltText = I18N.Translate("Widget.GearsetSwitcher.JobLevel", job.Level);
        }
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
