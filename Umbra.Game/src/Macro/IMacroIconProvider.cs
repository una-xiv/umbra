namespace Umbra.Game.Macro;

public interface IMacroIconProvider
{
    /// <summary>
    /// Returns an icon ID of a macro.
    /// </summary>
    /// <param name="set">0 = individual, 1 = shared.</param>
    /// <param name="macroIndex">The macro index, ranged from 0 to 99.</param>
    /// <returns>Icon ID</returns>
    public uint GetIconIdForMacro(byte set, uint macroIndex);
}
