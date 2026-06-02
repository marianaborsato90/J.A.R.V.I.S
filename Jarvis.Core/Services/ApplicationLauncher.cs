using System;
using System.Diagnostics;
using System.Globalization;

namespace Jarvis.Core.Services;

public class ApplicationLauncher : IApplicationLauncher {
    private readonly Dictionary<string, List<AppLaunchTarget>> _applicationTargets = new(StringComparer.OrdinalIgnoreCase)
    {
        {
            "discord",
            BuildTargets("discord", "Discord", "discord.exe")
        },
        {
            "steam",
            BuildTargets("steam", "Steam", "steam.exe")
        },
        {
            "spotify",
            BuildTargets("spotify", "Spotify", "spotify.exe")
        },
        {
            "chrome",
            BuildTargets("chrome", "Chrome", "chrome.exe")
        }
    };

    public void OpenDiscord() {
        LaunchApplication("discord", "Discord");
    }

    public void OpenSteam() {
        LaunchApplication("steam", "Steam");
    }

    public void OpenSpotify() {
        LaunchApplication("spotify", "Spotify");
    }

    public void OpenChrome(string? url = null) {
        LaunchApplication("chrome", "Chrome", url);
    }

    public void OpenApplication(string applicationName) {
        var key = applicationName.Trim().ToLowerInvariant();
        if (_applicationTargets.ContainsKey(key)) {
            LaunchApplication(key, ToDisplayName(key));
        }
    }

    private void LaunchApplication(string key, string displayName, string? extraArguments = null) {
        if (!_applicationTargets.TryGetValue(key, out var targets)) {
            Console.WriteLine($"✗ Aplicativo desconhecido: {displayName}.");
            return;
        }

        foreach (var target in targets) {
            if (!CanUseTarget(target)) {
                continue;
            }

            var args = CombineArgs(target.Arguments, extraArguments);
            if (TryStart(target.FileName, args)) {
                Console.WriteLine($"✓ {displayName} aberto com sucesso!");
                return;
            }
        }

        Console.WriteLine($"✗ {displayName} não foi encontrado no sistema.");
    }

    private static List<AppLaunchTarget> BuildTargets(string key, string appName, string exeName) {
        var targets = new List<AppLaunchTarget>();

        foreach (var knownPath in GetKnownPaths(key, appName, exeName)) {
            targets.Add(new AppLaunchTarget(knownPath));
        }

        if (key.Equals("discord", StringComparison.OrdinalIgnoreCase)) {
            var updateExe = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Discord", "Update.exe");
            targets.Add(new AppLaunchTarget(updateExe, "--processStart Discord.exe"));

            var appFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Discord");
            if (Directory.Exists(appFolder)) {
                var newestDiscordExe = Directory.GetDirectories(appFolder, "app-*")
                    .Select(dir => Path.Combine(dir, "Discord.exe"))
                    .Where(File.Exists)
                    .OrderByDescending(path => path, StringComparer.OrdinalIgnoreCase)
                    .FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(newestDiscordExe)) {
                    targets.Insert(0, new AppLaunchTarget(newestDiscordExe));
                }
            }
        }

        // Shell fallback lets Windows resolve apps in PATH or registered handlers.
        targets.Add(new AppLaunchTarget(exeName, RequiresExistingFile: false));
        return targets;
    }

    private static IEnumerable<string> GetKnownPaths(string key, string appName, string exeName) {
        var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        var programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var roamingAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        var defaultPaths = new[]
        {
            Path.Combine(programFiles, appName, exeName),
            Path.Combine(programFilesX86, appName, exeName),
            Path.Combine(localAppData, appName, exeName),
            Path.Combine(roamingAppData, appName, exeName)
        };

        foreach (var path in defaultPaths) {
            yield return path;
        }

        if (key.Equals("chrome", StringComparison.OrdinalIgnoreCase)) {
            yield return Path.Combine(programFiles, "Google", "Chrome", "Application", "chrome.exe");
            yield return Path.Combine(programFilesX86, "Google", "Chrome", "Application", "chrome.exe");
            yield return Path.Combine(localAppData, "Google", "Chrome", "Application", "chrome.exe");
        }

        if (key.Equals("steam", StringComparison.OrdinalIgnoreCase)) {
            yield return Path.Combine(programFilesX86, "Steam", "steam.exe");
            yield return Path.Combine(programFiles, "Steam", "steam.exe");
        }

        if (key.Equals("spotify", StringComparison.OrdinalIgnoreCase)) {
            yield return Path.Combine(roamingAppData, "Spotify", "Spotify.exe");
            yield return Path.Combine(localAppData, "Microsoft", "WindowsApps", "Spotify.exe");
        }
    }

    private static bool CanUseTarget(AppLaunchTarget target) {
        return !target.RequiresExistingFile || File.Exists(target.FileName);
    }

    private static string? CombineArgs(string? baseArgs, string? extraArgs) {
        if (string.IsNullOrWhiteSpace(baseArgs)) {
            return string.IsNullOrWhiteSpace(extraArgs) ? null : extraArgs;
        }

        if (string.IsNullOrWhiteSpace(extraArgs)) {
            return baseArgs;
        }

        return $"{baseArgs} {extraArgs}";
    }

    private static bool TryStart(string fileName, string? arguments) {
        try {
            var startInfo = new ProcessStartInfo {
                FileName = fileName,
                UseShellExecute = true
            };

            if (!string.IsNullOrWhiteSpace(arguments)) {
                startInfo.Arguments = arguments;
            }

            Process.Start(startInfo);
            return true;
        }
        catch {
            return false;
        }
    }

    private static string ToDisplayName(string key) {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(key);
    }

    private sealed record AppLaunchTarget(string FileName, string? Arguments = null, bool RequiresExistingFile = true);
}