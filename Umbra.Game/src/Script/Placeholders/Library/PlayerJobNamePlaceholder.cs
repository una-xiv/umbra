using Umbra.Common;

namespace Umbra.Game.Script;

[Service]
internal class PlayerJobNamePlaceholder(IPlayer player) : ScriptPlaceholder(
    "player.job.name",
    "The name of the currently active job."
)
{
    [OnTick]
    public void Update()
    {
        Value = player.GetJobInfo(player.JobId).Name;
    }
}
