using Jarvis.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Jarvis.Core.Services;

public class CommandService : ICommandService {
    private readonly ILogger<CommandService> _logger;
    private readonly IApplicationLauncher _launcher;

    public CommandService(ILogger<CommandService> logger, IApplicationLauncher launcher) {
        _logger = logger;
        _launcher = launcher;
    }

    public void Execute(string input) {
        _logger.LogInformation("Comando recebido: {Input}", input);

        var normalizedInput = NormalizeInput(input);

        if (TryResolveApplicationCommand(normalizedInput, out var appKey)) {
            _launcher.OpenApplication(appKey);
            return;
        }

        switch (normalizedInput) {
            case "ajuda":
                ShowHelp();
                break;

            default:
                Respond($"Comando não reconhecido: '{input}'. Digite 'ajuda' para ver os comandos disponíveis.");
                break;
        }
    }

    private static string NormalizeInput(string input) {
        return string.Join(' ', input
            .Trim()
            .ToLowerInvariant()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }

    private static bool TryResolveApplicationCommand(string input, out string appKey) {
        appKey = string.Empty;

        var knownApps = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "discord",
            "steam",
            "spotify",
            "chrome"
        };

        if (knownApps.Contains(input)) {
            appKey = input;
            return true;
        }

        var words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (words.Length < 2) {
            return false;
        }

        var verbs = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "abrir",
            "abre",
            "open",
            "iniciar"
        };

        if (!verbs.Contains(words[0])) {
            return false;
        }

        var targetIndex = words.Length > 2 && (words[1] is "o" or "a" or "os" or "as") ? 2 : 1;
        if (targetIndex >= words.Length) {
            return false;
        }

        var candidate = words[targetIndex];
        if (!knownApps.Contains(candidate)) {
            return false;
        }

        appKey = candidate;
        return true;
    }

    private static void ShowHelp() {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n📋 Comandos disponíveis:");
        Console.WriteLine("  ajuda              → Exibe esta mensagem");
        Console.WriteLine("  discord            → Abre o Discord (ou 'abre o discord')");
        Console.WriteLine("  steam              → Abre o Steam (ou 'abre o steam')");
        Console.WriteLine("  spotify            → Abre o Spotify (ou 'abre o spotify')");
        Console.WriteLine("  chrome             → Abre o Chrome (ou 'abre o chrome')");
        Console.WriteLine("  sair / exit        → Encerra o J.A.R.V.I.S");
        Console.ResetColor();
    }

    private static void Respond(string message) {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"JARVIS: {message}");
        Console.ResetColor();
    }
}