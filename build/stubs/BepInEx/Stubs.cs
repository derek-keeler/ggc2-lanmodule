// Hand-written minimal stand-in for the subset of BepInEx.dll used by
// GGC2LanModule.cs. See build/stubs/README.md.

namespace BepInEx
{
    namespace Logging
    {
        public class ManualLogSource
        {
            public void LogInfo(object data) { }
            public void LogWarning(object data) { }
            public void LogError(object data) { }
        }
    }

    public class BaseUnityPlugin : UnityEngine.MonoBehaviour
    {
        public Logging.ManualLogSource Logger { get; } = new Logging.ManualLogSource();
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class BepInPlugin : System.Attribute
    {
        public BepInPlugin(string guid, string name, string version) { }
    }
}
