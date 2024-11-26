
using Nvovka.CommandManager.Contract.Enums;

namespace Nvovka.CommandManager.Contract.Messages;

public interface IUpdateCommandStatusMessage
{
    public int Id { get; set; }
    public CommandStatus Status { get; set; }
}

public record UpdateCommandStatusMessage(int Id, CommandStatus Status);
