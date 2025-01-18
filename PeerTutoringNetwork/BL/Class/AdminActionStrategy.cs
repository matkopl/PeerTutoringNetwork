using BL.lnterfaces;
using BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Class
{// 5. Strategy pattern -- ovo je Strategy pattern jer se koristi za izvršavanje različitih akcija u zavisnosti od korisnika
    public class AdminActionStrategy : IUserActionStrategy
    {
        private readonly PeerTutoringNetworkContext _context;

        public AdminActionStrategy(PeerTutoringNetworkContext context)
        {
            _context = context;
        }

        public void ExecuteAction(int userId)
        {
            Console.WriteLine("Admin Action: Fetching all users from the database.");
            var users = _context.Users.ToList();

            foreach (var user in users)
            {
                Console.WriteLine($"User ID: {user.UserId}, Username: {user.Username}");
            }
        }
    }
}
