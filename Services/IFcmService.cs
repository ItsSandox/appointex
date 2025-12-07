namespace appointex.Services
{
    public interface IFcmService
    {
        Task<string> GetTokenAsync();
    }
}