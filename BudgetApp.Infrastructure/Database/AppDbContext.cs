using BudgetApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;

namespace BudgetApp.Infrastructure.Database;
//IDataProtectionKeyContext är ett interface från paketet vi just installerade.
//Det talar om för Data Protection-systemet: "den här DbContext-klassen kan användas för att lagra krypteringsnycklar".
//Utan det vet inte biblioteket var den ska spara dem.
public class AppDbContext : DbContext, IDataProtectionKeyContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    //EF Core only creates tables for the classes that they are aware of via a DbSet. 
    public DbSet<User> Users { get; set; }
    public DbSet<Budget> Budgets { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<BankConnection> BankConnections { get; set; }
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User → Budget (1 till N) — en användare kan ha flera budgetar, en per månad och år
        modelBuilder.Entity<Budget>()
            .HasOne(b => b.User)
            .WithMany(u => u.Budgets)
            .HasForeignKey(b => b.UserId);

        // Budget → Categories (1 till N)
        modelBuilder.Entity<Category>()
            .HasOne(c => c.Budget)
            .WithMany(b => b.Categories)
            .HasForeignKey(c => c.BudgetId);

        // Category → Transactions (1 till N)
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Category)
            .WithMany(c => c.Transactions)
            .HasForeignKey(t => t.CategoryId);

        // User → BankConnection (1 till 1) — a user has at most one bankconnection
        modelBuilder.Entity<BankConnection>()
            .HasOne(bc => bc.User)
            .WithOne()
            .HasForeignKey<BankConnection>(bc => bc.UserId);


        // Decimal precision
        modelBuilder.Entity<Budget>()
            .Property(b => b.TotalAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Category>()
            .Property(c => c.AllocatedAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Category>()
            .Property(c => c.CurrentBalance)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Category>()
            .Property(c => c.WeeklyAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasPrecision(18, 2);
    }
}