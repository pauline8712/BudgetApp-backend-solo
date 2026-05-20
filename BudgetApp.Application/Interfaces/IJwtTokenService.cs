using BudgetApp.Domain.Entities;

namespace BudgetApp.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}