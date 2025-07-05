using AirlineTicketsAppWebApi.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AirlineTicketsAppWebApi.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("SqlServerDb")
            ?? throw new ArgumentNullException("Missing SQL connection string");
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        await using var connection = new SqlConnection(_connectionString);

        const string sql = "SELECT * FROM UserTable WHERE username = @username";

        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { username });
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        const string sql = "SELECT * FROM UserTable WHERE userid = @UserId";

        await using var connection = new SqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { userid = userId });
    }

    public async Task CreateUserAsync(User user)
    {
        await using var connection = new SqlConnection(_connectionString);

        const string sql = @"
            INSERT INTO UserTable 
            (Name, Type, Username, Password) 
            VALUES 
            (@Name, @Type, @Username, @Password)";

        await connection.ExecuteAsync(sql, user);
    }
}
