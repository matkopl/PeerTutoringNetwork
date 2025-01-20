using BL.lnterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Class
{// 5. Strategy pattern -- ovo je Strategy pattern jer se koristi za izvršavanje različitih akcija
    public class UserContext
    {
        private IUserActionStrategy _strategy;

        public void SetStrategy(IUserActionStrategy strategy)
        {
            _strategy = strategy;
        }

        public void ExecuteAction(int userId)
        {
            if (_strategy == null)
            {
                throw new InvalidOperationException("Strategy is not set.");
            }

            _strategy.ExecuteAction(userId);
        }
    }
}
