using AirlineTicketsAppWebApi.Models;

namespace AirlineTicketsAppWebApi.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserByUsernameAsync(string username);
    Task CreateUserAsync(User user);

    Task<User?> GetUserByIdAsync(int userId);
}
