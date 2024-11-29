using System.Text;

namespace Nvovka.CommandManager.Authentication.MiddleWare;

public class BasicAuthMiddleware
{
    private readonly RequestDelegate _next;

    public BasicAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var authorizationHeader = httpContext.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Basic "))
        {
            httpContext.Response.StatusCode = 401; // Unauthorized
            return;
        }

        var encodedCredentials = authorizationHeader.Substring("Basic ".Length).Trim();
        var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
        var credentials = decodedCredentials.Split(':');
        if (credentials.Length == 2)
        {
            var username = credentials[0];
            var password = credentials[1];

            // Validate username and password (replace with your validation logic)
            if (IsValidUser(username, password))
            {
                httpContext.Items["User"] = username; // Store user info in the request context
                await _next(httpContext);
            }
            else
            {
                httpContext.Response.StatusCode = 401; // Unauthorized
            }
        }
        else
        {
            httpContext.Response.StatusCode = 401; // Unauthorized
        }
    }

    private bool IsValidUser(string username, string password)
    {
        return username == "user" && password == "password"; // Example validation
    }
}