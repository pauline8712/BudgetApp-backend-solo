using BudgetApp.Application.Interfaces;
using BudgetApp.Domain.Entities;
using BudgetApp.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace BudgetApp.Infrastructure.Repositories;

public class BankConnectionRepository : BaseRepository<BankConnection>, IBankConnectionRepository
{
    public BankConnectionRepository(AppDbContext context) : base(context)
    {

    }
}
