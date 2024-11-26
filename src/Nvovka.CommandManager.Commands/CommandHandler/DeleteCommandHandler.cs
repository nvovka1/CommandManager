using MediatR;
using Nvovka.CommandManager.Commands.Command;
using Nvovka.CommandManager.Contract.Models;
using Nvovka.CommandManager.Data;

namespace Nvovka.CommandManager.Commands.CommandHandler;

public class DeleteCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteCommand, int>
{
    public async Task<int> Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        var repository = unitOfWork.GetRepository<CommandItem>();
        var item  = await repository.GetByIdAsync(request.Id);
        await repository.DeleteAsync(item);
        return await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}