using BudgetApp.Application.Interfaces;
using BudgetApp.Domain.Entities;
using BudgetApp.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace BudgetApp.Infrastructure.Repositories;

// BudgetRepository ärver från BaseRepository — får GetByIdAsync, AddAsync, UpdateAsync, DeleteAsync gratis
// Behåller sina egna GetAllAsync eftersom budgetar filtreras på UserId
public class BudgetRepository : BaseRepository<Budget>, IBudgetRepository
{
    public BudgetRepository(AppDbContext context) : base(context)
    {
    }

    // Hämtar alla budgetar för en specifik användare
    public async Task<List<Budget>> GetAllAsync(Guid userId)
    {
        return await _context.Budgets
            .Where(b => b.UserId == userId)
            .ToListAsync();
    }

    // Hämtar ALLA budgetar utan filter — används bara av Admin
    public async Task<List<Budget>> GetAllAsync()
    {
        return await _context.Budgets.ToListAsync();
    }
}