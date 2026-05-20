using AutoMapper;
using BudgetApp.Application.Features.Categories.Commands;
using BudgetApp.Application.Features.Categories.Queries;
using BudgetApp.Application.Mappings;
using BudgetApp.Domain.Entities;
using BudgetApp.Infrastructure.Database;
using BudgetApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BudgetApp.Tests;

[TestFixture]
public class CategoryHandlerTests
{
    private AppDbContext _context;
    private CategoryRepository _categoryRepository;
    private IMapper _mapper;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
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

    [Test]
    public async Task CreateCategory_ShouldReturnCategoryDto_WithCorrectValues()
    {
        var handler = new CreateCategoryCommandHandler(_categoryRepository, _mapper);
        var command = new CreateCategoryCommand
        {
            BudgetId = Guid.NewGuid(),
            Name = "Mat",
            AllocatedAmount = 3000,
            IsWeekly = false,
            WeeklyAmount = null
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Mat"));
        Assert.That(result.AllocatedAmount, Is.EqualTo(3000));
        Assert.That(result.CurrentBalance, Is.EqualTo(3000));
    }

    [Test]
    public async Task GetCategories_ShouldReturnAllCategoriesForBudget()
    {
        var budgetId = Guid.NewGuid();

        _context.Categories.AddRange(
            new Category { Id = Guid.NewGuid(), BudgetId = budgetId, Name = "Mat", AllocatedAmount = 3000, CurrentBalance = 3000, CreatedAt = DateTime.UtcNow },
            new Category { Id = Guid.NewGuid(), BudgetId = budgetId, Name = "Hyra", AllocatedAmount = 6500, CurrentBalance = 6500, CreatedAt = DateTime.UtcNow }
        );
        await _context.SaveChangesAsync();

        var handler = new GetCategoriesQueryHandler(_categoryRepository, _mapper);
        var query = new GetCategoriesQuery { BudgetId = budgetId };

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].Name, Is.EqualTo("Mat"));
        Assert.That(result[1].Name, Is.EqualTo("Hyra"));
    }

    [Test]
    public async Task GetCategoryById_ShouldReturnCorrectCategory()
    {
        var categoryId = Guid.NewGuid();
        _context.Categories.Add(new Category
        {
            Id = categoryId,
            BudgetId = Guid.NewGuid(),
            Name = "Mat",
            AllocatedAmount = 3000,
            CurrentBalance = 3000,
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        var handler = new GetCategoryByIdQueryHandler(_categoryRepository, _mapper);
        var query = new GetCategoryByIdQuery { Id = categoryId };

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(categoryId));
        Assert.That(result.Name, Is.EqualTo("Mat"));
    }

    [Test]
    public async Task UpdateCategory_ShouldReturnUpdatedCategoryDto()
    {
        var categoryId = Guid.NewGuid();
        _context.Categories.Add(new Category
        {
            Id = categoryId,
            BudgetId = Guid.NewGuid(),
            Name = "Mat",
            AllocatedAmount = 3000,
            CurrentBalance = 3000,
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        var handler = new UpdateCategoryCommandHandler(_categoryRepository, _mapper);
        var command = new UpdateCategoryCommand
        {
            Id = categoryId,
            Name = "Mat uppdaterad",
            AllocatedAmount = 4000,
            IsWeekly = true,
            WeeklyAmount = 1000
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Mat uppdaterad"));
        Assert.That(result.AllocatedAmount, Is.EqualTo(4000));
        Assert.That(result.IsWeekly, Is.True);
        Assert.That(result.WeeklyAmount, Is.EqualTo(1000));
    }

    [Test]
    public async Task DeleteCategory_ShouldReturnTrue_WhenCategoryExists()
    {
        var categoryId = Guid.NewGuid();
        _context.Categories.Add(new Category
        {
            Id = categoryId,
            BudgetId = Guid.NewGuid(),
            Name = "Mat",
            AllocatedAmount = 3000,
            CurrentBalance = 3000,
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        var handler = new DeleteCategoryCommandHandler(_categoryRepository);
        var command = new DeleteCategoryCommand { Id = categoryId };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task DeleteCategory_ShouldReturnFalse_WhenCategoryDoesNotExist()
    {
        var handler = new DeleteCategoryCommandHandler(_categoryRepository);
        var command = new DeleteCategoryCommand { Id = Guid.NewGuid() };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result, Is.False);
    }
}
