using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Nvovka.CommandManager.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(Guid userId, string userEmail);
}

public class JwtTokenGenerator : IJwtTokenGenerator
{
    public string GenerateToken(Guid userId, string userEmail)
    {
        var key = "MySecretKeyForMyApplicationVeryLong"u8.ToArray();
        var tokenHandler = new JwtSecurityTokenHandler();
        var claimes = new List<Claim>()
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, userId.ToString()),
        };

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claimes),
            Expires = DateTime.UtcNow.AddDays(1),
            Issuer = "http://nvovka.com",
            Audience = "http://localhost:5000",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
