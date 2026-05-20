using FluentValidation;
using MediatR;

namespace BudgetApp.Application.Behaviours;

// Pipeline Behaviour — körs automatiskt innan varje MediatR handler
// Validerar incoming requests med FluentValidation om en validator finns registrerad
public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    // Alla validators för TRequest injiceras via konstruktorn
    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Om inga validators finns — gå vidare direkt till handler
        if (!_validators.Any())
            return await next();

        // Kör alla validators parallellt
        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken))
        );

        // Samla alla valideringsfel
        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        // Kasta ett undantag om det finns valideringsfel
        if (failures.Count != 0)
            throw new ValidationException(failures);

        // Inga fel — gå vidare till handler
        return await next();
    }
}