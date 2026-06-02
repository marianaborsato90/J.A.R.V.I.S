using Jarvis.Core.Interfaces;
using Jarvis.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// ─────────────────────────────────────────
// FASE 0 — Configuração e Infraestrutura
// ─────────────────────────────────────────

// 1. Configuração via appsettings.json
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// 2. Dependency Injection
var services = new ServiceCollection();

services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddConfiguration(configuration.GetSection("Logging"));
});

services.AddSingleton<IConfiguration>(configuration);
services.AddSingleton<ICommandService, CommandService>();
services.AddSingleton<IApplicationLauncher, ApplicationLauncher>();

var serviceProvider = services.BuildServiceProvider();

// ─────────────────────────────────────────
// FASE 1.1 — Loop Principal do Console
// ─────────────────────────────────────────

var commandService = serviceProvider.GetRequiredService<ICommandService>();
var applicationLauncher = serviceProvider.GetRequiredService<IApplicationLauncher>();
var jarvisConfig   = configuration.GetSection("Jarvis");

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("╔══════════════════════════════════════════════╗");
Console.WriteLine($"║   {jarvisConfig["Nome"]} — v{jarvisConfig["Versao"]}              ║");
Console.WriteLine("║   Online e operacional. Aguardando ordens.   ║");
Console.WriteLine("║   Digite 'ajuda' para ver os comandos.       ║");
Console.WriteLine("╚══════════════════════════════════════════════╝");
Console.ResetColor();

// Loop principal — lê comandos até o usuário digitar 'sair'
while (true)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write("\nJARVIS > ");
    Console.ResetColor();

    // Leitura do comando
    var input = Console.ReadLine()?.Trim().ToLower();

    if (string.IsNullOrWhiteSpace(input))
        continue;

    // Comando: sair
    if (input is "sair" or "exit")
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("JARVIS: Encerrando sistemas. Até logo.");
        Console.ResetColor();
        break;
    }

    // Delega o comando ao serviço
    commandService.Execute(input);
}

partial class Program 
{
    static void Main() 
    {
     Process.Start("notepad.exe");

    }
}