namespace BudgetApp.Application.Interfaces;

public interface ITinkClient
{
    string GetAuthorizationUrl(string state);
    Task<TinkTokenResult> ExchangeCodeForTokenAsync(string code);
}
