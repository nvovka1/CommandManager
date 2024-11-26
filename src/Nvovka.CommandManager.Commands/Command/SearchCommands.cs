using MediatR;
using Nvovka.CommandManager.Contract.Models;

namespace Nvovka.CommandManager.Commands.Command;

public record SearchCommands() : IRequest<IReadOnlyCollection<CommandItem>>;

