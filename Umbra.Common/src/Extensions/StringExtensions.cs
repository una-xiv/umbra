using System.Buffers;

namespace Umbra.Common.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Removes all non-alphanumeric characters from the string and replaces
    /// them with the given delimiter (underscore by default).
    /// The returned string will never start or end with the delimiter, nor
    /// can a delimiter occur twice in a row.
    /// </summary>
    /// <returns></returns>
    public static string Slugify(this string str, char delimiter = '_')
    {
        if (string.IsNullOrEmpty(str)) return string.Empty;

        char[]     rentedBuffer = ArrayPool<char>.Shared.Rent(str.Length);
        Span<char> buffer       = rentedBuffer.AsSpan();

        int  writeIndex             = 0;
        bool lastWrittenWasAlphaNum = false;

        for (int readIndex = 0; readIndex < str.Length; readIndex++) {
            char c = str[readIndex];

            if (char.IsLetterOrDigit(c)) {
                buffer[writeIndex++]   = c;    
                lastWrittenWasAlphaNum = true;
                continue;
            }
            
            if (lastWrittenWasAlphaNum && readIndex < str.Length - 1) {
                buffer[writeIndex++]   = delimiter;
                lastWrittenWasAlphaNum = false;
            }
        }

        string result = new string(buffer[..writeIndex]);
        ArrayPool<char>.Shared.Return(rentedBuffer);

        return result;
    }
}
