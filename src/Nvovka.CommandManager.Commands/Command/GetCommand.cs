using MediatR;
using Nvovka.CommandManager.Contract.Models;

namespace Nvovka.CommandManager.Commands.Command;

public record GetCommand(int Id) : IRequest<CommandItem>;
