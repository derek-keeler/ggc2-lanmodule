// Hand-written minimal stand-ins for the subset of the game's own
// Assembly-CSharp public API surface that GGC2LanModule.cs compiles
// against (LobbyManagerGGC, NetworkDiscoveryManagerGGC, WindowManager,
// etc.). These are NOT the real game types - no game logic, assets, or
// copyrighted content is reproduced here, only the minimal method/property
// signatures needed to compile the plugin in CI. See build/stubs/README.md.
//
// The real game defines these types in the global namespace (the plugin
// source references them without any `using` for a game namespace), so
// this stub does the same.

using System;
using UnityEngine.Networking;

public class LobbyManagerGGC
{
    public enum State
    {
        MainMenu,
        Lobby,
        Connecting,
        InGame,
    }

    public enum DisconnectType
    {
        Unknown,
        Timeout,
        Kicked,
        Voluntary,
    }

    public static LobbyManagerGGC Instance { get; set; }

    public State state;
    public bool pingVisible;
    public NetworkClient client;
    public NetworkManagerHudGGC networkManagerHud;

    public void CreateMatch(bool isLan) { }

    public void ChangeState(State newState) { }

    public void DestroyMatch(DisconnectType reason) { }

    public static void ConnectViaIP(string ip) { }
}

public class NetworkManagerHudGGC
{
    public bool showGUI;
}

public class LobbyPlayerGGC
{
    public System.Collections.IEnumerator TestConnection()
    {
        yield break;
    }
}

public class DiscoveryBroadcastGGC
{
    public string ip;
}

public static class NetworkDiscoveryManagerGGC
{
    public static void StartListeningForNetworkMatches(Action<DiscoveryBroadcastGGC> onFound) { }

    public static void StopListeningForNetworkMatches() { }
}

public class Window
{
}

public class LobbyWindow : Window
{
    public LobbyWindow(bool isReturning) { }
}

public class WindowManager
{
    public static WindowManager Instance { get; set; }

    public void OpenWindow(Window window) { }
}

public static class MainMenuGameState
{
    public static bool MainMenuIsTransitional;
}

public class LevelTypeData
{
    public static LevelTypeData mainMenuScene;

    public string SceneName;
}

public static class NetworkCommunicator
{
    public static bool lobbyIsLockedForGameStart;
}
