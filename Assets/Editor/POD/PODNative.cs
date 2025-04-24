using System.Runtime.InteropServices;
using UnityEngine;

public static unsafe class PODNative
{
    [DllImport("libpvrt")]
    public static extern SPODScene* load_pod(char* path);

    [DllImport("libpvrt")]
    public static extern SPODScene* load_pod_deinterleaved(char* path);

    [DllImport("libpvrt")]
    public static extern void destroy_pod(SPODScene* scene);
}