using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Options;
using Nvovka.CommandManager.Contract.Options;

namespace Nvovka.CommandManager.Authentication.MiddleWare;

public class BasicAuthInterceptor : Interceptor
{
    private BasicLoginOption _options;
    private ILogger<BasicAuthInterceptor> _logger;
    public BasicAuthInterceptor(IOptions<BasicLoginOption> options, ILogger<BasicAuthInterceptor> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var authorizationHeader = context.RequestHeaders.GetValue("Authorization");

        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Basic "))
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Missing or invalid credentials"));
        }

        var encodedCredentials = authorizationHeader.Substring("Basic ".Length).Trim();
        var decodedCredentials = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
        var credentials = decodedCredentials.Split(':');
        if (credentials.Length == 2)
        {
            var username = credentials[0];
            var password = credentials[1];

            // Validate the credentials (can be from a DB, hardcoded, etc.)
            if (IsValidUser(username, password))
            {
                return await continuation(request, context);
            }
        }

        throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid credentials"));
    }

    private bool IsValidUser(string username, string password)
    {
        _logger.LogInformation($"BasicAuthInterceptor {_options.UserName}: {_options.Password}");
        _logger.LogInformation($"BasicAuthInterceptor receive {username}: {password}");
        return username == _options.UserName && password == _options.Password;
    }
}