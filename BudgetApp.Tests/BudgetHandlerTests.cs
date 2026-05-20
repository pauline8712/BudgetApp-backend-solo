using AutoMapper;
using BudgetApp.Application.Features.Budgets.Commands;
using BudgetApp.Application.Features.Budgets.Queries;
using BudgetApp.Application.Mappings;
using BudgetApp.Domain.Entities;
using BudgetApp.Infrastructure.Database;
using BudgetApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BudgetApp.Tests;

[TestFixture]
public class BudgetHandlerTests
{
    private AppDbContext _context;
    private BudgetRepository _budgetRepository;
    private IMapper _mapper;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _budgetRepository = new BudgetRepository(_context);

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

    [Test]
    public async Task CreateBudget_ShouldReturnBudgetDto_WithCorrectValues()
    {
        var userId = Guid.NewGuid();
        var handler = new CreateBudgetCommandHandler(_budgetRepository, _mapper);
        var command = new CreateBudgetCommand
        {
            UserId = userId,
            Name = "Maj 2026",
            Month = 5,
            Year = 2026,
            TotalAmount = 15000
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.UserId, Is.EqualTo(userId));
        Assert.That(result.Name, Is.EqualTo("Maj 2026"));
        Assert.That(result.Month, Is.EqualTo(5));
        Assert.That(result.Year, Is.EqualTo(2026));
        Assert.That(result.TotalAmount, Is.EqualTo(15000));
    }

    [Test]
    public async Task UpdateBudget_ShouldReturnUpdatedBudgetDto()
    {
        var budgetId = Guid.NewGuid();
        _context.Budgets.Add(new Budget
        {
            Id = budgetId,
            UserId = Guid.NewGuid(),
            Name = "Gammal budget",
            Month = 4,
            Year = 2026,
            TotalAmount = 10000,
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        var handler = new UpdateBudgetCommandHandler(_budgetRepository, _mapper);
        var command = new UpdateBudgetCommand
        {
            Id = budgetId,
            Name = "Maj 2026 uppdaterad",
            Month = 5,
            Year = 2026,
            TotalAmount = 20000
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Maj 2026 uppdaterad"));
        Assert.That(result.Month, Is.EqualTo(5));
        Assert.That(result.TotalAmount, Is.EqualTo(20000));
    }

    [Test]
    public async Task DeleteBudget_ShouldReturnTrue_WhenBudgetExists()
    {
        var budgetId = Guid.NewGuid();
        _context.Budgets.Add(new Budget
        {
            Id = budgetId,
            UserId = Guid.NewGuid(),
            Name = "Test",
            Month = 5,
            Year = 2026,
            TotalAmount = 10000,
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        var handler = new DeleteBudgetCommandHandler(_budgetRepository);
        var result = await handler.Handle(new DeleteBudgetCommand { Id = budgetId }, CancellationToken.None);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task DeleteBudget_ShouldReturnFalse_WhenBudgetDoesNotExist()
    {
        var handler = new DeleteBudgetCommandHandler(_budgetRepository);
        var result = await handler.Handle(new DeleteBudgetCommand { Id = Guid.NewGuid() }, CancellationToken.None);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task GetAllBudgets_ShouldReturnAllBudgetsForUser()
    {
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        _context.Budgets.AddRange(
            new Budget { Id = Guid.NewGuid(), UserId = userId, Name = "Januari", Month = 1, Year = 2026, TotalAmount = 10000, CreatedAt = DateTime.UtcNow },
            new Budget { Id = Guid.NewGuid(), UserId = userId, Name = "Februari", Month = 2, Year = 2026, TotalAmount = 10000, CreatedAt = DateTime.UtcNow },
            new Budget { Id = Guid.NewGuid(), UserId = otherUserId, Name = "Annan users budget", Month = 1, Year = 2026, TotalAmount = 5000, CreatedAt = DateTime.UtcNow }
        );
        await _context.SaveChangesAsync();

        var handler = new GetAllBudgetsQueryHandler(_budgetRepository, _mapper);
        var result = await handler.Handle(new GetAllBudgetsQuery { UserId = userId }, CancellationToken.None);

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].Name, Is.EqualTo("Januari"));
        Assert.That(result[1].Name, Is.EqualTo("Februari"));
    }

    [Test]
    public async Task GetBudgetById_ShouldReturnCorrectBudget()
    {
        var budgetId = Guid.NewGuid();
        _context.Budgets.Add(new Budget
        {
            Id = budgetId,
            UserId = Guid.NewGuid(),
            Name = "Maj 2026",
            Month = 5,
            Year = 2026,
            TotalAmount = 15000,
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        var handler = new GetBudgetByIdQueryHandler(_budgetRepository, _mapper);
        var result = await handler.Handle(new GetBudgetByIdQuery { Id = budgetId }, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(budgetId));
        Assert.That(result.Name, Is.EqualTo("Maj 2026"));
        Assert.That(result.TotalAmount, Is.EqualTo(15000));
    }
}
