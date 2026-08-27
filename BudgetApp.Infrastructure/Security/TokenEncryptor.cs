using BudgetApp.Application.Interfaces;
using Microsoft.AspNetCore.DataProtection;

namespace BudgetApp.Infrastructure.Security
{
    public class TokenEncryptor : ITokenEncryptor
    {
        private readonly IDataProtector _protector;

        public TokenEncryptor(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("BudgetApp.TinkTokens");
        }

        public string Encrypt(string plainText)
        {
            return _protector.Protect(plainText);
        }

        public string Decrypt(string cipherText)
        {
            return _protector.Unprotect(cipherText);
        }
    }
}
