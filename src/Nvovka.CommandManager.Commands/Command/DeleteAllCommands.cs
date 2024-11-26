using MediatR;

namespace Nvovka.CommandManager.Commands.Command;

public record DeleteAllCommands() : IRequest<int>;
