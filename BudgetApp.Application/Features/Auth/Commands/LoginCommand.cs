using BudgetApp.Application.Features.Auth.DTOs;
using MediatR;

namespace BudgetApp.Application.Features.Auth.Commands;

public class LoginCommand : IRequest<AuthResponseDto>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}