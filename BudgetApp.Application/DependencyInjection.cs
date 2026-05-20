using BudgetApp.Application.Behaviours;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BudgetApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        // Registrerar alla MediatR handlers i Application-lagret
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(
                typeof(DependencyInjection).Assembly));

        // Registrerar ValidationBehaviour som körs innan varje handler
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        // Registrerar alla FluentValidation validators i Application-lagret
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

         return services;
    }
}