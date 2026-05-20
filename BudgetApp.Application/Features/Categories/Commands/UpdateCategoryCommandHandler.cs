using AutoMapper;
using BudgetApp.Application.Features.Categories.DTOs;
using BudgetApp.Application.Interfaces;
using MediatR;

namespace BudgetApp.Application.Features.Categories.Commands;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id);

        if (category == null)
            throw new Exception($"Category with id {request.Id} not found");

        category.Name = request.Name;
        category.AllocatedAmount = request.AllocatedAmount;
        category.IsWeekly = request.IsWeekly;
        category.WeeklyAmount = request.WeeklyAmount;

        var updated = await _categoryRepository.UpdateAsync(category);
        return _mapper.Map<CategoryDto>(updated);
    }
}
