using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

// Prepares a release locally: checks the repo state, runs the unit tests, bumps the version
// in AssemblyInfo.cs and the bug report template, builds the plugin against the developer's
// real GamePath, then commits "vX.Y.Z" and (unless --no-tag) tags it. Nothing is pushed -
// pushing the tag is what triggers the CI release job, so that stays a deliberate step.

const string VersionFile = "GGC2LanModule/Properties/AssemblyInfo.cs";
const string BugReportFile = ".github/ISSUE_TEMPLATE/bug_report.yml";
const string PluginProject = "GGC2LanModule/GGC2LanModule.csproj";
const string TestProject = "tests/GGC2LanModule.Tests/GGC2LanModule.Tests.csproj";
const string ReleaseBranch = "main";
const string Remote = "origin";

try
{
    return Run(args);
}
catch (ReleaseException ex)
{
    Console.Error.WriteLine($"\nRelease aborted: {ex.Message}");
    return 1;
}

static int Run(string[] args)
{
    bool noTag = args.Contains("--no-tag");
    string[] positional = args.Where(a => a != "--no-tag").ToArray();
    if (positional.Length != 1 || positional[0] is "-h" or "--help")
    {
        Console.WriteLine("Usage: dotnet run --project build/release -- <version|major|minor|patch> [--no-tag]");
        Console.WriteLine("  e.g. 1.8.0, v1.8.0, or a bump keyword relative to the current version.");
        Console.WriteLine("  --no-tag  commit only (any branch); tag the merged commit yourself later,");
        Console.WriteLine("            e.g. after a squash-merged pull request.");
        return positional.Length == 1 ? 0 : 1;
    }

    string root = Path.GetFullPath(Git("rev-parse --show-toplevel"));
    Directory.SetCurrentDirectory(root);

    string versionPath = Path.Combine(root, VersionFile);
    string bugReportPath = Path.Combine(root, BugReportFile);
    var versionPattern = new Regex(@"(public const string Version = "")(\d+\.\d+\.\d+)("";)");
    string source = ReadText(versionPath, out Encoding versionEncoding);
    Match match = versionPattern.Match(source);
    if (!match.Success) throw new ReleaseException($"Could not find the Version constant in {VersionFile}.");

    Version current = Version.Parse(match.Groups[2].Value);
    Version next = ResolveVersion(positional[0], current);
    string tag = $"v{next}";
    Console.WriteLine($"Releasing {tag} (current: v{current})");

    // 1. Repo state
    string branch = Git("rev-parse --abbrev-ref HEAD");
    if (!noTag && branch != ReleaseBranch)
        throw new ReleaseException($"Tagging is only allowed on '{ReleaseBranch}' (you are on '{branch}'). Switch branches or pass --no-tag.");
    if (Git("status --porcelain --untracked-files=no").Length > 0)
        throw new ReleaseException("The working tree has uncommitted changes to tracked files.");
    if (next <= current)
        throw new ReleaseException($"{tag} must be greater than the current version v{current}.");

    Step($"Checking '{Remote}' for newer commits and an existing {tag} tag");
    Exec("git", $"fetch {Remote} {ReleaseBranch} --quiet");
    if (int.Parse(Git($"rev-list --count HEAD..{Remote}/{ReleaseBranch}")) > 0)
        throw new ReleaseException($"'{branch}' is behind {Remote}/{ReleaseBranch}; pull or rebase first.");
    if (Git($"tag --list {tag}").Length > 0 || Git($"ls-remote --tags {Remote} refs/tags/{tag}").Length > 0)
        throw new ReleaseException($"Tag {tag} already exists.");

    // 2. Tests
    Step("Running unit tests");
    Exec("dotnet", $"test {TestProject} -c Release");

    // 3. Bump + real build. The bump is reverted if anything fails before the commit.
    string bugReport = ReadText(bugReportPath, out Encoding bugReportEncoding);
    File.WriteAllText(versionPath, versionPattern.Replace(source, $"${{1}}{next}${{3}}", 1), versionEncoding);
    File.WriteAllText(bugReportPath, AddBugReportVersion(bugReport, tag), bugReportEncoding);
    string dll = Path.Combine(root, "GGC2LanModule", "bin", "Release", "GGC2LanModule.dll");
    try
    {
        Step("Building the plugin against your local GamePath");
        Exec("dotnet", $"build {PluginProject} -c Release");

        string? built = File.Exists(dll) ? FileVersionInfo.GetVersionInfo(dll).ProductVersion : null;
        if (built != next.ToString())
            throw new ReleaseException($"Built DLL reports version '{built}', expected '{next}'.");

        Step(noTag ? "Committing" : "Committing and tagging");
        Exec("git", $"add {VersionFile} {BugReportFile}");
        Exec("git", $"commit -m {tag}");
    }
    catch
    {
        Exec("git", $"checkout -- {VersionFile} {BugReportFile}", check: false);
        throw;
    }

    if (noTag)
    {
        Console.WriteLine($"""

            Release commit for {tag} is ready locally (nothing has been pushed, no tag created).
              DLL:  {dll}

            1. Test the DLL in-game (copy it to <game>\BepInEx\plugins\).
            2. When satisfied, get this commit into {ReleaseBranch} via a pull request (branch '{branch}' is where it lives now).
            3. Tag the commit as it exists on {ReleaseBranch} (a squash merge creates a new commit, so don't tag the PR branch):
                 git fetch {Remote}; git tag -a {tag} -m {tag} {Remote}/{ReleaseBranch}; git push {Remote} {tag}
               The tag push triggers the CI release job.

            To discard instead:  git reset --hard HEAD~1
            """);
        return 0;
    }

    Exec("git", $"tag -a {tag} -m {tag}");
    Console.WriteLine($"""

        Release {tag} is ready locally (nothing has been pushed).
          DLL:  {dll}

        1. Test the DLL in-game (copy it to <game>\BepInEx\plugins\).
        2. When satisfied, publish - the tag push triggers the CI release job:
             git push {Remote} {ReleaseBranch} {tag}
        3. Optional: to attach the exact DLL you tested instead of the CI-built one, after CI creates the release:
             gh release upload {tag} "{dll}" --clobber

        To discard instead:  git tag -d {tag}; git reset --hard HEAD~1
        """);
    return 0;
}

// Adds the new version as the first option of the "Module version" dropdown.
static string AddBugReportVersion(string yaml, string tag)
{
    var dropdown = new Regex(@"(id: version-module\b[\s\S]*?options:[ \t]*(\r?\n))([ \t]+)- v\d");
    Match m = dropdown.Match(yaml);
    if (!m.Success) throw new ReleaseException($"Could not find the 'version-module' options in {BugReportFile}.");
    if (Regex.IsMatch(yaml, $@"^[ \t]+- {Regex.Escape(tag)}[ \t]*\r?$", RegexOptions.Multiline)) return yaml;
    string newline = m.Groups[2].Value;
    return yaml.Insert(m.Groups[3].Index, $"{m.Groups[3].Value}- {tag}{newline}");
}

// Reads a text file and reports its encoding so a UTF-8 BOM (or its absence) survives the rewrite.
static string ReadText(string path, out Encoding encoding)
{
    byte[] bytes = File.ReadAllBytes(path);
    encoding = new UTF8Encoding(bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF);
    return encoding.GetString(bytes).TrimStart('\uFEFF');
}
static Version ResolveVersion(string input, Version current) => input.ToLowerInvariant() switch
{
    "major" => new Version(current.Major + 1, 0, 0),
    "minor" => new Version(current.Major, current.Minor + 1, 0),
    "patch" => new Version(current.Major, current.Minor, current.Build + 1),
    var v when Regex.IsMatch(v, @"^v?\d+\.\d+\.\d+$") => Version.Parse(v.TrimStart('v')),
    _ => throw new ReleaseException($"'{input}' is not a version (X.Y.Z) or one of major/minor/patch."),
};

static void Step(string message) => Console.WriteLine($"\n==> {message}");

static string Git(string arguments)
{
    var psi = new ProcessStartInfo("git", arguments) { RedirectStandardOutput = true, RedirectStandardError = true };
    using var process = Process.Start(psi) ?? throw new ReleaseException("Could not start git.");
    string output = process.StandardOutput.ReadToEnd().Trim();
    string error = process.StandardError.ReadToEnd().Trim();
    process.WaitForExit();
    if (process.ExitCode != 0) throw new ReleaseException($"git {arguments} failed: {error}");
    return output;
}

static void Exec(string file, string arguments, bool check = true)
{
    using var process = Process.Start(new ProcessStartInfo(file, arguments))
        ?? throw new ReleaseException($"Could not start {file}.");
    process.WaitForExit();
    if (check && process.ExitCode != 0)
        throw new ReleaseException($"'{file} {arguments}' failed with exit code {process.ExitCode}.");
}

class ReleaseException(string message) : Exception(message);
