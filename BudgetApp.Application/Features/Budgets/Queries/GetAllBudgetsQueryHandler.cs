using AutoMapper;
using BudgetApp.Application.Features.Budgets.DTOs;
using BudgetApp.Application.Interfaces;
using MediatR;

namespace BudgetApp.Application.Features.Budgets.Queries;

public class GetAllBudgetsQueryHandler : IRequestHandler<GetAllBudgetsQuery, List<BudgetDto>>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IMapper _mapper;

    public GetAllBudgetsQueryHandler(IBudgetRepository budgetRepository, IMapper mapper)
    {
        _budgetRepository = budgetRepository;
        _mapper = mapper;
    }

    public async Task<List<BudgetDto>> Handle(GetAllBudgetsQuery request, CancellationToken cancellationToken)
    {
        var budgets = await _budgetRepository.GetAllAsync(request.UserId);
        return _mapper.Map<List<BudgetDto>>(budgets);
    }
}
