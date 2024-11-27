using Microsoft.AspNetCore.Mvc;
using Nvovka.CommandManager.Api.Dto;
using Nvovka.CommandManager.Data.Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Nvovka.CommandManager.Api.Controllers
{
    [Route("api/v1/commandItems")]
    [ApiController]
    public class CommandItemsController(ICommandDupperRepository repository) : ControllerBase
    {
        // GET: api/<CommandItemsController>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await repository.GetCommandItemAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CommandItemDto value)
        {
            var entity = new Contract.Models.CommandItem()
            {
                Name = value.Name,
                Description = value.Description,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                Status = Contract.Enums.CommandStatus.Scheduled,
            };

            foreach (var item in value.Items)
            {
                entity.CommandReferenceItems.Add(new Contract.Models.CommandReferenceItem()
                {
                    Description = item.Name,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow,
                });
            }

            return Ok(await repository.InsertOrderAsync(entity));
        }

        ////// PUT api/<CommandItemsController>/5
        ////[HttpPut("{id}")]
        ////public void Put(int id, [FromBody] string value)
        ////{
        ////}

        ////// DELETE api/<CommandItemsController>/5
        ////[HttpDelete("{id}")]
        ////public void Delete(int id)
        ////{
        ////}
    }
}
