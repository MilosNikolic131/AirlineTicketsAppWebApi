using AirlineTicketsAppWebApi.Models;
using AirlineTicketsAppWebApi.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace AirlineTicketsAppWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    //private readonly string connectionString;
    //private readonly IdentityController identityController;
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        //connectionString = configuration["ConnectionStrings:SqlServerDb"] ?? "";
        //identityController = _identityController;
        _authService = authService;
        _logger = logger;
    }

    //[HttpPost]
    //public IActionResult Login(UserDto userDto)
    //{
    //    User user = new User();
    //    //IdentityController identityController = new IdentityController();

    //    try
    //    {
    //        using (var connection = new SqlConnection(connectionString))
    //        {
    //            connection.Open();

    //            string sql = "Select * from UserTable where username = @username and password = @password";

    //            using (var command = new SqlCommand(sql, connection))
    //            {
    //                command.Parameters.AddWithValue("@username", userDto.Username);
    //                command.Parameters.AddWithValue("@password", userDto.Password);

    //                using (var reader = command.ExecuteReader())
    //                {
    //                    while (reader.Read())
    //                    {
    //                        user.UserId = reader.GetInt32(0);
    //                        user.Name = reader.GetString(1);
    //                        user.Type = (UserType)Int32.Parse(reader.GetString(2));
    //                        user.Username = reader.GetString(3);
    //                        user.Password = reader.GetString(4);
    //                    }
    //                }
    //            }

    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        ModelState.AddModelError("User", $"Exception thrown {ex.ToString()}");
    //        return BadRequest(ModelState);
    //    }

    //    string token = identityController.GenerateToken(user.Type.ToString(), user.Username);
    //    return Ok(token);
    //}

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var result = await _authService.AuthenticateAsync(loginDto.Username, loginDto.Password);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed login attempt for {Username}", loginDto.Username);
                return Unauthorized(result.Message);
            }

            _logger.LogInformation("User {Username} logged in", loginDto.Username);
            return Ok(new { Token = result.Token });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Authentication error for {Username}", loginDto.Username);
            return StatusCode(500, "An error occurred during authentication");
        }
    }

}
