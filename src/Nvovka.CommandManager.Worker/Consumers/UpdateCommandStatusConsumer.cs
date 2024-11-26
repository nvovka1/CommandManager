using MassTransit;
using Nvovka.CommandManager.Contract.Messages;
using Nvovka.CommandManager.Contract.Models;
using Nvovka.CommandManager.Data;

namespace Nvovka.CommandManager.Worker.Consumers;

public class UpdateCommandStatusConsumer(IUnitOfWork unitOfWork) : IConsumer<IUpdateCommandStatusMessage>
{
    public async Task Consume(ConsumeContext<IUpdateCommandStatusMessage> context)
    {
        var message = context.Message;
        var repository = unitOfWork.GetRepository<CommandItem>();     
        var item = await repository.GetByIdAsync(message.Id);
        if (item == null)
        {
            return;
        }
        item.Status = message.Status;
        await repository.UpdateAsync(item);
        await unitOfWork.SaveChangesAsync();
    }
}
