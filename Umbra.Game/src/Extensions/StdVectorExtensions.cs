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
