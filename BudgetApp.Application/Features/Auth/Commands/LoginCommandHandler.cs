using BudgetApp.Application.Features.Auth.DTOs;
using BudgetApp.Application.Interfaces;
using MediatR;

namespace BudgetApp.Application.Features.Auth.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginCommandHandler(IUserRepository userRepository, IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Hitta användaren via email
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
            throw new Exception("Invalid email or password");

        // Verifiera lösenordet med BCrypt
        var isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isValid)
            throw new Exception("Invalid email or password");

        // Skapa och returnera token
        var token = _jwtTokenService.GenerateToken(user);
        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email,
            Role = user.Role
        };
    }
}