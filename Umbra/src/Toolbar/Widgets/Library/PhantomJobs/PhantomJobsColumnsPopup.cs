using Umbra.Widgets.Popup;

namespace Umbra.Widgets.Library.PhantomJobs;

public class PhantomJobsColumnsPopup : WidgetPopup
{
    private Node _node = null!;

    protected override Node Node => _node;

    public bool IsDisabled { get; set; }

    private readonly UdtDocument _document = UmbraDrawing.DocumentFrom("umbra.widgets._phantom_jobs_columns.xml");

    private MenuPopup _menuPopup { get; } = new();

    public PhantomJobsColumnsPopup()
    {
        _node = _document.RootNode!;
    }

    public MenuPopup GetMenuPopup()
    {
        return _menuPopup;
    }

    public void BuildColumnsView(Dictionary<string, List<byte>> categories, Dictionary<byte, (string name, uint iconId, string level, Action<byte> onClick)> jobData)
    {
        var container = Node.QuerySelector(".columns-container");
        if (container == null) {
            Logger.Warning("BuildColumnsView: .columns-container not found");
            return;
        }

        container.Clear();

        foreach (var (categoryName, jobIds) in categories) {
            var columnNode = new Node {
                ClassList = ["job-column"]
            };

            var headerNode = new Node {
                ClassList = ["column-header"],
                NodeValue = categoryName
            };
            columnNode.AppendChild(headerNode);

            var contentNode = new Node { ClassList = ["column-content"] };
            columnNode.AppendChild(contentNode);

            foreach (var jobId in jobIds) {
                if (jobData.TryGetValue(jobId, out var job)) {
                    var buttonNode = new Node {
                        ClassList = ["job-button"],
                        ChildNodes = [
                            new() { ClassList = ["job-icon"], Style = new() { IconId = job.iconId } },
                            new() { ClassList = ["job-name"], NodeValue = job.name },
                            new() { ClassList = ["job-level"], NodeValue = job.level }
                        ]
                    };

                    buttonNode.OnClick += _ => {
                        job.onClick(jobId);
                        Close();
                    };
                    contentNode.AppendChild(buttonNode);
                }
            }

            if (contentNode.ChildNodes.Count > 0)
                container.AppendChild(columnNode);
        }
    }

    protected override bool CanOpen()
    {
        return !IsDisabled && base.CanOpen();
    }
}
