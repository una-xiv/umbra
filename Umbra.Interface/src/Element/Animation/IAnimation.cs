namespace Umbra.Interface;

public interface IAnimation
{
    /// <summary>
    /// Assigns the given element to the animation instance.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="playOnce"></param>
    public void Assign(Element element, bool playOnce = true);

    /// <summary>
    /// Advances the animation.
    /// </summary>
    /// <returns>True if the animation should keep advancing in the next frame, false otherwise.</returns>
    internal bool Advance();
}
