using AirlineTicketsAppWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace AirlineTicketsAppWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly string connectionString;

    public AuthController(IConfiguration configuration)
    {
        connectionString = configuration["ConnectionStrings:SqlServerDb"] ?? "";
    }

    [HttpPost]
    public IActionResult Login(UserDto userDto)
    {
        User user = new User();
        IdentityController identityController = new IdentityController();

        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = "Select * from UserTable where username = @username and password = @password";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@username", userDto.Username);
                    command.Parameters.AddWithValue("@password", userDto.Password);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user.UserId = reader.GetInt32(0);
                            user.Name = reader.GetString(1);
                            user.Type = (UserType)Int32.Parse(reader.GetString(2));
                            user.Username = reader.GetString(3);
                            user.Password = reader.GetString(4);
                        }
                    }
                }

            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("User", $"Exception thrown {ex.ToString()}");
            return BadRequest(ModelState);
        }

        string token = identityController.GenerateToken(user.Type.ToString(), user.Username);
        return Ok(token);
    }

}
