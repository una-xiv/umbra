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

using System.Collections.Generic;
using FFXIVClientStructs.STD;

namespace Umbra.Game;
public static class StdListExtensions
{
    // public static List<T> ToList<T>(this StdList<T> stdList) where T : unmanaged
    // {
    //     var list = new List<T>();
    //
    //     unsafe
    //     {
    //         StdList<T>.Node* currentNode = stdList.Head;
    //         for (ulong i = 0; i < stdList.Size; i++)
    //         {
    //             list.Add(currentNode->Value);
    //             currentNode = currentNode->Next;
    //         }
    //     }
    //
    //     return list;
    // }
}
