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
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

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
