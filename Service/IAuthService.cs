using AirlineTicketsAppWebApi.Models;
using Microsoft.AspNetCore.Authentication;

namespace AirlineTicketsAppWebApi.Service;

public interface IAuthService
{
    Task<AuthResult> AuthenticateAsync(string username, string password);
}
