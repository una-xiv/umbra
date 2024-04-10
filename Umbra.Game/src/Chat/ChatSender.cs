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
using Umbra.Common;

namespace Umbra.Game;

[Service]
internal sealed class ChatSender : IChatSender
{
    private static class Signatures
    {
        internal const string SendChat       = "48 89 5C 24 ?? 57 48 83 EC 20 48 8B FA 48 8B D9 45 84 C9";
        internal const string SanitiseString = "E8 ?? ?? ?? ?? EB 0A 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 48 8D 8D";
    }

    private delegate void ProcessChatBoxDelegate(IntPtr uiModule, IntPtr message, IntPtr unused, byte a4);

    private readonly ProcessChatBoxDelegate? _processChatBox;

    private readonly unsafe delegate* unmanaged<Utf8String*, int, IntPtr, void> _sanitizeString = null!;

    public ChatSender(ISigScanner sigScanner)
    {
        if (sigScanner.TryScanText(Signatures.SendChat, out var processChatBoxPtr)) {
            _processChatBox = Marshal.GetDelegateForFunctionPointer<ProcessChatBoxDelegate>(processChatBoxPtr);
        }

        unsafe {
            if (sigScanner.TryScanText(Signatures.SanitiseString, out var sanitisePtr)) {
                _sanitizeString = (delegate* unmanaged<Utf8String*, int, IntPtr, void>)sanitisePtr;
            }
        }
    }

    public void Send(string message)
    {
        try {
            var bytes = Encoding.UTF8.GetBytes(SanitizeText(message));
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
            var uiModule = (IntPtr)FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance()->GetUiModule();

            using var payload = new ChatPayload(message);
            var       mem1    = Marshal.AllocHGlobal(400);
            Marshal.StructureToPtr(payload, mem1, false);

            _processChatBox(uiModule, mem1, IntPtr.Zero, 0);

            Marshal.FreeHGlobal(mem1);
        } catch (Exception e) {
            Logger.Error($"Failed to send message (unsafe): {e.Message}");
        }
    }

    private unsafe string SanitizeText(string text)
    {
        if (_sanitizeString == null)
            throw new InvalidOperationException("Could not find signature for chat sanitisation");

        var uText = Utf8String.FromString(text);

        _sanitizeString(uText, 0x27F, IntPtr.Zero);
        var sanitised = uText->ToString();

        uText->Dtor();
        IMemorySpace.Free(uText);

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
