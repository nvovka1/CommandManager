using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Nvovka.CommandManager.Api.Dto;
using Nvovka.CommandManager.Authentication;

namespace Nvovka.CommandManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto value)
        {
            using var channel = GrpcChannel.ForAddress("http://nvovka.commandmanager.authentication:5001");

            var client = new UserLoginService.UserLoginServiceClient(channel);

            var reply = await client.LoginAsync(new LoginRequest
            {
                Username = value.UserName,
                Password = value.Password
            });

            return Ok(reply);
        }
    }
}
