using AutoMapper;
using BudgetApp.Application.Features.Transactions.DTOs;
using BudgetApp.Application.Interfaces;
using BudgetApp.Domain.Entities;
using MediatR;

namespace BudgetApp.Application.Features.Transactions.Commands;

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, TransactionDto>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CreateTransactionCommandHandler(
        ITransactionRepository transactionRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<TransactionDto> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId);

        if (category == null)
            throw new Exception($"Category with id {request.CategoryId} not found");

        // Expense minskar saldot, Income ökar saldot
        if (request.Type == "Expense")
            category.CurrentBalance -= request.Amount;
        else if (request.Type == "Income")
            category.CurrentBalance += request.Amount;

        await _categoryRepository.UpdateAsync(category);

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            CategoryId = request.CategoryId,
            Amount = request.Amount,
            Type = request.Type,
            Description = request.Description,
            Date = request.Date
        };

        var created = await _transactionRepository.AddAsync(transaction);
        return _mapper.Map<TransactionDto>(created);
    }
}
