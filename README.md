# GGC2LanModule
Adds LAN multiplayer support to Guns, Gore and Cannoli 2.

- Star this repository to stay updated on new releases.
- This project is still in development, so bugs may occur - feel free to open an [issue](https://github.com/ggc2-lan-dev/ggc2-lanmodule/issues/new/choose) if you run into one.

> [!IMPORTANT]
> This is an unofficial fan-made module/mod for Guns, Gore and Cannoli 2, not affiliated with or endorsed by Rogueside.
> All game assets and trademarks belong to their respective owners.

## Installing the mod
### 1. Downloading files
1. Go to the [releases](https://github.com/ggc2-lan-dev/ggc2-lanmodule/releases) page, select the latest version, and download `GGC2LanModule.dll`.
2. Go to this [link](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.5) and download the `BepInEx` archive matching your system (default is `BepInEx_win_x64_5.4.23.5.zip` for **Windows**).

### 2. Installing on your computer
1. Go to the game's root folder `Guns, Gore and Cannoli 2\` and extract the contents of the `BepInEx` archive directly into it.
2. Launch the game once, wait for the main menu to load, then close the game (this initializes `BepInEx`).
3. Open the `Guns, Gore and Cannoli 2\BepInEx\plugins\` folder and place `GGC2LanModule.dll` there.

## How to connect
1. To play together over LAN, all players need to be on the same local Wi-Fi network. If a player is not physically with you, use [Radmin VPN](https://www.radmin-vpn.com) or similar tools and make sure everyone is connected within the app.
2. Launch the game and wait for the main menu to appear.
3. Click the `LAN` button. The first player should click `HOST LAN SERVER`, and the others should click `FIND LAN SERVER`.
4. You'll then see the familiar online lobby window. Enjoy playing with friends!

## Mod updates
1. Updates to the `GGC2LanModule.dll` file will be released on the [releases](https://github.com/ggc2-lan-dev/ggc2-lanmodule/releases) page. You just need to select the latest version, download it, and replace your current file with the updated one in the `Guns, Gore and Cannoli 2\BepInEx\plugins\` folder, and that's it.

## For developers
1. If you want to modify the code, you can download the repository as a `.zip` or clone it using the `git clone` command.
2. Set up your local game path once - see "Local development" below.
3. Build the plugin using **either**:
   - **Visual Studio**: open `GGC2LanModule/GGC2LanModule.csproj`, then build (Debug or Release).
   - **dotnet CLI** (no Visual Studio required): from the repo root, run `dotnet build GGC2LanModule/GGC2LanModule.csproj -c Debug` (or `-c Release`).
4. The compiled file is output to `GGC2LanModule/bin/Debug/GGC2LanModule.dll` (or `bin/Release/...` for a Release build).
5. Copy the compiled file into the game's root folder at `Guns, Gore and Cannoli 2\BepInEx\plugins\`.

### Local development
The project needs to find your own install of the game (and the BepInEx/Harmony DLLs inside it) via the `GamePath` MSBuild property. Don't edit `GGC2LanModule.csproj` directly for this - it's a tracked file shared by everyone. Instead, pick one:

- **Recommended - `GGC2LanModule.csproj.user`** (works identically in Visual Studio and the dotnet CLI): copy `GGC2LanModule/GGC2LanModule.csproj.user.example` to `GGC2LanModule/GGC2LanModule.csproj.user` and edit the `GamePath` value inside it. This file is already excluded by `.gitignore` (`*.user`), so your local path is never committed, and it's picked up automatically on every build - no further setup needed.
- **One-off CLI override**: pass it directly, e.g. `dotnet build GGC2LanModule/GGC2LanModule.csproj -p:GamePath="C:\path\to\game"`. Note: the game's own folder name contains a comma ("Guns, Gore and Cannoli 2"), and MSBuild's `-p:` parser splits plain commas in property values - if your path includes that comma, escape it as `%2c`, e.g. `-p:GamePath=C:\...\Guns%2c Gore and Cannoli 2`. The `.user` file above doesn't have this issue, so it's the easier option for everyday use.

If `GamePath` isn't set (or doesn't point at a real install), the build fails immediately with a clear error telling you how to fix it, instead of a cryptic "reference not found".

## CI and tests
- GitHub Actions (`.github/workflows/ci.yml`) builds the plugin on every push/PR using hand-written, copyright-free stub assemblies (`build/stubs/`) in place of the real game/BepInEx DLLs, as a compile-time smoke check. See `build/stubs/README.md` for details.
- Pushing a `vX.Y.Z` tag builds and attaches `GGC2LanModule.dll` to a new GitHub release automatically.
- `tests/GGC2LanModule.Tests/` contains unit tests for the plugin's pure logic (`GGC2LanModule/PluginLogic.cs`). Run with `dotnet test tests/GGC2LanModule.Tests/GGC2LanModule.Tests.csproj`.