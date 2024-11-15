using Lumina.Excel;

namespace Umbra.Common;

public static class ExcelSheetExtensions
{
    /// <summary>
    /// Finds and returns the row with the given ID if it exists in the ExcelSheet.
    /// </summary>
    /// <typeparam name="T">The type of the rows in the ExcelSheet.</typeparam>
    /// <param name="sheet">The ExcelSheet instance to search in.</param>
    /// <param name="id">The ID of the row to find.</param>
    /// <returns>The row with the given ID if found; otherwise, null.</returns>
    public static T? FindRow<T>(this ExcelSheet<T> sheet, uint id) where T : struct, IExcelRow<T>
    {
        if (sheet.HasRow(id)) {
            return sheet.GetRow(id);
        }

        return null;
    }
}
