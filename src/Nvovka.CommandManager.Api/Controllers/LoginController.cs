using Microsoft.AspNetCore.Mvc;
using Nvovka.CommandManager.Api.Dto;
using Nvovka.CommandManager.Api.Services;


namespace Nvovka.CommandManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IGrpcLoginService _grpcLoginService;
        public LoginController(IGrpcLoginService grpcLoginService)
        {
            _grpcLoginService = grpcLoginService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto value)
        {
            var logindata = await _grpcLoginService.SendLoginAsync(value);
            return Ok(logindata);
        }
    }
}
