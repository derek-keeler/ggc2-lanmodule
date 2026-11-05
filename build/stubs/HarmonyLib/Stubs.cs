// Hand-written minimal stand-in for the subset of Harmony (0Harmony.dll)
// used by GGC2LanModule.cs. See build/stubs/README.md.

namespace HarmonyLib
{
    public class Harmony
    {
        public Harmony(string id) { }

        public void PatchAll() { }
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class HarmonyPatch : System.Attribute
    {
        public HarmonyPatch(System.Type type, string methodName) { }
    }
}
