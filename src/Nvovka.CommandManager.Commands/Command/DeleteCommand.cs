using MediatR;

namespace Nvovka.CommandManager.Commands.Command;
public record DeleteCommand(int Id) : IRequest<int>;
