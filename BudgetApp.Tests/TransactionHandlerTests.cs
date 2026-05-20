using AutoMapper;
using BudgetApp.Application.Features.Transactions.Commands;
using BudgetApp.Application.Features.Transactions.Queries;
using BudgetApp.Application.Mappings;
using BudgetApp.Domain.Entities;
using BudgetApp.Infrastructure.Database;
using BudgetApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BudgetApp.Tests;

[TestFixture]
public class TransactionHandlerTests
{
    private AppDbContext _context;
    private TransactionRepository _transactionRepository;
    private CategoryRepository _categoryRepository;
    private IMapper _mapper;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _transactionRepository = new TransactionRepository(_context);
        _categoryRepository = new CategoryRepository(_context);

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    // Hjälpmetod för att lägga till en kategori i databasen
    private async Task<Category> SeedCategoryAsync(decimal allocatedAmount = 5000)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            BudgetId = Guid.NewGuid(),
            Name = "Mat",
            AllocatedAmount = allocatedAmount,
            CurrentBalance = allocatedAmount,
            CreatedAt = DateTime.UtcNow
        };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    [Test]
    public async Task CreateTransaction_ShouldReturnTransactionDto_WithCorrectValues()
    {
        var category = await SeedCategoryAsync();
        var handler = new CreateTransactionCommandHandler(_transactionRepository, _categoryRepository, _mapper);
        var command = new CreateTransactionCommand
        {
            CategoryId = category.Id,
            Amount = 200,
            Type = "Expense",
            Description = "Lunch",
            Date = DateTime.UtcNow
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.CategoryId, Is.EqualTo(category.Id));
        Assert.That(result.Amount, Is.EqualTo(200));
        Assert.That(result.Type, Is.EqualTo("Expense"));
        Assert.That(result.Description, Is.EqualTo("Lunch"));
    }

    [Test]
    public async Task CreateTransaction_Expense_ShouldDecreaseCurrentBalance()
    {
        var category = await SeedCategoryAsync(allocatedAmount: 5000);
        var handler = new CreateTransactionCommandHandler(_transactionRepository, _categoryRepository, _mapper);
        var command = new CreateTransactionCommand
        {
            CategoryId = category.Id,
            Amount = 500,
            Type = "Expense",
            Description = "Mat",
            Date = DateTime.UtcNow
        };

        await handler.Handle(command, CancellationToken.None);

        var updated = await _categoryRepository.GetByIdAsync(category.Id);
        Assert.That(updated!.CurrentBalance, Is.EqualTo(4500));
    }

    [Test]
    public async Task CreateTransaction_Income_ShouldIncreaseCurrentBalance()
    {
        var category = await SeedCategoryAsync(allocatedAmount: 5000);
        var handler = new CreateTransactionCommandHandler(_transactionRepository, _categoryRepository, _mapper);
        var command = new CreateTransactionCommand
        {
            CategoryId = category.Id,
            Amount = 1000,
            Type = "Income",
            Description = "Lön",
            Date = DateTime.UtcNow
        };

        await handler.Handle(command, CancellationToken.None);

        var updated = await _categoryRepository.GetByIdAsync(category.Id);
        Assert.That(updated!.CurrentBalance, Is.EqualTo(6000));
    }

    [Test]
    public async Task UpdateTransaction_ShouldUpdateFieldsAndAdjustBalance()
    {
        var category = await SeedCategoryAsync(allocatedAmount: 5000);

        // Skapa en transaktion först
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            CategoryId = category.Id,
            Amount = 500,
            Type = "Expense",
            Description = "Gammal",
            Date = DateTime.UtcNow
        };
        _context.Transactions.Add(transaction);
        category.CurrentBalance -= 500; // simulera att den redan körts
        await _context.SaveChangesAsync();

        var handler = new UpdateTransactionCommandHandler(_transactionRepository, _categoryRepository, _mapper);
        var command = new UpdateTransactionCommand
        {
            Id = transaction.Id,
            Amount = 300,
            Type = "Expense",
            Description = "Ny beskrivning",
            Date = DateTime.UtcNow
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Amount, Is.EqualTo(300));
        Assert.That(result.Description, Is.EqualTo("Ny beskrivning"));

        var updatedCategory = await _categoryRepository.GetByIdAsync(category.Id);
        Assert.That(updatedCategory!.CurrentBalance, Is.EqualTo(4700)); // 4500 + 500 (ångra) - 300 (ny)
    }

    [Test]
    public async Task DeleteTransaction_ShouldReturnTrue_AndRestoreBalance()
    {
        var category = await SeedCategoryAsync(allocatedAmount: 5000);

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            CategoryId = category.Id,
            Amount = 1000,
            Type = "Expense",
            Description = "Test",
            Date = DateTime.UtcNow
        };
        _context.Transactions.Add(transaction);
        category.CurrentBalance -= 1000;
        await _context.SaveChangesAsync();

        var handler = new DeleteTransactionCommandHandler(_transactionRepository, _categoryRepository);
        var result = await handler.Handle(new DeleteTransactionCommand { Id = transaction.Id }, CancellationToken.None);

        Assert.That(result, Is.True);

        var updatedCategory = await _categoryRepository.GetByIdAsync(category.Id);
        Assert.That(updatedCategory!.CurrentBalance, Is.EqualTo(5000)); // balance återställd
    }

    [Test]
    public async Task DeleteTransaction_ShouldReturnFalse_WhenTransactionDoesNotExist()
    {
        var handler = new DeleteTransactionCommandHandler(_transactionRepository, _categoryRepository);
        var result = await handler.Handle(new DeleteTransactionCommand { Id = Guid.NewGuid() }, CancellationToken.None);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task GetTransactionsByCategory_ShouldReturnAllTransactionsForCategory()
    {
        var category = await SeedCategoryAsync();

        _context.Transactions.AddRange(
            new Transaction { Id = Guid.NewGuid(), CategoryId = category.Id, Amount = 100, Type = "Expense", Description = "A", Date = DateTime.UtcNow },
            new Transaction { Id = Guid.NewGuid(), CategoryId = category.Id, Amount = 200, Type = "Expense", Description = "B", Date = DateTime.UtcNow }
        );
        await _context.SaveChangesAsync();

        var handler = new GetTransactionsByCategoryQueryHandler(_transactionRepository, _mapper);
        var result = await handler.Handle(new GetTransactionsByCategoryQuery { CategoryId = category.Id }, CancellationToken.None);

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].Description, Is.EqualTo("A"));
        Assert.That(result[1].Description, Is.EqualTo("B"));
    }

    [Test]
    public async Task GetTransactionById_ShouldReturnCorrectTransaction()
    {
        var category = await SeedCategoryAsync();
        var transactionId = Guid.NewGuid();

        _context.Transactions.Add(new Transaction
        {
            Id = transactionId,
            CategoryId = category.Id,
            Amount = 350,
            Type = "Expense",
            Description = "Middag",
            Date = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        var handler = new GetTransactionByIdQueryHandler(_transactionRepository, _mapper);
        var result = await handler.Handle(new GetTransactionByIdQuery { Id = transactionId }, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(transactionId));
        Assert.That(result.Amount, Is.EqualTo(350));
        Assert.That(result.Description, Is.EqualTo("Middag"));
    }
}
