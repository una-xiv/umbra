using System.Collections.Generic;

namespace Umbra.Game;

public interface ITravelDestinationRepository
{
    public List<TravelDestination> Destinations { get; }

    internal void Sync();
}
