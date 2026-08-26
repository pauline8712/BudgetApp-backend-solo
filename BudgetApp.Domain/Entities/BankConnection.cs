using System;

//A new database table that are looking att the connection between the user and their bank account for Tink.
//Because of this class we can see what user has connected with what bank account
//What acess-token to get the users transactions
//When the token is expiring

//Its in the Domain with the same reasons as the other classes- Which is "this user have a bankconnectiont). 
//Busniesslogic
namespace BudgetApp.Domain.Entities
{
    public class BankConnection
    {
       public Guid Id { get; set; }
       public Guid UserId { get; set; }
        public string AcessToken { get; set; } = string.Empty;
        public string RefreshToken {  get; set; } = string.Empty;
        public DateTime ExpiresAt {  get; set; }
        public string TinkUserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }
    }
}
