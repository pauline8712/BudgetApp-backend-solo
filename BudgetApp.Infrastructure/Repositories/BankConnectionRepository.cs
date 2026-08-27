using BudgetApp.Application.Interfaces;
using BudgetApp.Domain.Entities;
using BudgetApp.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace BudgetApp.Infrastructure.Repositories;

public class BankConnectionRepository : BaseRepository<BankConnection>, IBankConnectionRepository
{
    public BankConnectionRepository(AppDbContext context) : base(context)
    {

    }

    public async Task<BankConnection?> GetByUserIdAsync(Guid userId)
    {
        return await _context.BankConnections
        .FirstOrDefaultAsync(BCrypt => BCrypt.UserId == userId);
    }
}
