using AutoMapper;
using BudgetApp.Application.Features.Users.Commands;
using BudgetApp.Application.Features.Users.Queries;
using BudgetApp.Application.Mappings;
using BudgetApp.Domain.Entities;
using BudgetApp.Infrastructure.Database;
using BudgetApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BudgetApp.Tests;

[TestFixture]
public class UserHandlerTests
{
    private AppDbContext _context;
    private UserRepository _userRepository;
    private IMapper _mapper;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _userRepository = new UserRepository(_context);

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
    public async Task CreateUser_ShouldReturnUserDto_WithCorrectValues()
    {
        var handler = new CreateUserCommandHandler(_userRepository, _mapper);
        var command = new CreateUserCommand
        {
            Email = "test@example.com",
            PasswordHash = "hashedpassword123",
            Role = "User"
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Email, Is.EqualTo("test@example.com"));
        Assert.That(result.Role, Is.EqualTo("User"));
    }

    [Test]
    public async Task CreateUser_ShouldNotExposePasswordHash()
    {
        var handler = new CreateUserCommandHandler(_userRepository, _mapper);
        var command = new CreateUserCommand
        {
            Email = "secure@example.com",
            PasswordHash = "hemligtlösenord",
            Role = "User"
        };

        var result = await handler.Handle(command, CancellationToken.None);

        // UserDto ska inte ha PasswordHash — verifiera att det inte läcker ut
        var properties = result.GetType().GetProperties().Select(p => p.Name);
        Assert.That(properties, Does.Not.Contain("PasswordHash"));
    }

    [Test]
    public async Task UpdateUser_ShouldReturnUpdatedUserDto()
    {
        var userId = Guid.NewGuid();
        _context.Users.Add(new User
        {
            Id = userId,
            Email = "gammal@example.com",
            PasswordHash = "hash",
            Role = "User",
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        var handler = new UpdateUserCommandHandler(_userRepository, _mapper);
        var command = new UpdateUserCommand
        {
            Id = userId,
            Email = "ny@example.com",
            Role = "Admin"
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Email, Is.EqualTo("ny@example.com"));
        Assert.That(result.Role, Is.EqualTo("Admin"));
    }

    [Test]
    public async Task DeleteUser_ShouldReturnTrue_WhenUserExists()
    {
        var userId = Guid.NewGuid();
        _context.Users.Add(new User
        {
            Id = userId,
            Email = "delete@example.com",
            PasswordHash = "hash",
            Role = "User",
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        var handler = new DeleteUserCommandHandler(_userRepository);
        var result = await handler.Handle(new DeleteUserCommand { Id = userId }, CancellationToken.None);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task DeleteUser_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        var handler = new DeleteUserCommandHandler(_userRepository);
        var result = await handler.Handle(new DeleteUserCommand { Id = Guid.NewGuid() }, CancellationToken.None);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task GetAllUsers_ShouldReturnAllUsers()
    {
        _context.Users.AddRange(
            new User { Id = Guid.NewGuid(), Email = "user1@example.com", PasswordHash = "hash", Role = "User", CreatedAt = DateTime.UtcNow },
            new User { Id = Guid.NewGuid(), Email = "user2@example.com", PasswordHash = "hash", Role = "Admin", CreatedAt = DateTime.UtcNow }
        );
        await _context.SaveChangesAsync();

        var handler = new GetAllUsersQueryHandler(_userRepository, _mapper);
        var result = await handler.Handle(new GetAllUsersQuery(), CancellationToken.None);

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].Email, Is.EqualTo("user1@example.com"));
        Assert.That(result[1].Email, Is.EqualTo("user2@example.com"));
    }

    [Test]
    public async Task GetUserById_ShouldReturnCorrectUser()
    {
        var userId = Guid.NewGuid();
        _context.Users.Add(new User
        {
            Id = userId,
            Email = "hitta@example.com",
            PasswordHash = "hash",
            Role = "User",
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        var handler = new GetUserByIdQueryHandler(_userRepository, _mapper);
        var result = await handler.Handle(new GetUserByIdQuery { Id = userId }, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(userId));
        Assert.That(result.Email, Is.EqualTo("hitta@example.com"));
    }
}
