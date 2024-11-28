using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nvovka.CommandManager.Commands.Command;
using Nvovka.CommandManager.Contract.Models;
using Nvovka.CommandManager.Data;

namespace Nvovka.CommandManager.Commands.CommandHandler;

public class GetCommandHandler(IUnitOfWork unitOfWork, ILogger<GetCommandHandler> logger) : IRequestHandler<GetCommand, CommandItem>
{
    public async Task<CommandItem> Handle(GetCommand request, CancellationToken cancellationToken)
    {
        var repository = unitOfWork.GetRepository<CommandItem>();

        var commandItem = unitOfWork.appDbContext()
            .CommandItems
            .Include(x => x.CommandReferenceItems)
            .SingleOrDefault(x=>x.Id == request.Id);
        logger.LogInformation($" 111  {commandItem?.Id}:");

        ////if (commandItem != null)
        ////{
        ////    await unitOfWork.appDbContext()
        ////        .Entry(commandItem)
        ////        .Collection(x => x.CommandReferenceItems)
        ////        .LoadAsync(cancellationToken);

        ////    logger.LogInformation($"111 {commandItem.Id}:");

        ////    foreach (var item in commandItem.CommandReferenceItems)
        ////    {
        ////        logger.LogInformation($" 111 {item.Id} - {item.Description}");
        ////    }
        ////}

        foreach (var item in commandItem.CommandReferenceItems)
        {
            logger.LogInformation($"222 {item.Id} - {item.Description}");
        }
        return await repository.GetByIdAsync(request.Id);
    }
}


