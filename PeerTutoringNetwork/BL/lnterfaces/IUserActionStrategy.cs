using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.lnterfaces
{// 5. Strategy pattern -- ovo je Strategy pattern jer se koristi za izvršavanje različitih akcija
    public interface IUserActionStrategy
    {
        void ExecuteAction(int userId);
    }
}
