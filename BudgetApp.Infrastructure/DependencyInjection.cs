using BudgetApp.Application.Interfaces;
using BudgetApp.Infrastructure.Database;
using BudgetApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BudgetApp.Infrastructure;

public static class DependencyInjection
{
    //Registrerar alla Infrastructure-tjänster - anropas från program.cs
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        //Konfiguerar EF Core med PostgreSQL och connection string från appsettings.json
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")));

        //Registrerar repositories - kopplar interface till implementationen. 
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IBudgetRepository, BudgetRepository>();

        services.AddScoped<ITransactionRepository, TransactionRepository>();

        return services;
    }
}