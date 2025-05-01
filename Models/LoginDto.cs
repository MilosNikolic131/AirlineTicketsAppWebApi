using System.ComponentModel.DataAnnotations;

namespace AirlineTicketsAppWebApi.Models;

public record LoginDto([Required] string Username, [Required][DataType(DataType.Password)] string Password);