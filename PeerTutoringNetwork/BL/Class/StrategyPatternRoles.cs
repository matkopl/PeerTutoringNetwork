using BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Class
{
    public class StrategyPatternRoles
    {
        private readonly PeerTutoringNetworkContext _context;

        public StrategyPatternRoles(PeerTutoringNetworkContext context)
        {
            _context = context;
        }

        public void Run(int userId, int roleId)
        {
            var userContext = new UserContext();

            switch (roleId)
            {
                case 3: // Admin
                    userContext.SetStrategy(new AdminActionStrategy(_context));
                    break;
                case 2: // Teacher
                    userContext.SetStrategy(new TeacherActionStrategy(_context));
                    break;
                case 1: // Student
                    userContext.SetStrategy(new StudentActionStrategy(_context));
                    break;
                default:
                    Console.WriteLine("Invalid role.");
                    return;
            }

            userContext.ExecuteAction(userId);
        }
    }
}
