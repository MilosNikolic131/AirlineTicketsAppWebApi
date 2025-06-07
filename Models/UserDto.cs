namespace AirlineTicketsAppWebApi.Models;
using System.ComponentModel.DataAnnotations;

public class UserDto
{
    [Required]
    public string? Name { get; set; }

    [Required]
    public UserType Type { get; set; }

    [Required]
    public string? Username { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
}
