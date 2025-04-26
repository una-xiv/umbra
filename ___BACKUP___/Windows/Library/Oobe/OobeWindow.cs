/* Umbra | (c) 2024 by Una              ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra is free software: you can redistribute  \/     \/             \/
 *     it and/or modify it under the terms of the GNU Affero General Public
 *     License as published by the Free Software Foundation, either version 3
 *     of the License, or (at your option) any later version.
 *
 *     Umbra UI is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Umbra.Common;
using Umbra.Windows.Oobe.Steps;

namespace Umbra.Windows.Oobe;

/// <summary>
/// Out-of-Box Experience window.
/// </summary>
internal partial class OobeWindow : Window
{
    protected override Vector2 MinSize     { get; } = new(720, 500);
    protected override Vector2 MaxSize     { get; } = new(720, 500);
    protected override Vector2 DefaultSize { get; } = new(720, 500);
    protected override string  Title       { get; } = I18N.Translate("OOBE.Title");

    private List<IOobeStep> Steps       { get; } = [];
    private IOobeStep?      CurrentStep { get; set; }
    private IOobeStep?      FirstStep   { get; }
    private IOobeStep?      LastStep    { get; }
    private bool            IsFirstTime { get; set; }

    public OobeWindow()
    {
        AddStep(new WelcomeStep());
        AddStep(new ToolbarWidgetsStep());
        AddStep(new ToolbarBehaviorStep());
        AddStep(new FinishStep());

        FirstStep = Steps.First();
        LastStep  = Steps.Last();

        NextButton.OnMouseUp     += _ => OnClickNext();
        PreviousButton.OnMouseUp += _ => OnClickPrevious();
        FinishButton.OnMouseUp   += _ => OnClickFinish();
        CancelButton.OnMouseUp   += _ => OnClickCancel();
    }

    protected override void OnOpen()
    {
        DontShowAgain.Style.IsVisible =  ConfigManager.Get<bool>("IsFirstTimeStart");
        DontShowAgain.OnValueChanged  += b => { ConfigManager.Set("IsFirstTimeStart", !b); };
        IsFirstTime                   =  ConfigManager.Get<bool>("IsFirstTimeStart");
    }

    protected override void OnUpdate(int instanceId)
    {
        foreach (var step in Steps) {
            step.Node.Style.IsVisible = CurrentStep == step;
        }

        HeaderTitleNode.NodeValue     = CurrentStep?.Title ?? "";
        HeaderSubtitleNode.NodeValue  = CurrentStep?.Description ?? "";
        NextButton.IsDisabled         = !CurrentStep?.CanContinue ?? false;
        FinishButton.IsDisabled       = !CurrentStep?.CanContinue ?? false;
        PreviousButton.IsDisabled     = CurrentStep == FirstStep;
        CancelButton.IsDisabled       = CurrentStep == LastStep;
        NextButton.Style.IsVisible    = CurrentStep != LastStep;
        FinishButton.Style.IsVisible  = CurrentStep == LastStep;
        DontShowAgain.Style.IsVisible = IsFirstTime && CurrentStep == FirstStep;
    }

    protected override void OnClose() { }

    private void AddStep(IOobeStep step)
    {
        Steps.Add(step);
        CurrentStep ??= step;

        step.Node.Style.IsVisible = CurrentStep == step;
        BodyNode.AppendChild(step.Node);
    }

    private void OnClickNext()
    {
        if (CurrentStep is null || CurrentStep == LastStep) return;

        int currentIndex = Steps.IndexOf(CurrentStep);
        CurrentStep.OnCommit();
        CurrentStep = Steps[currentIndex + 1];
    }

    private void OnClickPrevious()
    {
        if (CurrentStep is null || CurrentStep == FirstStep) return;

        int currentIndex = Steps.IndexOf(CurrentStep);
        CurrentStep = Steps[currentIndex - 1];
    }

    private void OnClickFinish()
    {
        if (CurrentStep != LastStep) return;

        CurrentStep?.OnCommit();
        ConfigManager.Set("IsFirstTimeStart", false);
        Close();
    }

    private void OnClickCancel()
    {
        if (DontShowAgain.Value) {
            // Save the user's preference to not show the OOBE again.
            ConfigManager.Set("IsFirstTimeStart", false);
        }

        Close();
    }
}
