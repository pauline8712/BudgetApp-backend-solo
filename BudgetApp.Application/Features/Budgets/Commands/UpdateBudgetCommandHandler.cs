using AutoMapper;
using BudgetApp.Application.Features.Budgets.DTOs;
using BudgetApp.Application.Interfaces;
using MediatR;

namespace BudgetApp.Application.Features.Budgets.Commands;

public class UpdateBudgetCommandHandler : IRequestHandler<UpdateBudgetCommand, BudgetDto>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IMapper _mapper;

    public UpdateBudgetCommandHandler(IBudgetRepository budgetRepository, IMapper mapper)
    {
        _budgetRepository = budgetRepository;
        _mapper = mapper;
    }

    public async Task<BudgetDto> Handle(UpdateBudgetCommand request, CancellationToken cancellationToken)
    {
        var budget = await _budgetRepository.GetByIdAsync(request.Id);

        if (budget == null)
            return null!;

        budget.Name = request.Name;
        budget.Month = request.Month;
        budget.Year = request.Year;
        budget.TotalAmount = request.TotalAmount;

        var updated = await _budgetRepository.UpdateAsync(budget);
        return _mapper.Map<BudgetDto>(updated);
    }
}
