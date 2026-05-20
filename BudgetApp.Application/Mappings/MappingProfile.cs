using AutoMapper;
using BudgetApp.Application.Features.Budgets.DTOs;
using BudgetApp.Application.Features.Categories.DTOs;
using BudgetApp.Application.Features.Transactions.DTOs;
using BudgetApp.Application.Features.Users.DTOs;
using BudgetApp.Domain.Entities;

namespace BudgetApp.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();

        CreateMap<Budget, BudgetDto>();

        CreateMap<Budget, BudgetSummaryDto>()
            .ForMember(d => d.Categories, opt => opt.Ignore())
            .ForMember(d => d.TotalSpent, opt => opt.Ignore())
            .ForMember(d => d.RemainingAmount, opt => opt.Ignore());

        CreateMap<Category, CategoryDto>();

        // AmountSpent är ett beräknat fält — inte en property på entiteten
        CreateMap<Category, CategorySummaryDto>()
            .ForMember(d => d.AmountSpent, opt => opt.MapFrom(s => s.AllocatedAmount - s.CurrentBalance));

        CreateMap<Transaction, TransactionDto>();
    }
}
