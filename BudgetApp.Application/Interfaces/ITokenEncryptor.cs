using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Application-lagret ska inte veta hur krypteringen görs 
//(det är en Infrastructure-detalj), bara att den finns.
namespace BudgetApp.Application.Interfaces;

public interface ITokenEncryptor
{
    string Encrypt (string plainText);
    string Decrypt(string cipherText);
}
