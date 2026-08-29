namespace BudgetApp.Application.Interfaces;

public interface ITinkClient
{
    string GetAuthorizationUrl(string state);
    Task<TinkTokenResult> ExchangeCodeForTokenAsync(string code);
}

public class TinkTokenResult
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
}
