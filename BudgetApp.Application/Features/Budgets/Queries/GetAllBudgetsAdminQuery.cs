using BudgetApp.Application.Features.Budgets.DTOs;
using MediatR;

namespace BudgetApp.Application.Features.Budgets.Queries;

// Query för att hämta ALLA budgetar — används bara av Admin
public class GetAllBudgetsAdminQuery : IRequest<List<BudgetDto>>
{
}