using AutoMapper;
using BudgetApp.Application.Features.Transactions.DTOs;
using BudgetApp.Application.Interfaces;
using MediatR;

namespace BudgetApp.Application.Features.Transactions.Commands;

public class UpdateTransactionCommandHandler : IRequestHandler<UpdateTransactionCommand, TransactionDto>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public UpdateTransactionCommandHandler(
        ITransactionRepository transactionRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<TransactionDto> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _transactionRepository.GetByIdAsync(request.Id);

        if (transaction == null)
            return null;

        var category = await _categoryRepository.GetByIdAsync(transaction.CategoryId);

        if (category == null)
            throw new Exception($"Category with id {transaction.CategoryId} not found");

        // Ångrar det gamla beloppets effekt på CurrentBalance
        if (transaction.Type == "Expense")
            category.CurrentBalance += transaction.Amount;
        else if (transaction.Type == "Income")
            category.CurrentBalance -= transaction.Amount;

        // Applicerar det nya beloppets effekt på CurrentBalance
        if (request.Type == "Expense")
            category.CurrentBalance -= request.Amount;
        else if (request.Type == "Income")
            category.CurrentBalance += request.Amount;

        await _categoryRepository.UpdateAsync(category);

        transaction.Amount = request.Amount;
        transaction.Type = request.Type;
        transaction.Description = request.Description;
        transaction.Date = request.Date;

        var updated = await _transactionRepository.UpdateAsync(transaction);
        return _mapper.Map<TransactionDto>(updated);
    }
}
