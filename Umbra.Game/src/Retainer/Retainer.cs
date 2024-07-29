using System;

namespace Umbra.Game.Retainer;

public readonly struct Retainer
{
    public string    Name                { get; init; }
    public JobInfo?  Job                 { get; init; }
    public DateTime? VentureCompleteTime { get; init; }
    public DateTime? MarketExpiresAt     { get; init; }
    public uint      Gil                 { get; init; }
    public uint      MarketItemCount     { get; init; }
    public uint      ItemCount           { get; init; }
}
