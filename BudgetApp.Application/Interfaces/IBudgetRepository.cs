using BudgetApp.Domain.Entities;

namespace BudgetApp.Application.Interfaces;

// Interface för BudgetRepository — definierar tillgängliga databasoperationer för Budget
public interface IBudgetRepository
{
    // Hämtar alla budgetar för en specifik användare
    Task<List<Budget>> GetAllAsync(Guid userId);

    // Hämtar ALLA budgetar — används bara av Admin
    Task<List<Budget>> GetAllAsync();

    // Hämtar en specifik budget baserat på Id
    Task<Budget?> GetByIdAsync(Guid id);

    // Skapar en ny budget
    Task<Budget> AddAsync(Budget budget);

    // Uppdaterar en befintlig budget
    Task<Budget> UpdateAsync(Budget budget);

    // Tar bort en budget baserat på Id
    Task DeleteAsync(Guid id);
}