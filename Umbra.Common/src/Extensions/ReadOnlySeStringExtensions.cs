using Lumina.Text;
using Lumina.Text.ReadOnly;
using System;

namespace Umbra.Common.Extensions;

public static class ReadOnlySeStringExtensions
{
    public static ReadOnlySeString ReplaceText(this ReadOnlySeString rosss, ReadOnlySpan<byte> toFind, ReadOnlySpan<byte> replacement)
    {
        if (rosss.IsEmpty)
            return rosss;

        var sb = new SeStringBuilder();

        foreach (var payload in rosss)
        {
            if (payload.Type == ReadOnlySePayloadType.Invalid)
                continue;

            if (payload.Type != ReadOnlySePayloadType.Text)
            {
                sb.Append(payload);
                continue;
            }

            var index = payload.Body.Span.IndexOf(toFind);
            if (index == -1)
            {
                sb.Append(payload);
                continue;
            }

            sb.Append(new ReadOnlySpan<byte>([
                .. payload.Body.Span[..index],
                .. replacement,
                .. payload.Body.Span[(index + toFind.Length)..]
            ]));
        }

        return sb.ToReadOnlySeString();
    }
}
