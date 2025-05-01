using AirlineTicketsAppWebApi.Models;

namespace AirlineTicketsAppWebApi.Service;

public interface ITokenService
{
    string GenerateToken(User user);
}
