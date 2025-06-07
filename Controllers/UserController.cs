using AirlineTicketsAppWebApi.Models;
using AirlineTicketsAppWebApi.Repositories;
using AirlineTicketsAppWebApi.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace AirlineTicketsAppWebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly string connectionString;
    private readonly ILogger<UserController> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserController(IConfiguration configuration,
                          ILogger<UserController> logger,
                          IUserRepository UserRepository,
                          IPasswordHasher<User> passwordHasher)
    {
        connectionString    = configuration["ConnectionStrings:SqlServerDb"] ?? "";
        _logger             = logger;
        _userRepository     = UserRepository;
        _passwordHasher     = passwordHasher;
    }

    [SubAuthorize("ADMIN")]
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {

            var user = new User
            {
                Name        = userDto.Name,
                Type        = userDto.Type,
                Username    = userDto.Username,
                Password    = _passwordHasher.HashPassword(null, userDto.Password)
            };

            await _userRepository.CreateUserAsync(user);
            return StatusCode(201, $"User {user.Username} created successfully");
        }
        catch (DbException ex)
        {
            _logger.LogError(ex, "Error creating user {Username}", userDto.Username);
            return Conflict("Username already exists");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating user");
            return StatusCode(500, "Internal server error");
        }
    }
    
}
