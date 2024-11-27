using MassTransit;
using Nvovka.CommandManager.Contract.Messages;
using Nvovka.CommandManager.Contract.Models;
using Nvovka.CommandManager.Data;

namespace Nvovka.CommandManager.Worker.Consumers;

public class CreateCommandConsumer(IUnitOfWork unitOfWork) : 
    IConsumer<ICreateCommandMessage>,
    IConsumer<IUpdateCommandStatusMessage>
{
    public async Task Consume(ConsumeContext<ICreateCommandMessage> context)
    {
        var repository = unitOfWork.GetRepository<CommandItem>();
        var message = context.Message;
        await repository.AddAsync(new CommandItem()
        {
            Description = context.Message.Description,
            Name = message.Name,
            Status = Contract.Enums.CommandStatus.Scheduled,
        });
        await unitOfWork.SaveChangesAsync();
    }

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
