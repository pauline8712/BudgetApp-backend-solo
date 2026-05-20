using BudgetApp.Application.Features.Budgets.Commands;
using FluentValidation;

namespace BudgetApp.Application.Features.Budgets.Validators;

// Validerar CreateBudgetCommand innan den hanteras av CreateBudgetCommandHandler
public class CreateBudgetCommandValidator : AbstractValidator<CreateBudgetCommand>
{
    public CreateBudgetCommandValidator()
    {
        // UserId måste vara ett giltigt GUID — får inte vara tomt
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId krävs");

        // Name får inte vara tomt
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Budgetnamn krävs");

        // Month måste vara mellan 1 och 12
        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12)
            .WithMessage("Månad måste vara mellan 1 och 12");

        // Year måste vara större än 2000
        RuleFor(x => x.Year)
            .GreaterThan(2000)
            .WithMessage("År måste vara större än 2000");

        // TotalAmount måste vara större än 0
        RuleFor(x => x.TotalAmount)
            .GreaterThan(0)
            .WithMessage("Totalbelopp måste vara större än 0");
    }
}