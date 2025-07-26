namespace Backend.Interfaces
{
    public interface IUserServices
    {
        Task<string?> AuthenticateAsync(string username, string password);
    }
}