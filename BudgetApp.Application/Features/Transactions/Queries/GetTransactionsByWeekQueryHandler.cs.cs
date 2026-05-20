using AutoMapper;
using BudgetApp.Application.Features.Transactions.DTOs;
using BudgetApp.Application.Interfaces;
using MediatR;
using System.Globalization;

namespace BudgetApp.Application.Features.Transactions.Queries;

public class GetTransactionsByWeekQueryHandler : IRequestHandler<GetTransactionsByWeekQuery, List<TransactionDto>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMapper _mapper;

    public GetTransactionsByWeekQueryHandler(ITransactionRepository transactionRepository, IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
    }

    public async Task<List<TransactionDto>> Handle(GetTransactionsByWeekQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _transactionRepository.GetAllByCategoryIdAsync(request.CategoryId);

        // Filtrerar transaktioner på rätt år och veckonummer
        var filtered = transactions.Where(t =>
            t.Date.Year == request.Year &&
            ISOWeek.GetWeekOfYear(t.Date) == request.WeekNumber
        ).ToList();

        return _mapper.Map<List<TransactionDto>>(filtered);
    }
}
