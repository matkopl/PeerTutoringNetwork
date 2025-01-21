using BL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PeerTutoringNetwork.DesignPatterns
{
    public interface IUtils
    {
        Task<IEnumerable<User>> GetUsersAsync();
    }

    public class Utils : IUtils
    {
        private readonly PeerTutoringNetworkContext _context;

        public Utils(PeerTutoringNetworkContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }
    }
}
