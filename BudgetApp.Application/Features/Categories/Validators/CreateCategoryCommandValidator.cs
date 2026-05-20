using BudgetApp.Application.Features.Categories.Commands;
using FluentValidation;

namespace BudgetApp.Application.Features.Categories.Validators;

// Validerar CreateCategoryCommand innan den hanteras av CreateCategoryCommandHandler
public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        // BudgetId måste vara ett giltigt GUID — får inte vara tomt
        RuleFor(x => x.BudgetId)
            .NotEmpty()
            .WithMessage("BudgetId krävs");

        // Name får inte vara tomt
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Kategorinamn krävs");

        // AllocatedAmount måste vara större än 0
        RuleFor(x => x.AllocatedAmount)
            .GreaterThan(0)
            .WithMessage("Budgetbelopp måste vara större än 0");

        // WeeklyAmount måste vara större än 0 om IsWeekly är true
        RuleFor(x => x.WeeklyAmount)
            .GreaterThan(0)
            .When(x => x.IsWeekly)
            .WithMessage("Veckobelopp måste vara större än 0 om kategorin är veckobaserad");
    }
}