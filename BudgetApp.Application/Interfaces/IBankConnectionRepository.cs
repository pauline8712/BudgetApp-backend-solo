using BudgetApp.Domain.Entities;

//Created this file because so we can actually save and retrieve Bankconnection data from the database via code.
namespace BudgetApp.Application.Interfaces
{
    public interface IBankConnectionRepository : IRepository<BankConnection>
    {
        //ärver från en gemensam IRepository<T> som TransactionRep och CategoryRep.
        //Det ger mig tillgång till grundläggande metoder.
        //Däremot i den här filen lägger vi till en egen metod GetByUser eftersom vi kommer,
        //alltid fråga "vilken bankkoppling har den här användaren?"
        Task<BankConnection?> GetByUserIdAsync(Guid userId);
    }
}
