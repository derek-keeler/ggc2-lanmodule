// Hand-written minimal stand-in for the subset of UnityEngine.Networking
// (UNET) used by GGC2LanModule.cs. See build/stubs/README.md.

namespace UnityEngine.Networking
{
    public class NetworkClient
    {
        public bool isConnected;

        public int GetRTT() => 0;
    }
}
