using System.Collections;
using System.Collections.Generic;

namespace Umbra.Game.Retainer;

public interface IRetainerRepository
{
    /// <summary>
    /// Requests the list of retainers from the server.
    /// </summary>
    /// <remarks>
    /// This function makes a request to the server. To stay in line with
    /// Dalamud's plugin guidelines, it MUST only be called after the user
    /// does a manual interaction. In our case, it is only called if and only
    /// if the user opens the retainer widget and the information is not
    /// already available. This function does absolutely nothing if either
    /// the information is already available, or if it is called a second time.
    /// </remarks>
    public void RequestRetainerList();

    /// <summary>
    /// Returns a list of retainers.
    /// </summary>
    public IEnumerable<Retainer> GetRetainers();
}
