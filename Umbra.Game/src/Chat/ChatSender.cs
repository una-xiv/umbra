using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace Umbra.Game;

[Service]
internal sealed class ChatSender : IChatSender
{
    public unsafe void Send(string message)
    {
        // Conversion from 0x27F:
        const AllowedEntities combinedEntities =
            AllowedEntities.UppercaseLetters |
            AllowedEntities.LowercaseLetters |
            AllowedEntities.Numbers |
            AllowedEntities.SpecialCharacters |
            AllowedEntities.CharacterList |
            AllowedEntities.OtherCharacters |
            AllowedEntities.Payloads |
            AllowedEntities.Unknown8 |
            AllowedEntities.Unknown9;

        using var msg = new Utf8String(message);
        msg.SanitizeString(combinedEntities, null);
        UIModule.Instance()->ProcessChatBoxEntry(&msg);
    }
}
