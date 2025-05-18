using AirlineTicketsAppWebApi.Models;
using AirlineTicketsAppWebApi.Repositories;
using Microsoft.AspNetCore.Identity;

namespace AirlineTicketsAppWebApi.Service;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }
    public async Task<AuthResult> AuthenticateAsync(string username, string password)
    {
        var user = await _userRepository.GetUserByUsernameAsync(username);

        if (user == null 
            //||
            //_passwordHasher.VerifyHashedPassword(user, user.Password, password)
            //!= PasswordVerificationResult.Success
            )
        {
            return AuthResult.Fail("Invalid credentials");
        }

        var token = _tokenService.GenerateToken(user);
        return AuthResult.Success(token);
    }
}
