# CI build stubs

The real `GGC2LanModule.csproj` compiles against game/engine assemblies
(`Assembly-CSharp.dll`, `UnityEngine.dll`, `UnityEngine.Networking.dll`) and
BepInEx/Harmony core assemblies (`BepInEx.dll`, `0Harmony.dll`) that are
normally found inside a licensed install of *Guns, Gore and Cannoli 2*.
Those files are copyrighted and/or third-party binaries and must never be
committed to, or fetched by, this repository.

To let CI verify that the plugin still **compiles** without needing a real
game install, this folder contains small, hand-written C# stand-ins that
declare only the exact method/property signatures `ggc2.lanmodule.cs`
references - no decompiled code, no game logic, no original assets.

These stub assemblies are:
- **not** used to run or test any game behavior,
- **not** a substitute for testing against the real game,
- only a compile-time smoke check (equivalent to "does this still build").

The CI workflow (`.github/workflows/ci.yml`) builds these stub projects,
stages their output DLLs into a fake `GamePath` folder layout
(`ggc2_Data\Managed\*.dll`, `BepInEx\core\*.dll`), and then builds
`GGC2LanModule.csproj` against that fake path. Anyone with a real copy of
the game can ignore this folder entirely and build against their own
`GamePath` as described in the main README.

If you add a call to a new game/engine API in `ggc2.lanmodule.cs`, add the
matching minimal signature here (in the appropriate stub project) so CI
keeps compiling.
