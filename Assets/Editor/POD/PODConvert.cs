using UnityEngine;

public static unsafe class PODConvert
{
    public static Vector2 ToUnityVector2(float* f, uint startingIndex)
    {
        return new Vector2(f[startingIndex], -f[startingIndex + 1]);
    }

    public static Vector3 ToUnityVector3(float* f, uint startingIndex)
    {
        return new Vector3(f[startingIndex], f[startingIndex + 1], f[startingIndex + 2]);
    }

    public static Quaternion ToUnityQuaternion(float* f, uint startingIndex)
    {
        return new Quaternion(-f[startingIndex], -f[startingIndex + 1], -f[startingIndex + 2], f[startingIndex + 3]);
    }

    public static Vector4 ToUnityVector4(float* f, uint startingIndex)
    {
        return new Vector4(f[startingIndex], f[startingIndex + 1], f[startingIndex + 2], f[startingIndex + 3]);
    }

    public static Matrix4x4 ToUnityMatrix(float* f, uint startingIndex)
    {
        return new Matrix4x4
        (
            new Vector4(f[startingIndex], f[startingIndex + 1], f[startingIndex + 2], f[startingIndex + 3]),
            new Vector4(f[startingIndex + 4], f[startingIndex + 5], f[startingIndex + 6], f[startingIndex + 7]),
            new Vector4(f[startingIndex + 8], f[startingIndex + 9], f[startingIndex + 10], f[startingIndex + 11]),
            new Vector4(f[startingIndex + 12], f[startingIndex + 13], f[startingIndex + 14], f[startingIndex + 15])
        );
    }
}
