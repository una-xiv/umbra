using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Umbra.Common;
using Umbra.Plugins.Repository;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Windows.Settings.Components;

/// <summary>
/// A control node that allows the user to install a plugin via a GitHub repository.
/// </summary>
/// <remarks>
/// This functionality is only available if the user has been warned about the
/// dangers of custom plugins and has then explicitly enabled this feature.
/// </remarks>
public class RepositoryInstallerNode : UdtNode
{
    private Node InputStepNode   => QuerySelector("#step-input")!;
    private Node FetchStepNode   => QuerySelector("#step-fetch")!;
    private Node ErrorStepNode   => QuerySelector("#step-error")!;
    private Node ConfirmStepNode => QuerySelector("#step-confirm")!;

    private StringInputNode AuthorInput          => QuerySelector<StringInputNode>("#author")!;
    private StringInputNode RepoInput            => QuerySelector<StringInputNode>("#repository")!;
    private ButtonNode      FetchButton          => QuerySelector<ButtonNode>("#btn-install-from-url")!;
    private ButtonNode      ErrorCancelButton    => QuerySelector<ButtonNode>("#btn-error-cancel")!;
    private ButtonNode      ConfirmCancelButton  => QuerySelector<ButtonNode>("#btn-confirm-cancel")!;
    private ButtonNode      ConfirmInstallButton => QuerySelector<ButtonNode>("#btn-confirm-install")!;

    private string _author = "una-xiv";
    private string _repo   = "Umbra.SamplePlugin";

    private PluginFetcher.Release? _release;

    public RepositoryInstallerNode() : base("umbra.windows.settings.components.repository_installer_node.xml")
    {
        AuthorInput.Value = _author;
        RepoInput.Value   = _repo;

        AuthorInput.OnValueChanged += val => {
            _author                = val;
            FetchButton.IsDisabled = !IsValidRepo();
        };
        RepoInput.OnValueChanged += val => {
            _repo                  = val;
            FetchButton.IsDisabled = !IsValidRepo();
        };

        FetchButton.IsDisabled       =  true;
        FetchButton.OnClick          += _ => Fetch().ContinueWith(_ => { });
        ErrorCancelButton.OnClick    += _ => ShowInputStep();
        ConfirmCancelButton.OnClick  += _ => ShowInputStep();
        ConfirmInstallButton.OnClick += _ => DownloadAndInstall().ContinueWith(_ => { });

        ShowInputStep();
    }

    private bool IsValidRepo()
    {
        return !string.IsNullOrWhiteSpace(_author) &&
               !string.IsNullOrWhiteSpace(_repo) &&
               null == PluginRepository.FindEntryFromRepository(_author, _repo);
    }

    private async Task Fetch()
    {
        if (!IsValidRepo()) return;

        ShowFetchStep();
        FetchButton.IsDisabled = true;

        (PluginFetcher.FetchResult result, PluginFetcher.Release? release)
            = await PluginFetcher.Fetch(_author, _repo);

        if (result == PluginFetcher.FetchResult.Error) {
            ShowErrorStep();
            return;
        }

        if (result == PluginFetcher.FetchResult.NewerVersionAvailable) {
            _release = release!.Value;
            ShowConfirmStep();
            return;
        }

        // Already on the latest version. This should never happen in this scenario.
        Logger.Warning($"Attempted to fetch a plugin repository that is already installed: {_author}/{_repo}");
        ShowInputStep();
    }

    private void ShowErrorStep()
    {
        ConfirmStepNode.Style.IsVisible = false;
        InputStepNode.Style.IsVisible   = false;
        FetchStepNode.Style.IsVisible   = false;
        ErrorStepNode.Style.IsVisible   = true;
    }

    private void ShowInputStep()
    {
        ConfirmStepNode.Style.IsVisible = false;
        InputStepNode.Style.IsVisible   = true;
        FetchStepNode.Style.IsVisible   = false;
        ErrorStepNode.Style.IsVisible   = false;

        FetchButton.IsDisabled = !IsValidRepo();
    }

    private void ShowFetchStep()
    {
        ConfirmStepNode.Style.IsVisible = false;
        InputStepNode.Style.IsVisible   = false;
        FetchStepNode.Style.IsVisible   = true;
        ErrorStepNode.Style.IsVisible   = false;
    }

    private void ShowConfirmStep()
    {
        if (null == _release) {
            ShowInputStep();
            return;
        }

        ConfirmStepNode.Style.IsVisible = true;
        InputStepNode.Style.IsVisible   = false;
        FetchStepNode.Style.IsVisible   = false;
        ErrorStepNode.Style.IsVisible   = false;

        QuerySelector("#author-name")!.NodeValue     = _release.Value.Author.Login;
        QuerySelector("#repository-name")!.NodeValue = $"{_author}/{_repo}";
        QuerySelector("#release-name")!.NodeValue    = _release.Value.Name;
    }

    private async Task DownloadAndInstall()
    {
        if (_release == null) return;
        
        ShowFetchStep();
        List<PluginEntry> entries = [];

        try {
            entries.AddRange(await PluginFetcher.DownloadRelease(_author, _repo, _release!.Value));
        } catch (Exception ex) {
            Logger.Error(ex.Message);
            ShowErrorStep();
            return;
        }

        if (entries.Count == 0) {
            ShowErrorStep();
            return;
        }
        
        foreach (var file in entries) {
            PluginRepository.AddEntry(file);
        }
        
        ShowInputStep();
    }
}
