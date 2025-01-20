using BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Managers
{
    // 1. Singleton pattern -- samo jedna instanca klase može postojati 
    public class SessionManager
    {
        private static SessionManager _instance; 
        private static readonly object _lock = new object(); // Lock za thread safety
        private readonly Dictionary<int, string> _activeSessions;

        private SessionManager()
        {
            _activeSessions = new Dictionary<int, string>();
        }

        // Singleton instanca
        public static SessionManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock) 
                    {
                        if (_instance == null)
                        {
                            _instance = new SessionManager();
                        }
                    }
                }
                return _instance;
            }
        }

        public void AddSession(int userId, string token)
        {
            if (!_activeSessions.ContainsKey(userId))
            {
                _activeSessions[userId] = token;
            }
        }

        public string GetSession(int userId)
        {
            return _activeSessions.TryGetValue(userId, out var token) ? token : null;
        }

        public void RemoveSession(int userId)
        {
            if (_activeSessions.ContainsKey(userId))
            {
                _activeSessions.Remove(userId);
            }
        }

        public List<int> GetActiveUsers()
        {
            return _activeSessions.Keys.ToList();
        }
    }
}
