using BudgetApp.Application.Features.Transactions.Commands;
using FluentValidation;

namespace BudgetApp.Application.Features.Transactions.Validators;

// Validerar CreateTransactionCommand innan den hanteras av CreateTransactionCommandHandler
public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionCommandValidator()
    {
        // CategoryId måste vara ett giltigt GUID — får inte vara tomt
        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("CategoryId krävs");

        // Amount måste vara större än 0
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Belopp måste vara större än 0");

        // Type måste vara antingen Expense eller Income
        RuleFor(x => x.Type)
            .NotEmpty()
            .Must(t => t == "Expense" || t == "Income")
            .WithMessage("Typ måste vara 'Expense' eller 'Income'");

        // Date får inte vara tomt
        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("Datum krävs");
    }
}