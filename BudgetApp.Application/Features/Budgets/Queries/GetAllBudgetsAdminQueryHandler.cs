using BudgetApp.Application.Features.Budgets.DTOs;
using BudgetApp.Application.Interfaces;
using MediatR;

namespace BudgetApp.Application.Features.Budgets.Queries;

// Hanterar GetAllBudgetsAdminQuery — hämtar alla budgetar för Admin
public class GetAllBudgetsAdminQueryHandler : IRequestHandler<GetAllBudgetsAdminQuery, List<BudgetDto>>
{
    private readonly IBudgetRepository _budgetRepository;

    public GetAllBudgetsAdminQueryHandler(IBudgetRepository budgetRepository)
    {
        _budgetRepository = budgetRepository;
    }

    public async Task<List<BudgetDto>> Handle(GetAllBudgetsAdminQuery request, CancellationToken cancellationToken)
    {
        // Hämtar alla budgetar från databasen utan filter
        var budgets = await _budgetRepository.GetAllAsync();

        // Mappar till DTO och returnerar
        return budgets.Select(b => new BudgetDto
        {
            Id = b.Id,
            UserId = b.UserId,
            Name = b.Name,
            Month = b.Month,
            Year = b.Year,
            TotalAmount = b.TotalAmount,
            CreatedAt = b.CreatedAt
        }).ToList();
    }
}