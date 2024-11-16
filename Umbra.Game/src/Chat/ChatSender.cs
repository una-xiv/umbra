/* Umbra.Game | (c) 2024 by Una         ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Game is free software: you can          \/     \/             \/
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General Public License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Game is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System;
using System.Runtime.InteropServices;
using System.Text;
using Dalamud.Game;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using Umbra.Common;

namespace Umbra.Game;

[Service]
internal sealed class ChatSender : IChatSender
{
    private static class Signatures
    {
        internal const string SendChat       = "48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC ?? 48 8B F2 48 8B F9 45 84 C9";
    }

    private delegate void ProcessChatBoxDelegate(IntPtr uiModule, IntPtr message, IntPtr unused, byte a4);

    private readonly ProcessChatBoxDelegate? _processChatBox;

    public ChatSender(ISigScanner sigScanner)
    {
        if (sigScanner.TryScanText(Signatures.SendChat, out var processChatBoxPtr)) {
            _processChatBox = Marshal.GetDelegateForFunctionPointer<ProcessChatBoxDelegate>(processChatBoxPtr);
        }
    }

    public void Send(string message)
    {
        try {
            var bytes = Encoding.UTF8.GetBytes(SanitizeString(message));
            if (bytes.Length == 0) throw new ArgumentException("The message is empty", nameof(message));

            if (bytes.Length > 500)
                throw new ArgumentException("The message is longer than 500 bytes", nameof(message));

            SendMessageUnsafe(bytes);
        } catch (Exception e) {
            Logger.Error($"Failed to send message: {e.Message}");
        }
    }

    private unsafe void SendMessageUnsafe(byte[] message)
    {
        if (_processChatBox == null) throw new InvalidOperationException("Could not find signature for chat sending");

        try {
            var uiModule = (IntPtr)UIModule.Instance();

            using var payload = new ChatPayload(message);
            var       mem1    = Marshal.AllocHGlobal(400);
            Marshal.StructureToPtr(payload, mem1, false);

            _processChatBox(uiModule, mem1, IntPtr.Zero, 0);

            Marshal.FreeHGlobal(mem1);
        } catch (Exception e) {
            Logger.Error($"Failed to send message (unsafe): {e.Message}");
        }
    }

    private static unsafe string SanitizeString(string str)
    {
        var uText = Utf8String.FromString(str);

        uText->SanitizeString( 0x27F, (Utf8String*)nint.Zero);
        var sanitised = uText->ToString();
        uText->Dtor(true);

        return sanitised;
    }

    [StructLayout(LayoutKind.Explicit)]
    private readonly struct ChatPayload : IDisposable
    {
        [FieldOffset(0)]  private readonly IntPtr textPtr;
        [FieldOffset(16)] private readonly ulong  textLen;
        [FieldOffset(8)]  private readonly ulong  unk1;
        [FieldOffset(24)] private readonly ulong  unk2;

        internal ChatPayload(byte[] stringBytes)
        {
            textPtr = Marshal.AllocHGlobal(stringBytes.Length + 30);
            Marshal.Copy(stringBytes, 0, textPtr, stringBytes.Length);
            Marshal.WriteByte(textPtr + stringBytes.Length, 0);

            textLen = (ulong)(stringBytes.Length + 1);

            unk1 = 64;
            unk2 = 0;
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(textPtr);
        }
    }
}
