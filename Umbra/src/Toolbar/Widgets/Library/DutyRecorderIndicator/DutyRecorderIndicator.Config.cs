using System.Collections.Generic;
using Umbra.Common;

namespace Umbra.Widgets.Library.DutyRecorderIndicator;

internal sealed partial class DutyRecorderIndicator
{
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            ..DefaultIconToolbarWidgetConfigVariables,
        ];
    }
}
