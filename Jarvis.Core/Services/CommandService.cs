using Jarvis.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Jarvis.Core.Services;

public class CommandService : ICommandService
{
    private readonly ILogger<CommandService> _logger;

    public CommandService(ILogger<CommandService> logger)
    {
        _logger = logger;
    }

    public void Execute(string input)
    {
        _logger.LogInformation("Comando recebido: {Input}", input);

        switch (input)
        {
            case "ajuda":
                ShowHelp();
                break;

            default:
                Respond($"Comando não reconhecido: '{input}'. Digite 'ajuda' para ver os comandos disponíveis.");
                break;
        }
    }

    private static void ShowHelp()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n📋 Comandos disponíveis:");
        Console.WriteLine("  ajuda          → Exibe esta mensagem");
        Console.WriteLine("  sair / exit    → Encerra o J.A.R.V.I.S");
        Console.ResetColor();
    }

    private static void Respond(string message)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"JARVIS: {message}");
        Console.ResetColor();
    }
}
