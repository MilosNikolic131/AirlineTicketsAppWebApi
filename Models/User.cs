using System.ComponentModel.DataAnnotations;

namespace AirlineTicketsAppWebApi.Models;

public class User
{
    public int UserId { get; set; }

    public string? Name { get; set; }

    public UserType Type { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }
}
