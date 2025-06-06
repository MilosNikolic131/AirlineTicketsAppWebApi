﻿using AirlineTicketsAppWebApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AirlineTicketsAppWebApi.Service;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
        ValidateConfig();
    }
    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        //var key = "KeyTestKeyTestKeyTestKeyTestKeyTestKeyTestKeyTest"u8.ToArray();
        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new (JwtRegisteredClaimNames.Sub, user.Type.ToString()),
            new (JwtRegisteredClaimNames.Name, user.Username)
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
    //public string GenerateToken(User user)
    //{
    //    var claims = new List<Claim>
    //    {
    //        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    //        new(ClaimTypes.Name, user.Username),
    //        new(ClaimTypes.Role, user.Type.ToString())
    //    };

    //    var key = new SymmetricSecurityKey(
    //        Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

    //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    //    var token = new JwtSecurityToken(
    //        issuer: _config["Jwt:Issuer"],
    //        audience: _config["Jwt:Audience"],
    //        claims: claims,
    //        expires: DateTime.UtcNow.AddHours(1),
    //        signingCredentials: creds);

    //    return new JwtSecurityTokenHandler().WriteToken(token);
    //}

    private void ValidateConfig()
    {
        var key = _config["Jwt:Key"];
        if (string.IsNullOrEmpty(key) || key.Length < 32)
            throw new ArgumentException("Invalid JWT configuration");
    }
}
