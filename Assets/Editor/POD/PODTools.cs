using UnityEngine;

public static unsafe class PODTools
{
    public static uint GetIndex(uint index, uint* i, uint len)
    {
        if (i == null)
        {
            return index * len;
        }

        return i[index];
    }
}
