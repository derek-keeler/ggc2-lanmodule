// Hand-written minimal stand-ins for the subset of the UnityEngine public
// API surface that GGC2LanModule.cs compiles against. These are NOT real
// Unity assemblies, contain no Unity code/assets, and exist purely so the
// plugin project can be compiled (not executed) in CI without redistributing
// any copyrighted game or engine binaries. See build/stubs/README.md.

namespace UnityEngine
{
    public class MonoBehaviour
    {
    }

    public struct Color
    {
        public float r, g, b, a;

        public Color(float r, float g, float b, float a = 1f)
        {
            this.r = r; this.g = g; this.b = b; this.a = a;
        }

        public static Color white => new Color(1f, 1f, 1f);
        public static Color black => new Color(0f, 0f, 0f);
        public static Color red => new Color(1f, 0f, 0f);
    }

    public struct Vector2
    {
        public float x, y;
        public Vector2(float x, float y) { this.x = x; this.y = y; }
    }

    public struct Rect
    {
        public float x, y, width, height;

        public Rect(float x, float y, float width, float height)
        {
            this.x = x; this.y = y; this.width = width; this.height = height;
        }

        public float xMax => x + width;
        public float yMax => y + height;

        public bool Contains(Vector2 point) =>
            point.x >= x && point.x <= xMax && point.y >= y && point.y <= yMax;
    }

    public enum FontStyle
    {
        Normal,
        Bold,
        Italic,
        BoldAndItalic,
    }

    public enum TextAnchor
    {
        UpperLeft, UpperCenter, UpperRight,
        MiddleLeft, MiddleCenter, MiddleRight,
        LowerLeft, LowerCenter, LowerRight,
    }

    public class GUIStyleState
    {
        public Color textColor;
    }

    public class GUIStyle
    {
        public Font font;
        public FontStyle fontStyle;
        public TextAnchor alignment;
        public int fontSize;
        public GUIStyleState normal = new GUIStyleState();

        public static GUIStyle none => new GUIStyle();

        public GUIStyle() { }

        public GUIStyle(GUIStyle other)
        {
            if (other == null) return;
            font = other.font;
            fontStyle = other.fontStyle;
            alignment = other.alignment;
            fontSize = other.fontSize;
            normal = new GUIStyleState { textColor = other.normal.textColor };
        }
    }

    public class GUIContent
    {
        public static GUIContent none => new GUIContent();
    }

    public class Texture2D
    {
        public static Texture2D whiteTexture => new Texture2D();
    }

    public static class GUI
    {
        public static Color color { get; set; }

        public static void DrawTexture(Rect position, Texture2D image) { }

        public static bool Button(Rect position, GUIContent content, GUIStyle style) => false;

        public static void Label(Rect position, string text, GUIStyle style) { }
    }

    public static class Screen
    {
        public static int width => 1920;
        public static int height => 1080;
    }

    public static class Mathf
    {
        public static float Min(float a, float b) => a < b ? a : b;

        public static int RoundToInt(float f) => (int)System.Math.Round(f, System.MidpointRounding.AwayFromZero);

        public static bool Approximately(float a, float b) => System.Math.Abs(a - b) < 0.00001f;
    }

    public class Font
    {
        public static string[] GetOSInstalledFontNames() => new string[0];

        public static Font CreateDynamicFontFromOSFont(string fontName, int size) => new Font();
    }

    public static class Resources
    {
        public static T Load<T>(string path) where T : class => null;
    }

    public class Event
    {
        public Vector2 mousePosition;

        public static Event current => new Event();
    }

    namespace SceneManagement
    {
        public struct Scene
        {
            public bool isLoaded;
        }

        public static class SceneManager
        {
            public static Scene GetSceneByName(string name) => new Scene();
        }
    }
}
