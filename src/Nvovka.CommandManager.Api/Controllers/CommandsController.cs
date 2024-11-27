using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Nvovka.CommandManager.Api.Dto;
using Nvovka.CommandManager.Commands.Command;
using Nvovka.CommandManager.Contract.Messages;
using IMediator = MediatR.IMediator;

#nullable enable

namespace Nvovka.CommandManager.Api.Controllers
{
    [Route("api/v1/commands")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IBus _bus;
        public CommandsController(IMediator mediator, IBus bus)
        {
            _mediator = mediator;
            _bus = bus;
        }


        [HttpPost()]
        public async Task<IActionResult> CreateCommand([FromBody] CommandDto message, CancellationToken cts = default)
        {
            var command = await _mediator.Send(new GetCommand(message.Id), cts);
            if (command is not null)
            {
                return BadRequest("This command already exist");
            }

            await _bus.Publish<ICreateCommandMessage>(new CreateCommandMessage(message.Name, message.Description), cts);
            return Accepted();
        }

        [HttpPut()]
        public async Task<IActionResult> UpdateCommand([FromBody] CommandUpdateDto message, CancellationToken cts = default)
        {
            var command = await _mediator.Send(new GetCommand(message.Id), cts);
            if (command is null)
            {
                return BadRequest("This command not exist");
            }

            await _bus.Publish<IUpdateCommandStatusMessage>(new UpdateCommandStatusMessage(message.Id, message.Status), cts);
            return Accepted();
        }

        [HttpGet()]
        public async Task<IActionResult> GetAll(CancellationToken cts = default)
        {
            var commands = await _mediator.Send(new SearchCommands(), cts);
            return Ok(commands);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cts = default)
        {
            var command = await _mediator.Send(new GetCommand(id), cts);
            return Ok(command);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cts = default)
        {
            _ = await _mediator.Send(new DeleteCommand(id), cts);
            return Ok();
        }

        [HttpDelete()]
        public async Task<IActionResult> DeleteAll(CancellationToken cts = default)
        {
            await _mediator.Send(new DeleteAllCommands(), cts);
            return Ok();
        }
    }
}
