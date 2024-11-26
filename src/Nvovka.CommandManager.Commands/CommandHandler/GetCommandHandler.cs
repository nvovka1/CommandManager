using MediatR;
using Nvovka.CommandManager.Commands.Command;
using Nvovka.CommandManager.Contract.Models;
using Nvovka.CommandManager.Data;

namespace Nvovka.CommandManager.Commands.CommandHandler;

public class GetCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCommand, CommandItem>
{
    public async Task<CommandItem> Handle(GetCommand request, CancellationToken cancellationToken)
    {
        var repository = unitOfWork.GetRepository<CommandItem>();
        return await repository.GetByIdAsync(request.Id);
    }
}
