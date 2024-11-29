using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using Nvovka.CommandManager.Api.Dto;
using Nvovka.CommandManager.Authentication;
using Nvovka.CommandManager.Contract.Options;

namespace Nvovka.CommandManager.Api.Services;

public interface IGrpcLoginService
{
    Task<LoginReply> SendLoginAsync(LoginDto value);
}

public class GrpcLoginService: IGrpcLoginService
{
    private BasicLoginOption _options;
    private readonly ILogger<GrpcLoginService> _logger;
    public GrpcLoginService(IOptions<BasicLoginOption> options, ILogger<GrpcLoginService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<LoginReply> SendLoginAsync(LoginDto value)
    {
        using var channel = GrpcChannel.ForAddress("http://nvovka.commandmanager.authentication:5001");
        var client = new UserLoginService.UserLoginServiceClient(channel);

        var credentials = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{_options.UserName}:{_options.Password}"));
        _logger.LogInformation($"{_options.UserName}:{_options.Password}");

         var headers = new Metadata { { "Authorization", $"Basic {credentials}" } };
        var reply = await client.LoginAsync(new LoginRequest
        {
            Username = value.UserName,
            Password = value.Password
        }, headers: headers);
        _logger.LogInformation($"{reply.Message}:{reply.Success}");
        return reply;
    }
}
