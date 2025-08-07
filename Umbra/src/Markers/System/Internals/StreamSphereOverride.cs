namespace Umbra.Markers.System;

[Service]
internal class StreamSphereOverride(IPlayer player)
{
    /// <summary>
    /// Overrides the collision stream sphere to be larger. This ensures that
    /// raycast operations across larger distances are more accurate.
    /// </summary>
    [OnTick]
    public unsafe void OverrideCollisionStreamSphere()
    {
        if (player.IsBetweenAreas || player.IsInQuestEvent) return;

        var fw = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance();
        if (fw == null) return;

        var bc = fw->BGCollisionModule;
        if (bc == null) return;

        bc->ForcedStreamingSphere.W = 4000;
    }
}
