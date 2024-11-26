using MediatR;
using Nvovka.CommandManager.Commands.Command;
using Nvovka.CommandManager.Contract.Models;
using Nvovka.CommandManager.Data;
namespace Nvovka.CommandManager.Commands.CommandHandler;

public class DeleteAllCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteAllCommands, int>
{
    public async Task<int> Handle(DeleteAllCommands request, CancellationToken cancellationToken)
    {
        var repository = unitOfWork.GetRepository<CommandItem>();
        var items = await repository.GetAllAsync();
        foreach (var item in items)
        {
            await repository.DeleteAsync(item);
        }
        return await unitOfWork.SaveChangesAsync();
    }
}
