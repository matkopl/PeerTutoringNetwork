using BL.Managers;
using BL.Models;
using PeerTutoringNetwork.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services
{       // 1. singleton pattern -- samo jedna instanca klase može postojati zbog lazy inicijalizacije
    public class AdminPanelService
    {
        private readonly PeerTutoringNetworkContext _context;

        public AdminPanelService(PeerTutoringNetworkContext context)
        {
            _context = context;
        }

        public bool Authenticate(string username, string password, out User authenticatedUser)
        {
            authenticatedUser = null;

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return false;

            var computedHash = PasswordHashProvider.GetHash(password, Convert.ToBase64String(user.PwdSalt));
            if (Convert.ToBase64String(user.PwdHash) != computedHash) return false;

            authenticatedUser = user;

            // Dodaj korisnika u SessionManager
            var token = JwtTokenProvider.CreateToken("SecureKeyHere", 60, username);
            SessionManager.Instance.AddSession(user.UserId, token);

            return true;
        }

        public void Logout(int userId)
        {
            SessionManager.Instance.RemoveSession(userId);
        }
    }
}
