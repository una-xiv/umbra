namespace Umbra.Game;

public interface IChatSender
{
    /// <summary>
    /// Sends out a chat message.
    /// </summary>
    /// <remarks>
    /// The message must be between 1 and 500 bytes long and will be sanitized before sending.
    /// </remarks>
    public void Send(string message);
}
