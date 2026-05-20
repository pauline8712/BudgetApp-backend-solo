using AutoMapper;
using BudgetApp.Application.Features.Categories.DTOs;
using BudgetApp.Application.Interfaces;
using BudgetApp.Domain.Entities;
using MediatR;

namespace BudgetApp.Application.Features.Categories.Commands;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            BudgetId = request.BudgetId,
            Name = request.Name,
            AllocatedAmount = request.AllocatedAmount,
            // CurrentBalance sätts till AllocatedAmount vid skapande
            CurrentBalance = request.AllocatedAmount,
            CreatedAt = DateTime.UtcNow,
            IsWeekly = request.IsWeekly,
            WeeklyAmount = request.WeeklyAmount
        };

        var created = await _categoryRepository.AddAsync(category);
        return _mapper.Map<CategoryDto>(created);
    }
}
