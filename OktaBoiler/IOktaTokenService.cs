namespace OktaBoiler
{
    public interface IOktaTokenService
    {
        Task<string> FetchTokenAsync();
    }
}
