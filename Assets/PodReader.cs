using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PodReader
{
    private BinaryReader reader;

    public PodReader(string path)
    {
        reader = new BinaryReader(File.Open(path, FileMode.Open));
    }

    public void Dispose()
    {
        reader.Dispose();
    }

    public bool ReadMarker(ref uint name, ref uint len)
    {
        try { name = reader.ReadUInt32(); }
            catch { return false; }

        try { len = reader.ReadUInt32(); }
            catch { return false; }

        return true;
    }

    public bool ReadUInt (ref uint obj)
    {
        try { obj = reader.ReadUInt32(); }
            catch { return false; }

        return true; 
    }

    public bool ReadInt (ref int obj)
    {
        try { obj = reader.ReadInt32(); }
            catch { return false; }

        return true; 
    }

    public bool ReadShort (ref short obj)
    {
        try { obj = reader.ReadInt16(); }
            catch { return false; }

        return true; 
    }

    public bool ReadFloat (ref float obj)
    {
        try { obj = reader.ReadSingle(); }
            catch { return false; }

        return true; 
    }

    public bool ReadIntArray (ref int[] obj, uint len)
    {
        List<int> objs = new List<int>();

        for (int i = 0; i < len; i++)
        {
            int entry = 0;

            if (!ReadInt(ref entry))
            {
                return false;
            }

            objs.Add(entry);
        }

        obj = objs.ToArray();

        return true;
    }

    public bool ReadIntArrayAlloc (ref int[] obj, uint len)
    {
        List<int> objs = new List<int>();

        for (int i = 0; i < len / sizeof(int); i++)
        {
            int entry = 0;

            if (!ReadInt(ref entry))
            {
                return false;
            }

            objs.Add(entry);
        }

        obj = objs.ToArray();

        return true;
    }

    public bool ReadUIntArray (ref uint[] obj, uint len)
    {
        List<uint> objs = new List<uint>();

        for (int i = 0; i < len; i++)
        {
            uint entry = 0;

            if (!ReadUInt(ref entry))
            {
                return false;
            }

            objs.Add(entry);
        }

        obj = objs.ToArray();

        return true;
    }

    public bool ReadUIntArrayAlloc (ref uint[] obj, uint len)
    {
        List<uint> objs = new List<uint>();

        for (int i = 0; i < len / sizeof(uint); i++)
        {
            uint entry = 0;

            if (!ReadUInt(ref entry))
            {
                return false;
            }

            objs.Add(entry);
        }

        obj = objs.ToArray();

        return true;
    }

    public bool ReadShortArray (ref short[] obj, uint len)
    {
        List<short> objs = new List<short>();

        for (int i = 0; i < len; i++)
        {
            short entry = 0;

            if (!ReadShort(ref entry))
            {
                return false;
            }

            objs.Add(entry);
        }

        obj = objs.ToArray();

        return true;
    }

    public bool ReadFloatArray (ref float[] obj, uint len)
    {
        List<float> objs = new List<float>();

        for (int i = 0; i < len; i++)
        {
            float entry = 0f;

            if (!ReadFloat(ref entry))
            {
                return false;
            }

            objs.Add(entry);
        }

        obj = objs.ToArray();

        return true;
    }

    public bool ReadFloatArrayAlloc (ref float[] obj, uint len)
    {
        List<float> objs = new List<float>();

        for (int i = 0; i < len / sizeof(float); i++)
        {
            float entry = 0f;

            if (!ReadFloat(ref entry))
            {
                return false;
            }

            objs.Add(entry);
        }

        obj = objs.ToArray();

        return true;
    }

    public bool ReadString(ref string obj, uint len)
    {
        List<char> objs = new List<char>();

        for (int i = 0; i < len; i++)
        {
            char entry = default(char);

            if (!ReadChar(ref entry))
            {
                return false;
            }

            objs.Add(entry);
        }

        obj = new string(objs.ToArray());

        return true;
    }

    public bool ReadChar(ref char obj)
    {
        try { obj = reader.ReadChar(); }
            catch { return false; }

        return true;
    }

    public bool Skip(uint len)
    {
        for (int i = 0; i < len; i++)
        {
            try { reader.ReadByte(); }
                catch (Exception e) { Debug.LogError(e.ToString() + " " + len); return false; }
        }

        return true;
    }

    public bool ReadByte(ref byte obj)
    {
        try { obj = reader.ReadByte(); }
            catch { return false; }

        return true; 
    }

    public bool ReadByteArray (ref byte[] obj, uint len)
    {
        List<byte> objs = new List<byte>();

        for (int i = 0; i < len; i++)
        {
            byte entry = 0;

            if (!ReadByte(ref entry))
            {
                return false;
            }

            objs.Add(entry);
        }

        obj = objs.ToArray();

        return true;
    }
}
