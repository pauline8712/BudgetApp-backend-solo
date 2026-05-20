using AutoMapper;
using BudgetApp.Application.Features.Budgets.DTOs;
using BudgetApp.Application.Interfaces;
using BudgetApp.Domain.Entities;
using MediatR;

namespace BudgetApp.Application.Features.Budgets.Commands;

public class CreateBudgetCommandHandler : IRequestHandler<CreateBudgetCommand, BudgetDto>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IMapper _mapper;

    public CreateBudgetCommandHandler(IBudgetRepository budgetRepository, IMapper mapper)
    {
        _budgetRepository = budgetRepository;
        _mapper = mapper;
    }

    public async Task<BudgetDto> Handle(CreateBudgetCommand request, CancellationToken cancellationToken)
    {
        var budget = new Budget
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Name = request.Name,
            Month = request.Month,
            Year = request.Year,
            TotalAmount = request.TotalAmount,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _budgetRepository.AddAsync(budget);
        return _mapper.Map<BudgetDto>(created);
    }
}
