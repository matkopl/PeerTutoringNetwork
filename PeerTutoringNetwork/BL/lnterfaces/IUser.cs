using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.lnterfaces
{
    public interface IUser
    {
        int UserId { get; set; }
        string Username { get; set; }
        string Email { get; set; }
        int Role { get; }
    }
}
