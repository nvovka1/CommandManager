using Nvovka.CommandManager.Contract.Enums;

namespace Nvovka.CommandManager.Api.Dto;

public class CommandUpdateDto
{
    public int Id { get; set; }

    public CommandStatus Status { get; set; }
}
