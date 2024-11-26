using MassTransit;
using Nvovka.CommandManager.Contract.Messages;
using Nvovka.CommandManager.Contract.Models;
using Nvovka.CommandManager.Data;

namespace Nvovka.CommandManager.Worker.Consumers;

public class CreateCommandConsumer(IUnitOfWork unitOfWork) : IConsumer<ICreateCommandMessage>
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
}
