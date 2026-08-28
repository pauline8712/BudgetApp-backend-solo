using BudgetApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
//A temporary testcontroller file to see if the
//TokenEncryptor is working
namespace BudgetApp.API.Controllers;

[ApiController]
[Route("api/test")]

public class TestController : ControllerBase
{
    private readonly ITokenEncryptor _encryptor;

    public TestController (ITokenEncryptor encryptor)
    {
        _encryptor = encryptor;
    }
}

