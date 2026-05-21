using BudgetApp.Domain.Entities;
using BudgetApp.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BudgetApp.Infrastructure;

// Seeder databasen med grunddata vid uppstart
public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Skapar Admin-användare om den inte redan finns
        var adminExists = await context.Users.AnyAsync(u => u.Role == "Admin");
        if (!adminExists)
        {
            context.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@budgetapp.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }
    }
}