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

public static class StdVectorExtensions
{
    public static List<T> ToList<T>(this StdVector<T> stdVector) where T : unmanaged
    {
        var  list = new List<T>();
        long size = stdVector.LongCount;

        unsafe {
            T* current = stdVector.First;

            for (long i = 0; i < size; i++) {
                list.Add(current[i]);
            }
        }

        return list;
    }
}
