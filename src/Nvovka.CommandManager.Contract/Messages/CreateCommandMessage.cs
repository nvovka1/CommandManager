namespace Nvovka.CommandManager.Contract.Messages;

public interface ICreateCommandMessage
{
    string Name { get; }
    string Description { get; }
}

public record CreateCommandMessage(string Name, string Description) : ICreateCommandMessage;