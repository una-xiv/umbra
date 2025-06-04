using Umbra.Common;

namespace Umbra.Game.Script;

[Service]
internal class PlayerJobAbbrPlaceholder(IPlayer player) : ScriptPlaceholder(
    "player.job.abbr",
    "The abbreviation of the currently active job."
)
{
    [OnTick]
    public void Update()
    {
        Value = player.GetJobInfo(player.JobId).Abbreviation;
    }
}
