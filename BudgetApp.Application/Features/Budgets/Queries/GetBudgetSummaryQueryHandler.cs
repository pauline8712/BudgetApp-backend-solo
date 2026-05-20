using AutoMapper;
using BudgetApp.Application.Features.Budgets.DTOs;
using BudgetApp.Application.Interfaces;
using MediatR;

namespace BudgetApp.Application.Features.Budgets.Queries;

public class GetBudgetSummaryQueryHandler : IRequestHandler<GetBudgetSummaryQuery, BudgetSummaryDto?>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public GetBudgetSummaryQueryHandler(
        IBudgetRepository budgetRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _budgetRepository = budgetRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<BudgetSummaryDto?> Handle(GetBudgetSummaryQuery request, CancellationToken cancellationToken)
    {
        var budget = await _budgetRepository.GetByIdAsync(request.BudgetId);

        if (budget == null)
            return null;

        var categories = await _categoryRepository.GetAllAsync(request.BudgetId);

        // AutoMapper beräknar AmountSpent = AllocatedAmount - CurrentBalance per kategori
        var categoryDtos = _mapper.Map<List<CategorySummaryDto>>(categories);

        var totalSpent = categoryDtos.Sum(c => c.AmountSpent);

        // Budget → BudgetSummaryDto för grundfälten, sedan sätter vi aggregerade värden manuellt
        var summary = _mapper.Map<BudgetSummaryDto>(budget);
        summary.Categories = categoryDtos;
        summary.TotalSpent = totalSpent;
        summary.RemainingAmount = budget.TotalAmount - totalSpent;

        return summary;
    }
}
