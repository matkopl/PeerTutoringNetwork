using BL.Models;
using Microsoft.EntityFrameworkCore;

namespace BL.Services;

public interface IUserService
{
    Task<User> GetUserById(int userId);
}

public class UserService : IUserService
{
    private readonly PeerTutoringNetworkContext _context;

    public UserService(PeerTutoringNetworkContext context)
    {
        _context = context;
    }

    public async Task<User> GetUserById(int userId)
    {
        User user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        return  user;
    }
    
    
}