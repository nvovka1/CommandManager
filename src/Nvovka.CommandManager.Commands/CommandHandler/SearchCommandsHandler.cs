using MediatR;
using Nvovka.CommandManager.Commands.Command;
using Nvovka.CommandManager.Contract.Models;
using Nvovka.CommandManager.Data;

namespace Nvovka.CommandManager.Commands.CommandHandler;

public class SearchCommandsHandler(IUnitOfWork unitOfWork) : IRequestHandler<SearchCommands, IReadOnlyCollection<CommandItem>>
{
    public async Task<IReadOnlyCollection<CommandItem>> Handle(SearchCommands request, CancellationToken cancellationToken)
    {
        var repository = unitOfWork.GetRepository<CommandItem>();
        return await repository.GetAllAsync();
    }
}
