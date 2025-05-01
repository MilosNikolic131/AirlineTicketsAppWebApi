namespace AirlineTicketsAppWebApi.Models;

public record AuthResult(bool IsSuccess, string? Token = null, string? Message = null)
{
    public static AuthResult Success(string token) => new(true, token);
    public static AuthResult Fail(string message) => new(false, null, message);
}
