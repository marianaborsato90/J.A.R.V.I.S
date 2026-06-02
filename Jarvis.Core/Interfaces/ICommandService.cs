namespace Jarvis.Core.Interfaces;

/// <summary>
/// Contrato para o serviço que processa e executa comandos do usuário.
/// </summary>
public interface ICommandService
{
    /// <summary>
    /// Recebe um comando em texto e executa a ação correspondente.
    /// </summary>
    /// <param name="input">Comando digitado pelo usuário.</param>
    void Execute(string input);
}
