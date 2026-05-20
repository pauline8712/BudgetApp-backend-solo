using AutoMapper;
using BudgetApp.Application.Features.Transactions.DTOs;
using BudgetApp.Application.Interfaces;
using MediatR;

namespace BudgetApp.Application.Features.Transactions.Queries;

public class GetTransactionsByCategoryQueryHandler : IRequestHandler<GetTransactionsByCategoryQuery, List<TransactionDto>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMapper _mapper;

    public GetTransactionsByCategoryQueryHandler(ITransactionRepository transactionRepository, IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
    }

    public async Task<List<TransactionDto>> Handle(GetTransactionsByCategoryQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _transactionRepository.GetAllByCategoryIdAsync(request.CategoryId);
        return _mapper.Map<List<TransactionDto>>(transactions);
    }
}
