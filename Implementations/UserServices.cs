using Backend.DbContexts;
using Backend.Helpers;
using Backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Implementations;

public class UserServices(CatalogContext context, IConfiguration config) : IUserServices
{
    private readonly CatalogContext _context = context;
    private readonly IConfiguration _config = config;

    public async Task<string?> AuthenticateAsync(string username, string password)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null)
            return null;

        var isValid = PasswordHelper.VerifyPassword(password, user.PasswordHash);

        if (!isValid)
            return null;

        return JwtHelper.GenerateToken(user.Username, user.Role, _config);
    }
}