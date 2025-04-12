using AirlineTicketsAppWebApi.Models;
using AirlineTicketsAppWebApi.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace AirlineTicketsAppWebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly string connectionString;

    public UserController(IConfiguration configuration)
    {
        connectionString = configuration["ConnectionStrings:SqlServerDb"] ?? "";
    }

    [SubAuthorize("ADMIN")]
    [HttpPost]
    public IActionResult CreateUser(UserDto userDto)
    {
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = "Insert into UserTable " +
                    "(name, type, username, password) values " +
                    "(@name, @type, @username, @password)";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@name", userDto.Name);
                    command.Parameters.AddWithValue("@type", userDto.Type);
                    command.Parameters.AddWithValue("@username", userDto.Username);
                    command.Parameters.AddWithValue("@password", userDto.Password);

                    command.ExecuteNonQuery();
                }

            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("User", $"Exception thrown {ex.ToString()}");
            return BadRequest(ModelState);
        }
        return Ok();
    }
}
