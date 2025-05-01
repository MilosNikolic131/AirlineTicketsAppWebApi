using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;

namespace AirlineTicketsAppWebApi.Controllers;


public class IdentityController
{
    private readonly IConfiguration config;

    public IdentityController(IConfiguration _config)
    {
        config = _config;
    }

    public string GenerateToken(string type, string username)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        //var key = "KeyTestKeyTestKeyTestKeyTestKeyTestKeyTestKeyTest"u8.ToArray();
        var key = Encoding.UTF8.GetBytes(config["Jwt:Key"]);

        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new (JwtRegisteredClaimNames.Sub, type),
            new (JwtRegisteredClaimNames.Name, username)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(60),
            Issuer = "test.com",
            Audience = "test.com",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
