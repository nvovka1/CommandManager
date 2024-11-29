using Grpc.Core;

namespace Nvovka.CommandManager.Authentication.Services;

public class LoginService : UserLoginService.UserLoginServiceBase
{
    private readonly ILogger<LoginService> _logger;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public LoginService(ILogger<LoginService> logger, IJwtTokenGenerator tokenGenerator)
    {
        _logger = logger;
        _tokenGenerator = tokenGenerator;
    }

    public override Task<LoginReply> Login(LoginRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"Received login request for username: {request.Username}");

        bool isSuccess = request.Username == "admin" && request.Password == "password";

        return Task.FromResult(new LoginReply
        {
            Message = isSuccess ? _tokenGenerator.GenerateToken(Guid.NewGuid(), request.Username) : "Invalid credentials.",
            Success = isSuccess
        });
    }
}
