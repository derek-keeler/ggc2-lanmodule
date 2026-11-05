// Pure, Unity/BepInEx/game-assembly-independent helper logic extracted
// from GGC2LanModule so it can be unit-tested without a Unity/game
// runtime. Keep this file free of UnityEngine/BepInEx/Assembly-CSharp
// dependencies - the test project links it directly as a plain C# file
// (see tests/GGC2LanModule.Tests), with no stub assemblies involved.
internal static class PluginLogic
{
    // Mirrors UnityEngine.Mathf.Min(float, float) exactly (a < b ? a : b),
    // not System.Math.Min, so results match what the live plugin computes.
    public static float ComputeScale(float screenWidth, float screenHeight, float refWidth, float refHeight)
    {
        float sx = screenWidth / refWidth;
        float sy = screenHeight / refHeight;
        return sx < sy ? sx : sy;
    }

    // Caps the displayed ping at "999+ms" instead of growing the label
    // unboundedly for very high/stalled round-trip times.
    public static string FormatPing(int rttMs)
    {
        return (rttMs > 999 ? "999+" : rttMs.ToString()) + "ms";
    }
}
