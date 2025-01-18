using BL.Class;
using BL.lnterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Factories
{   // 2. Factory pattern -- ovo je Factory pattern jer se koristi za instanciranje objekata
    public static class UserFactory
    {
        public static IUser CreateUser(int role)
        {
            return role switch
            {
                3 => new AdminUser(),
                2 => new TeacherUser(),
                1 => new StudentUser(),
                _ => throw new ArgumentException("Invalid role type")
            };
        }
    }
}
